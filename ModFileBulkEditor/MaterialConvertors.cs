using Penumbra.GameData.Files;
using Penumbra.GameData.Files.MaterialStructs;
using Penumbra.GameData.Structs;

namespace ModFileBulkEditor;

public delegate void MaterialConvertor(FileInfo inputFile, ShpkFile shdrPk, string outputFilePath);

public class MaterialConvertors
{
    public static void turnMaterialStoneMarble(FileInfo inputFile, ShpkFile shdrPk, string outputFilePath)
    {
        byte[] file = File.ReadAllBytes(inputFile.FullName);
        MtrlFile material = new(file);
        material = MetaDataConversion(material, shdrPk);

        IColorTable? Table = material.Table;
        if (Table is ColorTable table)
        {
            table[30].DiffuseColor = Constants.AccentMaterialSubpaths.Any(s => inputFile.Name.Contains(s)) ? Constants.whiteDarkerHalfColor : Constants.whiteHalfColor;
            table[30].SpecularColor = Constants.whiteHalfColor;
            table[30].Roughness = (Half)0.25;
            table[30].Metalness = (Half)0.20;
            table[30].SheenRate = (Half)0.80;
            table[30].SheenTintRate = (Half)0.20;
            table[30].SheenAperture = (Half)5.0;
            table[30].Scalar11 = (Half)1.0;
        }

        File.WriteAllBytes(outputFilePath, material.Write());
    }

    public static void turnMaterialStoneMatte(FileInfo inputFile, ShpkFile shdrPk, string outputFilePath)
    {
        byte[] file = File.ReadAllBytes(inputFile.FullName);
        MtrlFile material = new(file);
        material = MetaDataConversion(material, shdrPk);

        IColorTable? Table = material.Table;
        if (Table is ColorTable table)
        {
            table[30].DiffuseColor = Constants.AccentMaterialSubpaths.Any(s => inputFile.Name.Contains(s)) ? Constants.whiteDarkerHalfColor : Constants.whiteHalfColor;
            table[30].SpecularColor = Constants.whiteHalfColor;
            table[30].Roughness = (Half)0.60;
            table[30].Metalness = (Half)0.10;
            table[30].SheenRate = (Half)0.20;
            table[30].SheenTintRate = (Half)0.20;
            table[30].SheenAperture = (Half)5.0;
            table[30].Scalar11 = (Half)1.0;
        }

        File.WriteAllBytes(outputFilePath, material.Write());
    }

    public static void turnMaterialGold(FileInfo inputFile, ShpkFile shdrPk, string outputFilePath)
    {
        byte[] file = File.ReadAllBytes(inputFile.FullName);
        MtrlFile material = new(file);
        material = MetaDataConversion(material, shdrPk);

        IColorTable? Table = material.Table;
        if (Table is ColorTable table)
        {

            table[30].DiffuseColor = Constants.AccentMaterialSubpaths.Any(s => inputFile.Name.Contains(s)) ? Constants.goldDarkerHalfColour : Constants.goldHalfColor;
            table[30].SpecularColor = Constants.whiteHalfColor;
            table[30].Roughness = (Half)0.15;
            table[30].Metalness = (Half)1;
            table[30].Scalar11 = (Half)1.0;
        }

        File.WriteAllBytes(outputFilePath, material.Write());
    }

    public static void turnMaterialJade(FileInfo inputFile, ShpkFile shdrPk, string outputFilePath)
    {
        byte[] file = File.ReadAllBytes(inputFile.FullName);
        MtrlFile material = new(file);
        material = MetaDataConversion(material, shdrPk);

        IColorTable? Table = material.Table;
        if (Table is ColorTable table)
        {

            table[30].DiffuseColor = Constants.AccentMaterialSubpaths.Any(s => inputFile.Name.Contains(s)) ? Constants.jadeHalfDarkerColour : Constants.jadeHalfDiffuseColour;
            table[30].SpecularColor = Constants.jadeHalfSpecularColour;
            table[30].Roughness = (Half)0.10;
            table[30].Metalness = (Half)0.25;
            table[30].SheenRate = (Half)0.20;
            table[30].SheenTintRate = (Half)1;
            table[30].SheenAperture = (Half)5.0;
            table[30].Scalar11 = (Half)1.0;
        }

        File.WriteAllBytes(outputFilePath, material.Write());
    }

    public static void turnMaterialIce(FileInfo inputFile, ShpkFile shdrPk, string outputFilePath)
    {
        byte[] file = File.ReadAllBytes(inputFile.FullName);
        MtrlFile material = new(file);
        material = MetaDataConversion(material, shdrPk);

        IColorTable? Table = material.Table;
        if (Table is ColorTable table)
        {

            table[30].DiffuseColor = Constants.iceHalfColor;
            table[30].Roughness = (Half)0.10;
            table[30].Metalness = (Half)0.5;
            table[30].SheenRate = (Half)0.20;
            table[30].SheenTintRate = (Half)1;
            table[30].SheenAperture = (Half)5.0;
            table[30].Scalar11 = (Half)1.0;
        }

        File.WriteAllBytes(outputFilePath, material.Write());
    }


