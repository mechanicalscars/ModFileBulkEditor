using Penumbra.GameData.Files;
using System.Text.Json;
using static Penumbra.UI.OptionSelectCombo;

namespace ModFileBulkEditor;

public class JSONFileCreator
{
    int groupOffset = 1;
    private string modPath;
    private bool splitHairAndFaceOptions;

    private static List<(string, string, string, bool)> diffuseMappings = [
        ("Light Marble", Constants.ScarStonex4TexturePath, Constants.ScarStoneTexturePath, false),
        ("Dark Marble", Constants.ScarStoneDarkerx4TexturePath, Constants.ScarStoneDarkerTexturePath, false),
        ( "Granite", Constants.ScarGranitex4TexturePath, Constants.ScarGraniteTexturePath, false),
        ("Pure White", Constants.WhiteTexturePath, Constants.WhiteTexturePath, true),
        ("Pure Black", Constants.BlackTexturePath, Constants.BlackTexturePath, true)
        ];

    public JSONFileCreator(string modPath, bool splitHairAndFaceOptions)
    {
        this.modPath = modPath;
        this.splitHairAndFaceOptions = splitHairAndFaceOptions;
    }

    public void WriteJSONFile<T>(string fileName, T seralizeTarget, bool useGroup = true)
    {
        if (useGroup)
        {
            fileName = $"group_{groupOffset:000}_{fileName}.json";
            groupOffset += 1;
        }
        var fileFullPath = Path.Combine(modPath, fileName);
        var jsonMapping = JsonSerializer.Serialize<T>(seralizeTarget, Constants.jsonSerializerOptions);
        File.WriteAllText(fileFullPath, jsonMapping);
    }

    /*
     * Assumes that the directories will be of the form:
     * - Mod (modPath)
     * -- Option (optionDirectory)
     * --- SubOption
     * 
     * And assumes that each file is replacing itself in game (VERY Naive, need to get some stuff in here about that. Just hard coded? Curses.)
     * Inputs:
     * optionsDirectory: the directory that we are turing into options. Should have subdirectories for each option.
     * additonalOverwrites: any files in the directories that require additional tlc; like if specific material files overwrite multiple ones in game.
     * type: Single or Multi; the mode for the options.
     */
    public void WriteFileRedirectionsToJSONFile(string optionDirectory, Dictionary<string, string>? additionalMappings = null)
    {
        var optionPath = Path.Join(modPath, optionDirectory);

        var childDirectories = Directory.GetDirectories(optionPath).ToList();

        var modOptions = GetOptionsFromDirectory(childDirectories, additionalMappings);

        var modFile = new Models.PenumbraModFile() { Name = optionDirectory, Type = "Single", Options = [new Models.PenumbraModOption { Name = "Do Not Install" }] };
        modFile.Options.AddRange(modOptions);

        WriteJSONFile(optionDirectory, modFile);
    }

