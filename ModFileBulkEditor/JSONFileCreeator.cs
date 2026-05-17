using Penumbra.GameData.Files;
using System.Text.Json;

namespace ModFileBulkEditor;

public class JSONFileCreator
{
    public struct PenumbraMetaFile
    {
        public int FileVersion = 3;
        public string Name = "";
        public string Author = "";
        public string Description = "";
        public string Image = "";
        public string Version = "";
        public string Website = "";

        public PenumbraMetaFile() {}
    }

    public struct PenumbraDefaultSubMod
    {
        public int Version = 0; 
        public Dictionary<string, string>? Files;
        public Dictionary<string, string>? FileSwaps;

        public PenumbraDefaultSubMod() {}
    }

    public struct PenumbraModFile
    {
        public int Version = 0;
        public string Name = "";
        public string Description = "";
        public string Image = "";
        public int Page = 0;
        public int Priority = 0;
        public string Type = "Single";
        public int DefaultSettings = 0;
        public List<PenumbraModOption> Options = [];

        public PenumbraModFile() {}
    }

    public struct PenumbraModOption
    {
        public string Name = "";
        public string Description = "";
        public Dictionary<string, string>? Files;
        public Dictionary<string, string>? FileSwaps;

        public PenumbraModOption() {}
    }

    private struct MaterialTexturePaths
    {
        public HashSet<string> NormalTextures = [];
        public HashSet<string> DiffuseTextures = [];
        public HashSet<string> MaskTextures = [];
        public HashSet<string> IndexTextures = [];

        public MaterialTexturePaths() {}

        public readonly void UnionWith(MaterialTexturePaths paths)
        {
            NormalTextures.UnionWith(paths.NormalTextures);
            DiffuseTextures.UnionWith(paths.DiffuseTextures);
            MaskTextures.UnionWith(paths.MaskTextures);
            IndexTextures.UnionWith(paths.IndexTextures);
        }
    }

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

        var modFile = new PenumbraModFile() { Name = optionDirectory, Type = type, Options = [new PenumbraModOption { Name = "Do Not Install" }] };
        modFile.Options.AddRange(modOptions);
         
