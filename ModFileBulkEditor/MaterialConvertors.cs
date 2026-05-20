using Penumbra.GameData.Files;
using Penumbra.GameData.Files.MaterialStructs;

namespace ModFileBulkEditor;

public delegate void MaterialConvertor(FileInfo inputFile, string outputFilePath);

public class MaterialConvertors
{
    public static void turnMaterialStoneMarble(FileInfo inputFile, string outputFilePath)
    {
        byte[] file = File.ReadAllBytes(inputFile.FullName);
        MtrlFile material = new(file);
        material = MetaDataConversion(material);

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

    public static void turnMaterialStoneMatte(FileInfo inputFile, string outputFilePath)
    {
        byte[] file = File.ReadAllBytes(inputFile.FullName);
        MtrlFile material = new(file);
        material = MetaDataConversion(material);

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

    public static void turnMaterialGold(FileInfo inputFile, string outputFilePath)
    {
        byte[] file = File.ReadAllBytes(inputFile.FullName);
        MtrlFile material = new(file);
        material = MetaDataConversion(material);

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

    public static void turnMaterialJade(FileInfo inputFile, string outputFilePath)
    {
        byte[] file = File.ReadAllBytes(inputFile.FullName);
        MtrlFile material = new(file);
        material = MetaDataConversion(material);

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

    public static void turnMaterialLatex(FileInfo inputFile, string outputFilePath)
    {
        byte[] file = File.ReadAllBytes(inputFile.FullName);
        MtrlFile material = new(file);
        material = MetaDataConversion(material);

        IColorTable? Table = material.Table;
        if (Table is ColorTable table)
        {

            table[30].DiffuseColor = Constants.whiteHalfColor;
            table[30].SpecularColor = Constants.whiteHalfColor;
            table[30].Roughness = (Half)0.00;
            table[30].Metalness = (Half)0.50;
            table[30].SheenRate = (Half)1.0;
            table[30].SheenTintRate = (Half)1;
            table[30].SheenAperture = (Half)5.0;
            table[30].Scalar11 = (Half)1.0;
        }

        File.WriteAllBytes(outputFilePath, material.Write());
    }


    private static MtrlFile MetaDataConversion(MtrlFile material)
    {
        if (!material.IsDawntrail)
        {
            material.MigrateToDawntrail();
        }
        material.ShaderPackage.Name = Constants.CharacterPackageName;
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

        return material;
    }
}