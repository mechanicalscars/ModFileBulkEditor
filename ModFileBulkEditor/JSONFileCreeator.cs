using Penumbra.GameData.Files;
using System.Text.Json;

namespace ModFileBulkEditor;

public class JSONFileCreator
{
    public static void WriteJSONFile<T>(string modPath, string metaFilePath, T metaFile)
    {
        var metaFullPath = Path.Combine(modPath, metaFilePath);
        var jsonMapping = JsonSerializer.Serialize<T>(metaFile, Constants.jsonSerializerOptions);
        File.WriteAllText(metaFullPath, jsonMapping);
    }

    /*
     * Assumes that the directories will be of the form:
     * - Mod (modPath)
     * -- Option (optionDirectory)
     * --- SubOption
     * 
     * And assumes that each file is replacing itself in game (VERY Naive, need to get some stuff in here about that. Just hard coded? Curses.)
     * Inputs:
     * modPath: the path to the main mod, where the json groups will be written to
     * optionsSubPath: the directory that we are turing into options. Should have subdirectories for each option.
     * additonalOverwrites: any files in the directories that require additional tlc; like if specific material files overwrite multiple ones in game.
     * type: Single or Multi; the mode for the options.
     */
    public static void WriteFileRedirectionsToJSONFile(string modPath, string optionDirectory, string outputFileName, Dictionary<string, string>? additionalMappings = null, string type = "Single")
    {
        var optionPath = Path.Join(modPath, optionDirectory);

        var childDirectories = Directory.GetDirectories(optionPath).ToList();

        var modOptions = GetOptionsFromDirectory(modPath, childDirectories, additionalMappings);

        var modFile = new Models.PenumbraModFile() { Name = optionDirectory, Type = type, Options = [new Models.PenumbraModOption { Name = "Do Not Install" }] };
        modFile.Options.AddRange(modOptions);

        var jsonMapping = JsonSerializer.Serialize<Models.PenumbraModFile>(modFile, Constants.jsonSerializerOptions);
        File.WriteAllText(Path.Join(modPath, outputFileName), jsonMapping);

    }

    private static List<Models.PenumbraModOption> GetOptionsFromDirectory(string baseModPath, List<string> directories, Dictionary<string, string>? additionalMappings = null)
    {
        List<Models.PenumbraModOption> modOptions = [];
        foreach (var directory in directories)
        {
            var mappings = GetNaiveMappingsFromDirectory(directory, baseModPath, directory);
            if (additionalMappings != null)
            {
                var optionSubpath = Path.GetRelativePath(baseModPath, directory);
                foreach (var overwrite in additionalMappings)
                {
                    var newOverWritePath = Path.Combine(optionSubpath, overwrite.Value);
                    mappings[overwrite.Key] = newOverWritePath;
                }
            }
            modOptions.Add(new Models.PenumbraModOption() { Name = new DirectoryInfo(directory).Name, Files = mappings });
        }
        return modOptions;
    }

    private static Dictionary<string, string> GetNaiveMappingsFromDirectory(string directoryPath, string baseModPath, string? originalPath = null)
    {
        originalPath ??= directoryPath;

        Dictionary<string, string> mappings = [];
        string[] childDirectories = Directory.GetDirectories(directoryPath);
        string[] childFiles = Directory.GetFiles(directoryPath);
        foreach (string directory in childDirectories)
        {
            var directoryMappings = GetNaiveMappingsFromDirectory(directory, baseModPath, originalPath);
            foreach (var newMapping in directoryMappings)
            {
                mappings[newMapping.Key] = newMapping.Value;
            }
        }
        foreach (string file in childFiles)
        {
            var modSubpath = Path.GetRelativePath(baseModPath, file);
            var optionSubpath = Path.GetRelativePath(originalPath, file);
            var ffxivStyleName = optionSubpath.Replace("\\", "/");
            mappings[ffxivStyleName] = modSubpath;
        }
        return mappings;
    }