        var jsonMapping = JsonSerializer.Serialize<PenumbraModFile>(modFile, Constants.jsonSerializerOptions);
        File.WriteAllText(Path.Join(modPath, outputFileName), jsonMapping);
        
    }

    private static List<PenumbraModOption> GetOptionsFromDirectory(string baseModPath, List<string> directories, Dictionary<string, string>? additionalMappings = null)
    {
        List<PenumbraModOption> modOptions = [];
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
            modOptions.Add(new PenumbraModOption() { Name = new DirectoryInfo(directory).Name, Files = mappings });
        }
        return modOptions;
    }

    private static Dictionary<string, string> GetNaiveMappingsFromDirectory(string directoryPath, string baseModPath, string optionPath)
    {
        Dictionary<string, string> mappings = [];
        var directoryName = Path.GetDirectoryName(directoryPath);
        string[] childDirectories = Directory.GetDirectories(directoryPath);
        string[] childFiles = Directory.GetFiles(directoryPath);
        foreach (string directory in childDirectories)
        {
            var directoryMappings = GetNaiveMappingsFromDirectory(directory, baseModPath, optionPath);
            foreach (var newMapping in directoryMappings)
            {
                mappings[newMapping.Key] = newMapping.Value;
            }
        }
        foreach (string file in childFiles)
        {
            var modSubpath = Path.GetRelativePath(baseModPath, file);
            var optionSubpath = Path.GetRelativePath(optionPath, file);
            var ffxivStyleName = optionSubpath.Replace("\\", "/");
            mappings[ffxivStyleName] = modSubpath;
        }
        return mappings;
    }

    /*
     * Statue Specific (lol because who else is using this, Scar?)
     * Takes a listings of a directory full of material files, and outputs all their index and mask textures into a dictionary.
     * Then takes normal and model files and 1:1's them into the same option file.
     * then takes the diffuses and makes a different group file that has all the different options we're using.
     */
    public static void WriteStatueRequiredFiles(string modPath, string materialsDirectory, string normalTextureDirectory, string modelDirectory, string requiredFilesFileName, string diffuseFileName)
    {
        var materialsFullPath = Path.Combine(modPath, materialsDirectory);
        var requiredFilesFullPath = Path.Combine(modPath, requiredFilesFileName);
        var diffuseFilesFullPath = Path.Combine(modPath, diffuseFileName);
        var modelsFullPath = Path.Combine(modPath, modelDirectory);
        var normalTextureFullPath = Path.Combine(modPath, normalTextureDirectory);

        var texturePaths = GetStatueTexturesFromMaterialsDirectory(materialsFullPath);

        var indexMappings = texturePaths.IndexTextures.ToDictionary(x => x, x => Constants.WhiteTexturePath);
        var maskMappings = texturePaths.MaskTextures.ToDictionary(x => x, x => Constants.WhiteTexturePath);

        // Get normalMaps from installed normal folder becuase we don't have a new normal for every material, unlike indexes/diffuses/masks.
        var modelMappings = GetNaiveMappingsFromDirectory(modelsFullPath, modPath, modelsFullPath);
        var normalMappings = GetNaiveMappingsFromDirectory(normalTextureFullPath, modPath, normalTextureFullPath);

        var indexOptions = new PenumbraModOption() { Name = "Indexs", FileSwaps = indexMappings };
        var maskOptions = new PenumbraModOption() { Name = "Masks", FileSwaps = maskMappings };
        var modelOptions = new PenumbraModOption() { Name = "Models", Files = modelMappings };
        var normalOptions = new PenumbraModOption() { Name = "Normals", Files = normalMappings };

        List<PenumbraModOption> options = [indexOptions, maskOptions, modelOptions, normalOptions];
        // saves default options on and off state as a binary number, so we want them all to be 1;
        int defaultOptions = (int)(Math.Pow(2,options.Count) - 1);

        var modFile = new PenumbraModFile() { Name = "Requried Materials", Type = "Multi", Options = options, Priority = 0, DefaultSettings = defaultOptions};
        var jsonMapping = JsonSerializer.Serialize(modFile, Constants.jsonSerializerOptions);
        File.WriteAllText(requiredFilesFullPath, jsonMapping);

        WriteDiffuseFile(diffuseFilesFullPath, texturePaths.DiffuseTextures);
    }

    private static void WriteDiffuseFile(string diffuseFileName, HashSet<string> diffusePaths)
    {
        var modFile = new PenumbraModFile() { Name = "Base Textures", Type = "Single", Options = [], Priority = 0 };
        modFile.Options.Add(new PenumbraModOption { Name = "Do Not Install" });
        modFile.Options.Add(MakeDiffuseMappings(diffusePaths, "Light Marble", Constants.ScarStonex4TexturePath, Constants.ScarStoneTexturePath));
        modFile.Options.Add(MakeDiffuseMappings(diffusePaths, "Dark Marble", Constants.ScarStoneDarkerx4TexturePath, Constants.ScarStoneDarkerx4TexturePath));
        modFile.Options.Add(MakeDiffuseMappings(diffusePaths, "Granite", Constants.ScarGranitex4TexturePath, Constants.ScarGraniteTexturePath));
        modFile.Options.Add(MakeDiffuseMappings(diffusePaths, "Pure White", Constants.WhiteTexturePath, Constants.WhiteTexturePath));

        var jsonMapping = JsonSerializer.Serialize(modFile, Constants.jsonSerializerOptions);
        File.WriteAllText(diffuseFileName, jsonMapping);
    }

    private static PenumbraModOption MakeDiffuseMappings(HashSet<string> diffusePaths, string name, string x4TexturePath, string texturePath)
    {
        var diffuseMaps = diffusePaths.ToDictionary(x => x, x => Constants.X4TextureSubpaths.Any(s => x.Contains(s)) ? x4TexturePath : texturePath);
        return new PenumbraModOption { Name = name, Files = diffuseMaps };
    }

    private static MaterialTexturePaths GetStatueTexturesFromMaterialsDirectory(string directoryPath)
    {
        MaterialTexturePaths texturePaths = new();
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

    private static MaterialTexturePaths GetMaterialTextures(string materialPath)
    {
        byte[] file = File.ReadAllBytes(materialPath);
        MtrlFile material = new(file);

        var normalPath = Utils.GetSamplerTexturePath(material, ShpkFile.NormalSamplerId);
        var diffusePath = Utils.GetSamplerTexturePath(material, ShpkFile.DiffuseSamplerId);
        var indexPath = Utils.GetSamplerTexturePath(material, ShpkFile.IndexSamplerId);
        var maskPath = Utils.GetSamplerTexturePath(material, ShpkFile.MaskSamplerId);

        return new MaterialTexturePaths()
        {
            NormalTextures = [normalPath],
            DiffuseTextures = [diffusePath],
            IndexTextures = [indexPath],
            MaskTextures = [maskPath]
        };
    }
}