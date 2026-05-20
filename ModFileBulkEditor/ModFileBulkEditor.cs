namespace ModFileBulkEditor;

public class ModFileBulkEditorMain
{
    private static readonly List<(string, MaterialConvertor?, TextureConvertor?)> FoldersAndConvertors = [
        (Constants.StoneMarbleFolder, MaterialConvertors.turnMaterialStoneMarble, null),
        (Constants.StoneMatteFolder, MaterialConvertors.turnMaterialStoneMatte, null),
        (Constants.GoldFolder, MaterialConvertors.turnMaterialGold, null),
        (Constants.JadeFolder, MaterialConvertors.turnMaterialJade, null),
        (Constants.LatexFolder, MaterialConvertors.turnMaterialLatex, null),
    ];


    public static void Main(string[] args)
    {
        var metaFile = new Models.PenumbraMetaFile() { Name = "[Scar] Statues", Author = "Scar", Description = Constants.StatueModDescription, Version = Constants.Version };
        CreateMod(Constants.InputPath, Constants.OutputPath, Constants.MaterialsFolder, metaFile, [Constants.NormalsFolder, Constants.ModelsFolder, Constants.ScarFolder], [Constants.ModelsFolder], Constants.NormalsFolder, Constants.additionalMaterialMappings);


        var dollMetaFile = new Models.PenumbraMetaFile() { Name = "[Scar] Statues - Doll Addon", Author = "Scar", Version = Constants.Version };
        CreateMod(Constants.InputPath, Constants.DollOutputPath, Constants.DollMaterialFolder, dollMetaFile, [Constants.ScarFolder], null, null, Constants.additionalDollMappings);
    }

    private static void CreateMod(
        string inputPath, 
        string outputPath, 
        string materialsFolderName,
        Models.PenumbraMetaFile metaFile,
        List<string> copyFolders,
        List<string>? naiveMappingFolders = null,
        string? normalMapsFolderName = null,
        Dictionary<string, string>? additionalMaterialMappings = null
        )
    {
        var materialInputPath = Path.Combine(inputPath, materialsFolderName);
        var materialOutputPath = Path.Combine(outputPath, materialsFolderName);

        foreach ((var optionFolder, var materialConvertor, var textureConvertor) in FoldersAndConvertors)
        {
            var materialOptionOutputPath = Path.Combine(materialOutputPath, optionFolder);
            ConvertDirectory(materialInputPath, materialOptionOutputPath, materialConvertor, textureConvertor);
        }

        foreach (var folder in copyFolders)
        {
            var copyInputPath = Path.Combine(inputPath, folder);
            var copyOutputPath = Path.Combine(outputPath, folder);
            CopyDirectory(copyInputPath, copyOutputPath);
        }

        JSONFileCreator.WriteJSONFile(outputPath, Constants.metaOutputJSONFile, metaFile);
        JSONFileCreator.WriteJSONFile(outputPath, Constants.defaultOutputJSONFile, new Models.PenumbraDefaultSubMod() { });
        JSONFileCreator.WriteFileRedirectionsToJSONFile(outputPath, materialsFolderName, Constants.materialsFilesOutputJSONFile, additionalMaterialMappings);

        JSONFileCreator.WriteStatueRequiredFiles(outputPath, materialsFolderName, naiveMappingFolders);
        JSONFileCreator.WriteNormalMapsFile(outputPath, materialsFolderName, normalMapsFolderName);
    }

    private static void ConvertDirectory(string folderPath, string newDirectoryPath, MaterialConvertor? materialConvertor = null, TextureConvertor? textureConvertor = null)
    {
        var directory = new DirectoryInfo(folderPath);
        foreach (var childDirectory in directory.GetDirectories())
        {
            var newOutputFolderPath = Path.Combine(newDirectoryPath, childDirectory.Name);
            ConvertDirectory(childDirectory.FullName, newOutputFolderPath, materialConvertor, textureConvertor);
        }
        foreach (var file in directory.GetFiles())
        {
            var newFilePath = Path.Combine(newDirectoryPath, file.Name);
            if (file.Name.EndsWith(".mtrl") && materialConvertor != null)
            {
                Directory.CreateDirectory(newDirectoryPath);
                materialConvertor(file, newFilePath);
            }
            else if (file.Name.EndsWith(".tex") && textureConvertor != null)
            {
                Directory.CreateDirectory(newDirectoryPath);
                textureConvertor(file);
            }
        }
    }

    private static void CopyDirectory(string directoryPath, string newDirectoryPath)
    {
        var directory = new DirectoryInfo(directoryPath);
        foreach (var childDirectory in directory.GetDirectories())
        {
            var newChildDirectoryPath = Path.Combine(newDirectoryPath, childDirectory.Name);
            CopyDirectory(childDirectory.FullName, newChildDirectoryPath);
        }
        foreach (var file in directory.GetFiles())
        {
            Directory.CreateDirectory(newDirectoryPath);
            var newFilePath = Path.Combine(newDirectoryPath, file.Name);
            file.CopyTo(newFilePath, true);
        }
    }
}
