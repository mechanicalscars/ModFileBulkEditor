using Penumbra.GameData.Files;
using Penumbra.GameData.Files.MaterialStructs;

namespace ModFileBulkEditor;

public delegate MtrlFile MaterialConvertor(MtrlFile material, string MaterialPath);

public class MaterialConvertors
{
    public static MtrlFile turnMaterialStoneMarble(MtrlFile material, string materialPath)
    {
        IColorTable? Table = material.Table;
        if (Table is ColorTable table)
        {
            table[30].DiffuseColor = (Constants.AccentMaterialSubpaths.Any(s => materialPath.Contains(s))) ? Constants.whiteDarkerHalfColor : Constants.whiteHalfColor;
            table[30].SpecularColor = Constants.whiteHalfColor;
            table[30].Roughness = (Half)0.25;
            table[30].Metalness = (Half)0.20;
            table[30].SheenRate = (Half)0.80;
            table[30].SheenTintRate = (Half)0.20;
            table[30].SheenAperture = (Half)5.0;
            table[30].Scalar11 = (Half)1.0;
        }
        return material;
    }

    public static MtrlFile turnMaterialStoneMatte(MtrlFile material, string materialPath)
    {
        IColorTable? Table = material.Table;
        if (Table is ColorTable table)
        {
            table[30].DiffuseColor = (Constants.AccentMaterialSubpaths.Any(s => materialPath.Contains(s))) ? Constants.whiteDarkerHalfColor : Constants.whiteHalfColor;
            table[30].SpecularColor = Constants.whiteHalfColor;
            table[30].Roughness = (Half)0.60;
            table[30].Metalness = (Half)0.10;
            table[30].SheenRate = (Half)0.20;
            table[30].SheenTintRate = (Half)0.20;
            table[30].SheenAperture = (Half)5.0;
            table[30].Scalar11 = (Half)1.0;
        }
        return material;
    }

    public static MtrlFile turnMaterialGold(MtrlFile material, string materialPath)
    {
        IColorTable? Table = material.Table;
        if (Table is ColorTable table)
        {

            table[30].DiffuseColor = (Constants.AccentMaterialSubpaths.Any(s => materialPath.Contains(s))) ? Constants.goldDarkerHalfColour : Constants.goldHalfColor;
            table[30].SpecularColor = Constants.whiteHalfColor;
            table[30].Roughness = (Half)0.15;
            table[30].Metalness = (Half)1;
        }
        return material;
    }

    public static MtrlFile turnMaterialJade(MtrlFile material, string materialPath)
    {
        IColorTable? Table = material.Table;
        if (Table is ColorTable table)
        {
            var diffuseColour = materialPath switch
            {
                string x when x.Contains("_etc_") => Constants.jadeHalfDarkerColour,
                string x when x.Contains("_acc_") => Constants.jadeHalfAccentColour,
                _ => Constants.jadeHalfDiffuseColour
            };
            table[30].DiffuseColor = diffuseColour;
            table[30].SpecularColor = Constants.jadeHalfSpecularColour;
            table[30].Roughness = (Half)0.10;
            table[30].Metalness = (Half)0.25;
            table[30].SheenRate = (Half)0.20;
            table[30].SheenTintRate = (Half)1;
            table[30].SheenAperture = (Half)5.0;
        }
        return material;
    }

    private static void SetShaderTexture(MtrlFile material, uint samplerID, string texture_path)
    {
        var samplerIndex = material.FindOrAddSampler(samplerID, texture_path);
        if (samplerIndex == -1)
        {
            throw new Exception("Newly Created/found sampler has index of -1. What?");
        }

        var textureIndex = material.ShaderPackage.Samplers[samplerIndex].TextureIndex;

        if (textureIndex < 0)
        {
            throw new Exception("Newly Created/found sampler texture has index of -1. What?");
        }

        material.Textures[textureIndex].Path = texture_path;
    }
}