    public static void WriteStatueRequiredFiles(string modPath,
        string materialsDirectory,
        List<string>? naiveMappingFolders = null)
    {
        var materialsFullPath = Path.Combine(modPath, materialsDirectory);

        var texturePaths = GetStatueTexturesFromMaterialsDirectory(materialsFullPath);

        var indexMappings = texturePaths.IndexTextures.ToDictionary(x => x, x => Constants.WhiteTexturePath);
        var maskMappings = texturePaths.MaskTextures.ToDictionary(x => x, x => Constants.WhiteTexturePath);
        var indexOptions = new Models.PenumbraModOption() { Name = "Indexs", FileSwaps = indexMappings };
        var maskOptions = new Models.PenumbraModOption() { Name = "Masks", FileSwaps = maskMappings };

        List<Models.PenumbraModOption> options = [indexOptions, maskOptions];

        if (naiveMappingFolders != null)
        {
            foreach (var mappingFolder in naiveMappingFolders)
            {
                var mappingFolderFullPath = Path.Combine(modPath, mappingFolder);

                var mappings = GetNaiveMappingsFromDirectory(mappingFolderFullPath, modPath);

                options.Add(new Models.PenumbraModOption() { Name = mappingFolder, Files = mappings });
            }
        }

        // saves default options on and off state as a binary number, so we want them all to be 1;
        int defaultOptions = (int)(Math.Pow(2, options.Count) - 1);

        var modFile = new Models.PenumbraModFile() { Name = Constants.requiredFilesOptionName, Type = "Multi", Options = options, Priority = 0, DefaultSettings = defaultOptions };
        var jsonMapping = JsonSerializer.Serialize(modFile, Constants.jsonSerializerOptions);

        var requiredFilesFullPath = Path.Combine(modPath, Constants.requiredFilesOutputJSONFile);
        File.WriteAllText(requiredFilesFullPath, jsonMapping);

        WriteDiffuseFile(modPath, texturePaths.DiffuseTextures);
    }

    public static void WriteNormalMapsFile(string modPath, string materialsDirectory, string? normalsDirectory = null)
    {
        List<Models.PenumbraModOption> options = [];
        int defaultSettings = 0;

        if (normalsDirectory != null)
        {
            var normalsFullPath = Path.Combine(modPath, normalsDirectory);
            // Get the true normalMaps from installed normal folder becuase we don't have a new normal for every material, unlike indexes/diffuses/masks.
            var normalMappings = GetNaiveMappingsFromDirectory(normalsFullPath, modPath);
            var normalOptions = new Models.PenumbraModOption() { Name = "Vanilla and Edited Normals", Files = normalMappings, Description = "Massaged normal maps from objects that use skin or hair shaders so they play nicely with the character shader. (Required even if you use the below)" };
            options.Add(normalOptions);
            defaultSettings = 1;
        }

        var materialsFullPath = Path.Combine(modPath, materialsDirectory);

        var texturePaths = GetStatueTexturesFromMaterialsDirectory(materialsFullPath);

        // Ignore hair for Smooth normals cause it does funky stuff.
        var normalSmoothMappings = texturePaths.NormalTextures.Where(x => !Constants.SmoothIgnoringSubPaths.Any(s => x.Contains(s))).ToDictionary(x => x, x => Constants.SmoothNomalsTexturePath);
        var normalSmoothOptions = new Models.PenumbraModOption() { Name = "Smooth Normals", Files = normalSmoothMappings, Priority = 1, Description = "For when you want your statues to look a little bit simpler. Hair not included." };
        options.Add(normalSmoothOptions);

        var modFile = new Models.PenumbraModFile() { Name = Constants.normalMapsOptionName, Type = "Multi", Options = options, Priority = 0, DefaultSettings = defaultSettings };
        var jsonMapping = JsonSerializer.Serialize(modFile, Constants.jsonSerializerOptions);

        var normalMapFileFullPath = Path.Combine(modPath, Constants.normalMapsOutputJSONFile);
        File.WriteAllText(normalMapFileFullPath, jsonMapping);
    }

