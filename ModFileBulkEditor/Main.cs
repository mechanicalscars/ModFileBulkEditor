namespace ModFileBulkEditor;

public class ModFileBulkEditorMain
{
    private static readonly List<(string, string, MaterialConvertor?, TextureConvertor?)> FoldersAndConvertors = [
        (Constants.StoneMarbleFolder, Constants.MaterialsFolder, MaterialConvertors.TurnMaterialStoneMarble, null),
        (Constants.StoneMatteFolder, Constants.MaterialsFolder, MaterialConvertors.TurnMaterialStoneMatte, null),
        (Constants.GoldFolder, Constants.MaterialsFolder, MaterialConvertors.TurnMaterialGold, null),
        (Constants.JadeFolder, Constants.MaterialsFolder, MaterialConvertors.TurnMaterialJade, null),
        (Constants.LatexFolder,Constants.MaterialsFolder, MaterialConvertors.TurnMaterialWhiteLatex, null),
        (Constants.LatexDyeableFolder, Constants.MaterialsFolder, MaterialConvertors.TurnMaterialWhiteDyeableLatex, null),
    ];

    private static readonly List<(string, string, MaterialConvertor?, TextureConvertor?)> SeeThroughFolders = [
        (Constants.IceFolder, Constants.MaterialsFolder, MaterialConvertors.TurnMaterialIce, null),
        (Constants.HologramFolder, Constants.MaterialsFolder, MaterialConvertors.TurnMaterialHologram, null)
    ];

    public static void Main(string[] args)
    {
        var metaFile = new Models.PenumbraMetaFile() { Name = "[Scar] Statues", Author = "Scar", Description = Constants.StatueModDescription, Version = Constants.Version };
        CreateMod(metaFile, Constants.InputPath, Constants.OutputPath, Constants.MaterialsFolder, [Constants.NormalsFolder, Constants.ModelsFolder, Constants.ScarFolder], Constants.additionalMaterialMappings, Constants.NormalsFolder, [Constants.ModelsFolder]);

        var dollMetaFile = new Models.PenumbraMetaFile() { Name = "[Scar] Statues - Doll Addon", Author = "Scar", Version = Constants.Version };
        CreateMod(dollMetaFile, Constants.InputPath, Constants.DollOutputPath, Constants.DollMaterialFolder, [Constants.ScarFolder], Constants.additionalDollMappings);

        var iceMetaFile = new Models.PenumbraMetaFile() { Name = "[Scar] Statues - Ice Addon", Author = "Scar", Version = Constants.Version };
        CreateIceMod(iceMetaFile, Constants.InputPath, Constants.IceOutputPath, Constants.MaterialsFolder, Constants.NormalsFolder, Constants.additionalMaterialMappings, Constants.additionalNormalIceMappings);
    }

    private static void CreateMod(
        Models.PenumbraMetaFile metaFile,
        string InputPath,
        string OutputPath,
        string MaterialFolder, 
        List<string> copyFolders, 
        Dictionary<string, string> additionalMaterialMappings,
        string? normalMapsFolderName = null,
        List<String>? naiveMappingFolders = null
        )
    {
        var modCreator = new ModFileCreator(Constants.CharacterShaderPath,
            InputPath,
            OutputPath);

        modCreator.CovertDirectories(FoldersAndConvertors);
        modCreator.CopyDirectories(copyFolders);
        modCreator.WriteMetaAndDefaultFiles(metaFile);
        modCreator.WriteFileRedirections(MaterialFolder, additionalMaterialMappings);

        modCreator.fileCreator.WriteStatueRequiredFiles(MaterialFolder, normalMapsFolderName, naiveMappingFolders);
    }

    private static void CreateIceMod(
        Models.PenumbraMetaFile metaFile,
        string InputPath,
        string outputPath,
        string materialsFolderName,
        string normalMapsFolderName,
        Dictionary<string, string> additionalMaterialMappings,
        Dictionary<string, string> additionalNormalMappings
        )
    {
        var modCreator = new ModFileCreator(Constants.CharacterShaderPath,
            InputPath,
            outputPath);

        modCreator.CovertDirectories(SeeThroughFolders);

        var normalsInputPath = Path.Combine(InputPath, normalMapsFolderName);
        var normalsOutputPath = Path.Combine(outputPath, normalMapsFolderName);
        var normalsAdditionalInputPath = Path.Combine(InputPath, Constants.AdditionalNormalsFolder);

        var normalsOptionOutputPath = Path.Combine(normalsOutputPath, "Ice Normals");

        modCreator.ConvertDirectory(normalsInputPath, normalsOptionOutputPath, null, TextureConvertors.ConvertIceTextures);
        modCreator.ConvertDirectory(normalsAdditionalInputPath, normalsOptionOutputPath, null, TextureConvertors.ConvertIceTextures);

        modCreator.WriteMetaAndDefaultFiles(metaFile);
        modCreator.WriteFileRedirections(materialsFolderName, additionalMaterialMappings);
        modCreator.WriteFileRedirections(normalMapsFolderName, additionalNormalMappings);
    }
}
