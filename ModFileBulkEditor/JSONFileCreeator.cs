using Penumbra.GameData.Files;
using System.Text.Json;

namespace ModFileBulkEditor;

using Models;

public class JSONFileCreator
{
    int groupOffset = 1;
    private readonly string modPath;
    private readonly bool splitOptions;

    public JSONFileCreator(string modPath, bool splitOptions)
    {
        this.modPath = modPath;
        this.splitOptions = splitOptions;
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
        if (!Path.Exists(optionPath))
        {
            return;
        }
        var childDirectories = Directory.GetDirectories(optionPath).ToList();

        var modOptions = GetOptionsFromDirectory(childDirectories, additionalMappings);

        WriteMappingsToFiles(optionDirectory, modOptions, true);
    }

    private List<PenumbraModOption> GetOptionsFromDirectory(List<string> directories, Dictionary<string, string>? additionalMappings = null)
    {
        List<PenumbraModOption> modOptions = [];
        foreach (var directory in directories)
        {
            var mappings = GetNaiveMappingsFromDirectory(directory);
            if (additionalMappings != null)
            {
                var optionSubpath = Path.GetRelativePath(modPath, directory);
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
        string? normalsDirectory = null,
        List<string>? naiveMappingFolders = null,
        bool mergeRequiredFiles = true)
    {
        var materialsFullPath = Path.Combine(modPath, materialsDirectory);

        var texturePaths = GetStatueTexturesFromMaterialsDirectory(materialsFullPath);

        List<PenumbraModOption> modOptions = [
           new PenumbraModOption() {Name = Constants.indexOptionName, FileSwaps = texturePaths.IndexTextures.ToDictionary(x => x, x => Constants.WhiteTexturePath) },
           new PenumbraModOption() {Name = Constants.maskOptionsName, FileSwaps = texturePaths.MaskTextures.ToDictionary(x => x, x => Constants.WhiteTexturePath) }
        ];

        if (naiveMappingFolders != null)
        {
            foreach (var mappingFolder in naiveMappingFolders)
            {
                var mappingFolderFullPath = Path.Combine(modPath, mappingFolder);

                var directoryMappings = GetNaiveMappingsFromDirectory(mappingFolderFullPath);
                modOptions.Add(new PenumbraModOption() { Name = mappingFolder, Files = directoryMappings });

            }
        }

        var normalMappings = GetNormalMappings(materialsDirectory, normalsDirectory);

        if (mergeRequiredFiles)
        {
            var mergedRequiredFiles = new PenumbraModOption { Name = Constants.requiredFilesOptionName };
            foreach (var modOption in modOptions)
            {
                mergedRequiredFiles.Add(modOption);
            }

            if (normalsDirectory != null)
            {
                mergedRequiredFiles.Add(normalMappings[0]);
                normalMappings.RemoveAt(0);
            }
            modOptions = [mergedRequiredFiles];
        }

        modOptions.AddRange(normalMappings);
        

        WriteMappingsToFiles(Constants.requiredFilesGroupName, modOptions);

        WriteDiffuseFile(texturePaths.DiffuseTextures);
    }

    private void WriteDiffuseFile(HashSet<string> diffusePaths)
    {
        List<PenumbraModOption> modOptions = [];
        foreach ((var name, var x4TexturePath, var texturePath) in Constants.diffuseMappings)
        {
            var diffuseMaps = diffusePaths.ToDictionary(x => x, x => Constants.X4TextureSubpaths.Any(s => x.Contains(s)) ? x4TexturePath : texturePath);
            var modOption = new PenumbraModOption() { Name = name };
            if (Constants.fileSwapOptionNames.Contains(name))
            {
                modOption.FileSwaps = diffuseMaps;
            } else
            {
                modOption.Files = diffuseMaps;
            }
            modOptions.Add(modOption);
        }
        WriteMappingsToFiles(Constants.baseTexturesGroupName, modOptions, true);

    }

    private void WriteMappingsToFiles(string fileName, List<PenumbraModOption> modOptions, bool single = false)
    {

        if (splitOptions)
        {
            foreach ((var shortName, var pathsToSearch) in Constants.alternativePartsSplits)
            {
                List<PenumbraModOption> partMappings = [];
                for (int i = 0; i < modOptions.Count; i++)
                {
                    (modOptions[i], var matchingModOption) = SplitMappingByRegexInKey(modOptions[i], pathsToSearch);
                    if (matchingModOption != null)
                    {
                        partMappings.Add((PenumbraModOption)matchingModOption);
                    }
                }
                if (partMappings.Count > 0)
                {
                    WriteMappingsToFileAsOptions($"{shortName} {fileName}", partMappings, single);
                }
            }

            modOptions = RemoveEmptyModOptions(modOptions);
        }

        if (modOptions.Count > 0)
        {
            WriteMappingsToFileAsOptions($"Other {fileName}", modOptions, single);
        }
    }

    private static List<PenumbraModOption> RemoveEmptyModOptions(List<PenumbraModOption> modOptions)
    {
        List<PenumbraModOption> newModOptions = [];
        foreach(var modOption in modOptions)
        {
            if(modOption.Files.Count > 0 || modOption.FileSwaps.Count > 0)
            {
                newModOptions.Add(modOption);
            }
        }
        return newModOptions;
    }

    // Note: This is naive and activates Smooth Normals; figure out a way to arbitrarily turn off specific things if multi, or target specific default options for single.
    private void WriteMappingsToFileAsOptions(string fileName, List<PenumbraModOption> modOptions, bool single = false) 
    {
        if (modOptions.Count == 0) { return; }

        List<PenumbraModOption> options = [];
        
        if(single)
        {
            options.Add(new PenumbraModOption { Name = "Do Not Install" });
        }
        
        options.AddRange(modOptions);

        var type = "Single";
        int defaultOptions = 1;

        if (!single)
        {
            type = "Multi";
            // saves default options on and off state as a binary number, so we want them all to be 1;
            defaultOptions = (int)(Math.Pow(2, options.Count) - 1);
        }

        var modFile = new PenumbraModFile() { Name = fileName, Type = type, Options = options, Priority = 0, DefaultSettings = defaultOptions };
        WriteJSONFile(fileName, modFile);
    }
    
    private static (PenumbraModOption, PenumbraModOption?) SplitMappingByRegexInKey(PenumbraModOption originalModFile, List<string> pathsToSearch)
    {
        var falseFileMappings = originalModFile.Files.Where(x => !pathsToSearch.Any(s => x.Key.Contains(s))).ToDictionary();
        var falseFileSwapMappings = originalModFile.FileSwaps.Where(x => !pathsToSearch.Any(s => x.Key.Contains(s))).ToDictionary();

        var trueFileMappings = originalModFile.Files.Where(x => pathsToSearch.Any(s => x.Key.Contains(s))).ToDictionary();
        var trueFileSwapMappings = originalModFile.FileSwaps.Where(x => pathsToSearch.Any(s => x.Key.Contains(s))).ToDictionary();

        originalModFile.Files = falseFileMappings;
        originalModFile.FileSwaps = falseFileSwapMappings;
        PenumbraModOption? splitModFile = null;
        if(trueFileMappings.Count > 0 || trueFileSwapMappings.Count > 0)
        {
            splitModFile = new PenumbraModOption() { Name = originalModFile.Name, Files = trueFileMappings, FileSwaps = trueFileSwapMappings, Description = originalModFile.Description };
        }
        return (originalModFile, splitModFile); 
    }

    private List<PenumbraModOption> GetNormalMappings(string materialsDirectory, string? normalsDirectory = null)
    {
        List<PenumbraModOption> modOptions = [];
        if (normalsDirectory != null)
        {
            var normalsFullPath = Path.Combine(modPath, normalsDirectory);
            // Get the true normalMaps from installed normal folder becuase we don't have a new normal for every material, unlike indexes/diffuses/masks.
            var normalMappings = GetNaiveMappingsFromDirectory(normalsFullPath);

            modOptions.Add(new PenumbraModOption() { Name = Constants.vanillaNormalsOptionName, Files = normalMappings });
        }

        var materialsFullPath = Path.Combine(modPath, materialsDirectory);

        var texturePaths = GetStatueTexturesFromMaterialsDirectory(materialsFullPath);

        // Ignore hair for Smooth normals cause it does funky stuff.
        var normalSmoothMappings = texturePaths.NormalTextures.Where(x => !Constants.SmoothIgnoringSubPaths.Any(s => x.Contains(s))).ToDictionary(x => x, x => Constants.SmoothNomalsTexturePath);

        modOptions.Add(new PenumbraModOption() { Name = Constants.smoothNormalsOptionName, Files = normalSmoothMappings, Description= Constants.SmoothNormalsDescription, Priority = 1 });

        return modOptions;
    }

    private static MaterialTexturePaths GetStatueTexturesFromMaterialsDirectory(string directoryPath)
    {
        if (!Path.Exists(directoryPath))
        {
            return new MaterialTexturePaths();
        }

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