    private List<Models.PenumbraModOption> GetOptionsFromDirectory(List<string> directories, Dictionary<string, string>? additionalMappings = null)
    {
        List<Models.PenumbraModOption> modOptions = [];
        foreach (var directory in directories)
        {
            var mappings = GetNaiveMappingsFromDirectory(directory, directory);
            if (additionalMappings != null)
            {
                var optionSubpath = Path.GetRelativePath(modPath, directory);
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

    private Dictionary<string, string> GetNaiveMappingsFromDirectory(string directoryPath, string? originalPath = null)
    {
        originalPath ??= directoryPath;

        Dictionary<string, string> mappings = [];
        string[] childDirectories = Directory.GetDirectories(directoryPath);
        string[] childFiles = Directory.GetFiles(directoryPath);
        foreach (string directory in childDirectories)
        {
            var directoryMappings = GetNaiveMappingsFromDirectory(directory, originalPath);
            foreach (var newMapping in directoryMappings)
            {
                mappings[newMapping.Key] = newMapping.Value;
            }
        }
        foreach (string file in childFiles)
        {
            var modSubpath = Path.GetRelativePath(modPath, file);
            var optionSubpath = Path.GetRelativePath(originalPath, file);
            var ffxivStyleName = optionSubpath.Replace("\\", "/");
            mappings[ffxivStyleName] = modSubpath;
        }
        return mappings;
    }

    public void WriteStatueRequiredFiles(string materialsDirectory,
        List<string>? naiveMappingFolders = null)
    {
        var materialsFullPath = Path.Combine(modPath, materialsDirectory);

        var texturePaths = GetStatueTexturesFromMaterialsDirectory(materialsFullPath);

        Dictionary<string, Dictionary<string, string>> mappings = new() {
            { "Index", texturePaths.IndexTextures.ToDictionary(x => x, x => Constants.WhiteTexturePath) },
            { "Masks", texturePaths.MaskTextures.ToDictionary(x => x, x => Constants.WhiteTexturePath) }
        };

        if (naiveMappingFolders != null)
        {
            foreach (var mappingFolder in naiveMappingFolders)
            {
                var mappingFolderFullPath = Path.Combine(modPath, mappingFolder);

                var directoryMappings = GetNaiveMappingsFromDirectory(mappingFolderFullPath, modPath);
                mappings[mappingFolder] = directoryMappings;

            }
        }
        if(splitHairAndFaceOptions)
        {
            foreach((var shortName, var pathsToSearch) in Constants.alternativePartsSplits)
            {
                Dictionary<string, Dictionary<string, string>> partMappings = [];
                foreach(var key in mappings.Keys)
                {
                    (var leftOverMapping, var matchingMapping) = splitMappingByRegexInKey(mappings[key], pathsToSearch);
                    if(leftOverMapping.Count > 0)
                    {
                        mappings[key] = leftOverMapping;
                    } else
                    {
                        mappings.Remove(key);
                    }

                    if (matchingMapping.Count > 0)
                    {
                        partMappings[key] = matchingMapping;
                    }
                }
                if(partMappings.Count > 0)
                {
                    WriteRequireFieldsForDifferentBodyParts($"{shortName} {Constants.requiredFilesOptionName}", partMappings);
                }
            }
        } 
        
        if(mappings.Count > 0)
        {
            WriteRequireFieldsForDifferentBodyParts(Constants.requiredFilesOptionName, mappings);
        }

        WriteDiffuseFile(texturePaths.DiffuseTextures);
    }

    private void WriteRequireFieldsForDifferentBodyParts(string fileName, Dictionary<string,Dictionary<string,string>> mappings) 
    {
        List<Models.PenumbraModOption> options = [];
        foreach ((var mapName, var mapping) in mappings)
        {
            if (mapping.Count > 0)
            {
                options.Add(new Models.PenumbraModOption() { Name = mapName, Files = mapping });
            }
        }
        // saves default options on and off state as a binary number, so we want them all to be 1;
        int defaultOptions = (int)(Math.Pow(2, options.Count) - 1);

        var modFile = new Models.PenumbraModFile() { Name = fileName, Type = "Multi", Options = options, Priority = 0, DefaultSettings = defaultOptions };
        WriteJSONFile(fileName, modFile);
    }
    
    private (Dictionary<string, string>, Dictionary<string, string>) splitMappingByRegexInKey(Dictionary<string,string> originalMappings, List<string> pathsToSearch)
    {
        var falseMappings = originalMappings.Where(x => !pathsToSearch.Any(s => x.Key.Contains(s))).ToDictionary();
        var trueMappings = originalMappings.Where(x => pathsToSearch.Any(s => x.Key.Contains(s))).ToDictionary();
        return (falseMappings, trueMappings); 
    }

    public void WriteNormalMapsFile(string materialsDirectory, string? normalsDirectory = null)
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
        WriteJSONFile(Constants.normalMapsOptionName, modFile);
    }

    private void WriteDiffuseFile(HashSet<string> diffusePaths)
    {
        var modFile = new Models.PenumbraModFile() { Name = Constants.baseTexturesOptionName, Type = "Single", Options = [], Priority = 0 };
        modFile.Options.Add(new Models.PenumbraModOption { Name = "Do Not Install" });
        foreach((var name, var x4TexturePath, var regularTexturePath, var fileSwaps)  in diffuseMappings)
        {
            modFile.Options.Add(MakeDiffuseMappings(diffusePaths, name, x4TexturePath, regularTexturePath, fileSwaps));
        }
        WriteJSONFile(Constants.baseTexturesOptionName, modFile);
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