    public static void turnMaterialWhiteLatex(FileInfo inputFile, ShpkFile shdrPk, string outputFilePath)
    {
        var material = turnMaterialLatexInternal(inputFile, outputFilePath, shdrPk, Constants.whiteHalfColor, Constants.whiteHalfColor);
        File.WriteAllBytes(outputFilePath, material.Write());
    }


    public static void turnMaterialWhiteDyeableLatex(FileInfo inputFile, ShpkFile shdrPk, string outputFilePath)
    {
        var material = turnMaterialLatexInternal(inputFile, outputFilePath, shdrPk, Constants.whiteHalfColor, Constants.whiteHalfColor);
        IColorDyeTable? Table = material.DyeTable;
        if (Table == null)
        {
            Table = new ColorDyeTable();
        }

        if (Table is ColorDyeTable table)
        {
            table[30].DiffuseColor = true;
            table[30].Channel = (byte)1u;
            table[30].Template = 1100;

        }
        material.DyeTable = Table;
       File.WriteAllBytes(outputFilePath, material.Write());
    }

    private static MtrlFile turnMaterialLatexInternal(FileInfo inputFile, string outputFilePath, ShpkFile shdrPk, HalfColor diffuseColor, HalfColor specularColor)
    {
        byte[] file = File.ReadAllBytes(inputFile.FullName);
        MtrlFile material = new(file);
        material = MetaDataConversion(material, shdrPk);

        IColorTable? Table = material.Table;
        if (Table is ColorTable table)
        {
            table[30].DiffuseColor = diffuseColor;
            table[30].SpecularColor = specularColor;
            table[30].Roughness = (Half)0.00;
            table[30].Metalness = (Half)0.50;
            table[30].SheenRate = (Half)1.0;
            table[30].SheenTintRate = (Half)1;
            table[30].SheenAperture = (Half)5.0;
            table[30].Scalar11 = (Half)1.0;
        }
        return material;
    }

    private static MtrlFile MetaDataConversion(MtrlFile material, ShpkFile shdrPk)
    {
        if (!material.IsDawntrail)
        {
            material.MigrateToDawntrail();
        }
        ref var shaderFlags = ref ShaderFlags.Wrap(ref material.ShaderPackage.Flags);
        shaderFlags.EnableTransparency = true;
        shaderFlags.HideBackfaces = true;
        material.ShaderPackage.Flags = shaderFlags.Flags;
        material.ShaderPackage.Name = Constants.CharacterPackageName;
        //material.ShaderPackage.Flags.EnableTransperency
        material.GetOrAddShaderKey(Constants.shaderVertexModeKey, Constants.vertexModeMulti);
        material.GetOrAddShaderKey(Constants.shaderTextureModeKey, Constants.textureModeCompatability);


        var normalPath = Utils.GetSamplerTexturePath(material, ShpkFile.NormalSamplerId);
        // Assumes, Naively, that normals are only ever in the form x_norm.tex or x_n.tex .
        var diffusePath = normalPath.Contains("_norm.tex") ? normalPath.Replace("norm.tex", "base.tex") : normalPath.Replace("_n.tex", "_d.tex");
        var indexPath = normalPath.Contains("_norm.tex") ? normalPath.Replace("_norm.tex", "_id.tex") : normalPath.Replace("_n.tex", "_id.tex");
        var maskPath = normalPath.Contains("_norm.tex") ? normalPath.Replace("norm.tex", "mask.tex") : normalPath.Replace("_n.tex", "_m.tex");

        Utils.SetSamplerTexturePath(material, ShpkFile.DiffuseSamplerId, diffusePath);
        Utils.SetSamplerTexturePath(material, ShpkFile.IndexSamplerId, indexPath);
        Utils.SetSamplerTexturePath(material, ShpkFile.MaskSamplerId, maskPath);
        EditConstantValue(material, shdrPk, Constants.AlphaThresholdDivisorKey, Constants.AlphaThreshold);

        return material;
    }

    private static void EditConstantValue(MtrlFile material, ShpkFile shdrPk, uint constantKey, float newValue)
    {
        var constantIndex = material.FindOrAddConstant(constantKey, shdrPk);
        if (constantIndex == -1)
        {
            throw new Exception("You didn't have the right constant key for the thing you were trying to update. Not even going to make it for you because this is just that fragile. Try again.");
        }
        var constant = material.ShaderPackage.Constants[constantIndex];
        var constantValue = material.GetConstantValue<float>(constant);
        constantValue.Fill(newValue);
    }
}