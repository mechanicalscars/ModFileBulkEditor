namespace ModFileBulkEditor;

public class ModFileBulkEditorMain
{
    public static void Main(string[] args)
    {
        var materialInputPath = Path.Combine(Constants.InputFolder, Constants.MaterialsFolder);
        var materialOutputPath = Path.Combine(Constants.OutputFolder, Constants.MaterialsFolder);

        var dollInputPath = Path.Combine(Constants.InputFolder, Constants.DollFolder);
        var dollOutputPath = Path.Combine(Constants.OutputFolder, Constants.DollFolder);


        var foldersAndConvertors = new List<(string, MaterialConvertor?, TextureConvertor?)> {
            (Constants.StoneMarbleFolder, MaterialConvertors.turnMaterialStoneMarble, null),
            (Constants.StoneMatteFolder, MaterialConvertors.turnMaterialStoneMatte, null),
            (Constants.GoldFolder, MaterialConvertors.turnMaterialGold, null),
            (Constants.JadeFolder, MaterialConvertors.turnMaterialJade, null),
            };

        foreach ((var optionFolder, var materialConvertor, var textureConvertor) in foldersAndConvertors)
        {
            var materialOptionOutputPath = Path.Combine(materialOutputPath, optionFolder);
            var dollOptionOutputPath = Path.Combine(dollOutputPath, optionFolder);
            ConvertDirectory(materialInputPath, materialOptionOutputPath, materialConvertor, textureConvertor);
            ConvertDirectory(dollInputPath, dollOptionOutputPath, materialConvertor, textureConvertor);
        }

        List<string> copyFolders = [Constants.NormalsFolder, Constants.ModelsFolder, Constants.ScarFolder];
        foreach (var folder in copyFolders)
        {
            var inputPath = Path.Combine(Constants.InputFolder, folder);
            var outputPath = Path.Combine(Constants.OutputFolder, folder);
            CopyDirectory(inputPath, outputPath);
        }

        var metaFile = new JSONFileCreator.PenumbraMetaFile() { Name = "[Scar] Statues", Author = "Scar", Description=Constants.StatueModDescription, Version="1.6" };
        var defaultFile = new JSONFileCreator.PenumbraDefaultSubMod() { };

        JSONFileCreator.WriteJSONFile(Constants.OutputFolder, Constants.metaOutputJSONFile, metaFile);
        JSONFileCreator.WriteJSONFile(Constants.OutputFolder, Constants.defaultOutputJSONFile, defaultFile);
        JSONFileCreator.WriteFileRedirectionsToJSONFile(Constants.OutputFolder, Constants.MaterialsFolder, Constants.materialsFilesOutputJSONFile, Constants.additionalMaterialMappings);
        JSONFileCreator.WriteFileRedirectionsToJSONFile(Constants.OutputFolder, Constants.DollFolder, Constants.dollFilesOutputJSONFile, Constants.additionalDollMappings);
        JSONFileCreator.WriteStatueRequiredFiles(Constants.OutputFolder, Constants.MaterialsFolder, Constants.NormalsFolder, Constants.ModelsFolder, Constants.requiredFilesOutputJSONFile, Constants.baseTexturesOutputJSONFile);
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
