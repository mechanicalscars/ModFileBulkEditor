using Penumbra.GameData.Files;

namespace ModFileBulkEditor;

public class ModFileCreator
{
    public readonly string InputPath;
    public readonly string OutputPath;

    private readonly ShpkFile shdrPk;
    public readonly JSONFileCreator fileCreator;

    public ModFileCreator(
        string shaderPackPath,
        string inputPath,
        string outputPath
        )
    {
        InputPath = inputPath;
        OutputPath = outputPath;

        byte[] characterBytes = File.ReadAllBytes(shaderPackPath);
        shdrPk = new ShpkFile(characterBytes, true);

        fileCreator = new(outputPath, true);
    }

    public void CovertDirectories(List<(string, string, MaterialConvertor?, TextureConvertor?)> FoldersAndConvertors)
    {
        foreach ((var optionFolder, var groupFolder, var materialConvertor, var textureConvertor) in FoldersAndConvertors)
        {
            var groupInputPath = Path.Combine(InputPath, groupFolder);
            var groupOutputPath = Path.Combine(OutputPath, groupFolder);
            var optionOutputPath = Path.Combine(groupOutputPath, optionFolder);
            ConvertDirectory(groupInputPath, optionOutputPath, materialConvertor, textureConvertor);
        }
    }

    public void CopyDirectories(List<string> CopyFolders)
    {
        foreach (var folder in CopyFolders)
        {
            var copyInputPath = Path.Combine(InputPath, folder);
            var copyOutputPath = Path.Combine(OutputPath, folder);
            CopyDirectory(copyInputPath, copyOutputPath);
        }
    }

    public void WriteMetaAndDefaultFiles(Models.PenumbraMetaFile MetaFile)
    {
        fileCreator.WriteJSONFile(Constants.metaOutputJSONFile, MetaFile, false);
        fileCreator.WriteJSONFile(Constants.defaultOutputJSONFile, new Models.PenumbraDefaultSubMod() { }, false);
    }

    public void WriteFileRedirections(String folderName, Dictionary<string, string> additionaMappings)
    {
        fileCreator.WriteFileRedirectionsToJSONFile(folderName, additionaMappings);
    }

    public void ConvertDirectory(string folderPath, string newDirectoryPath, MaterialConvertor? materialConvertor = null, TextureConvertor? textureConvertor = null)
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
                materialConvertor(file, shdrPk, newFilePath);
            }
            else if (file.Name.EndsWith(".tex") && textureConvertor != null)
            {
                Directory.CreateDirectory(newDirectoryPath);
                textureConvertor(file, newFilePath);
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