    private static void WriteDiffuseFile(string modPath, HashSet<string> diffusePaths, bool mainMod = true)
    {
        var modFile = new Models.PenumbraModFile() { Name = Constants.baseTexturesOptionName, Type = "Single", Options = [], Priority = 0 };
        modFile.Options.Add(new Models.PenumbraModOption { Name = "Do Not Install" });
        modFile.Options.Add(MakeDiffuseMappings(diffusePaths, "Light Marble", Constants.ScarStonex4TexturePath, Constants.ScarStoneTexturePath, mainMod));
        modFile.Options.Add(MakeDiffuseMappings(diffusePaths, "Dark Marble", Constants.ScarStoneDarkerx4TexturePath, Constants.ScarStoneDarkerTexturePath, mainMod));
        modFile.Options.Add(MakeDiffuseMappings(diffusePaths, "Granite", Constants.ScarGranitex4TexturePath, Constants.ScarGraniteTexturePath, mainMod));
        modFile.Options.Add(MakeDiffuseMappings(diffusePaths, "Pure White", Constants.WhiteTexturePath, Constants.WhiteTexturePath, true));
        modFile.Options.Add(MakeDiffuseMappings(diffusePaths, "Pure Black", Constants.BlackTexturePath, Constants.BlackTexturePath, true));

        var jsonMapping = JsonSerializer.Serialize(modFile, Constants.jsonSerializerOptions);
        var diffuseFilesFullPath = Path.Combine(modPath, Constants.baseTexturesOutputJSONFile);
        File.WriteAllText(diffuseFilesFullPath, jsonMapping);
    }

    private static Models.PenumbraModOption MakeDiffuseMappings(HashSet<string> diffusePaths, string name, string x4TexturePath, string texturePath, bool fileSwaps = false)
    {
        var diffuseMaps = diffusePaths.ToDictionary(x => x, x => Constants.X4TextureSubpaths.Any(s => x.Contains(s)) ? x4TexturePath : texturePath);
        var modOption = new Models.PenumbraModOption { Name = name };

        if (fileSwaps)
        {
            modOption.FileSwaps = diffuseMaps;
        }
        else
        {
            modOption.Files = diffuseMaps;
        }
        return modOption;
    }

    private static Models.MaterialTexturePaths GetStatueTexturesFromMaterialsDirectory(string directoryPath)
    {
        Models.MaterialTexturePaths texturePaths = new();
        string[] childDirectories = Directory.GetDirectories(directoryPath);
        string[] childFiles = Directory.GetFiles(directoryPath);
        foreach (string directory in childDirectories)
        {
            var directoryTexturePaths = GetStatueTexturesFromMaterialsDirectory(directory);
            texturePaths.UnionWith(directoryTexturePaths);
        }
        foreach (string file in childFiles)
        {
            if (File.Exists(file) && file.EndsWith(".mtrl"))
            {
                var filePaths = GetMaterialTextures(file);
                texturePaths.UnionWith(filePaths);
            }
        }
        return texturePaths;
    }

    private static Models.MaterialTexturePaths GetMaterialTextures(string materialPath)
    {
        byte[] file = File.ReadAllBytes(materialPath);
        MtrlFile material = new(file);

        var normalPath = Utils.GetSamplerTexturePath(material, ShpkFile.NormalSamplerId);
        var diffusePath = Utils.GetSamplerTexturePath(material, ShpkFile.DiffuseSamplerId);
        var indexPath = Utils.GetSamplerTexturePath(material, ShpkFile.IndexSamplerId);
        var maskPath = Utils.GetSamplerTexturePath(material, ShpkFile.MaskSamplerId);

        return new Models.MaterialTexturePaths()
        {
            NormalTextures = [normalPath],
            DiffuseTextures = [diffusePath],
            IndexTextures = [indexPath],
            MaskTextures = [maskPath]
        };
    }
}