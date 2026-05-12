using Penumbra.GameData.Files;
using Penumbra.GameData.Files.MaterialStructs;
using Penumbra.GameData.Structs;
using System.Numerics;


public class MaterialBulkEditor
{
    static readonly string JadeFolder = "E:\\FFXIVModsDT\\[Scar] Statues\\Material\\Jade";
    static readonly string GoldFolder = "E:\\FFXIVModsDT\\[Scar] Statues\\Material\\Gold";
    static readonly string StoneMarbleFolder = "E:\\FFXIVModsDT\\[Scar] Statues\\Material\\Stone (Marble)";

    static readonly string scar_stone_texture_path = "scar/textures/stone.tex";
    static readonly string scar_stone_texture_x4_path = "scar/textures/stone_x4.tex";
    static readonly string white_texture_path = "chara/common/texture/white.tex";

    static readonly string[] texture_subpaths_zoomed = ["_fac_a", "hir_a", "_etc_", "_acc_"];
    static readonly string[] material_subpaths_accents = ["_etc_", "_acc_"];

    static readonly HalfColor goldHalfColor = getHalfColorFromRGB(245, 200, 65);
    static readonly HalfColor goldDarkerHalfColour = getHalfColorFromRGB(110, 86, 38);
    static readonly HalfColor whiteHalfColor = getHalfColorFromRGB(255, 255, 255);
    static readonly HalfColor whiteDarkerHalfColor = getHalfColorFromRGB(180, 180, 180);

    static readonly HalfColor jadeHalfDiffuseColour = getHalfColorFromRGB(10, 53, 40);
    static readonly HalfColor jadeHalfDarkerColour = getHalfColorFromRGB(3, 20, 20);
    static readonly HalfColor jadeHalfAccentColour = getHalfColorFromRGB(196, 19, 19);
    static readonly HalfColor jadeHalfSpecularColour = getHalfColorFromRGB(172, 254, 187);

    static readonly string character_package_name = "character.shpk";

    static readonly uint shaderTextureModeKey = 0xB616DC5A;
    static readonly uint shaderVertexModeKey = 0xF52CCF05;

    static readonly uint textureModeCompatability = 1611594207;
    static readonly uint vertexModeMulti = 2815623008;

    static readonly uint fauxWindMultiplierKey = 1611594207;
    static readonly uint FauxWindAmplitudeKey = 2815623008;


    static HalfColor getHalfColorFromRGB(float red, float green, float blue)
    {
        /*
         * Translate three 0-255 floats into three % based Half's for a half colour.
         * Fun note: this doesn't actually work. Gotta figure out how to actually get 1:1 RGB valeus here...
         */
        var pseudoSquared = PseudoSquareRgb(new Vector3((red / 255), (green / 255), (blue / 255)));
        return (HalfColor)pseudoSquared;
    }

    private delegate MtrlFile MaterialConvertor(MtrlFile material, string MaterialPath);

    public static void Main(string[] args)
    {
        // convertDirectory(StoneMarbleFolder, turnMaterialStoneMarbleWhite);
        //convertDirectory(GoldFolder, turnMaterialGold);
        convertDirectory(JadeFolder, turnMaterialJade);
    }

    static void convertDirectory(string folderPath, MaterialConvertor materiealConvertor)
    {
        string[] childDirectories = Directory.GetDirectories(folderPath);
        string[] childFiles = Directory.GetFiles(folderPath);
        foreach (string directory in childDirectories)
        {
            convertDirectory(directory, materiealConvertor);
        }
        foreach (string file in childFiles)
        {
            if (File.Exists(file) && file.EndsWith(".mtrl"))
            {
                convertMaterial(file, materiealConvertor);
            }
        }
    }

    static void convertMaterial(string materialPath, MaterialConvertor materiealConvertor)
    {
        byte[] file = File.ReadAllBytes(materialPath);
        MtrlFile material = new(file);
        material = materiealConvertor(material, materialPath);
        File.WriteAllBytes(materialPath, material.Write());
    }
    static MtrlFile turnMaterialStoneMarbleWhite(MtrlFile material, string materialPath)
    {
        if (!material.IsDawntrail)
        {
            material.MigrateToDawntrail();
        }
        material.ShaderPackage.Name = character_package_name;
        material.GetOrAddShaderKey(shaderVertexModeKey, vertexModeMulti);
        material.GetOrAddShaderKey(shaderTextureModeKey, textureModeCompatability);

        var diffuse_texture_path = texture_subpaths_zoomed.Any(s => materialPath.Contains(s)) ? scar_stone_texture_x4_path : scar_stone_texture_path;
        SetShaderTexture(material, ShpkFile.DiffuseSamplerId, diffuse_texture_path);
        SetShaderTexture(material, ShpkFile.IndexSamplerId, white_texture_path);
        SetShaderTexture(material, ShpkFile.MaskSamplerId, white_texture_path);

        IColorTable? Table = material.Table;
        if (Table is ColorTable table)
        {
            table[30].DiffuseColor = (material_subpaths_accents.Any(s => materialPath.Contains(s))) ? whiteDarkerHalfColor : whiteHalfColor;
            table[30].SpecularColor = whiteHalfColor;
            table[30].Roughness = (Half)0.25;
            table[30].Metalness = (Half)0.10;
            table[30].SheenRate = (Half)0.80;
            table[30].SheenTintRate = (Half)0.20;
            table[30].SheenAperture = (Half)5.0;
            table[30].Scalar11 = (Half)1.0;
        }
        return material;
    }

    static MtrlFile turnMaterialGold(MtrlFile material, string materialPath)
    {
        //EditConstantValue(material, FauxWindAmplitudeKey, 0);
        //EditConstantValue(material, fauxWindMultiplierKey, 0);
        IColorTable? Table = material.Table;
        if (Table is ColorTable table)
        {

            table[30].DiffuseColor = (material_subpaths_accents.Any(s => materialPath.Contains(s))) ? goldDarkerHalfColour : goldHalfColor;
            table[30].SpecularColor = whiteHalfColor;
            table[30].Roughness = (Half)0.15;
            table[30].Metalness = (Half)1;
        }
        return material;
    }

    static MtrlFile turnMaterialJade(MtrlFile material, string materialPath)
    {
        IColorTable? Table = material.Table;
        if (Table is ColorTable table)
        {
            var diffuseColour = materialPath switch
            {
                string x when x.Contains("_etc_") => jadeHalfDarkerColour,
                string x when x.Contains("_acc_") => jadeHalfAccentColour,
                _ => jadeHalfDiffuseColour
            };
            table[30].DiffuseColor = diffuseColour;
            table[30].SpecularColor = jadeHalfSpecularColour;
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

    /*
     * WARNING WARNING WARNING DANGER WILL ROBINSON
     * The constants in the material file are stored as a long byte array, 
     * and Penumbra does NOT have the functions in it to pull one out and politely update it.
     * We have to slice into that byte array and change the values in it, and carefully place them back.
     * This means this function is horrifically unsafe for use and if you are going to use it:
     * Dear GODS please make sure you have back ups of the material files you're almost certainly about to break.
     */
    private static void EditConstantValue(MtrlFile material, uint constantKey, float newValue)
    {
        // Game has to be running to get the shader pack? figure out a work around I guess.
        //var constantIndex = material.FindOrAddConstant(constantKey);
        //if (constantIndex == -1)
        //{
        //    throw new Exception("You didn't have the right constant key for the thing you were trying to update. Not even going to make it for you because this is just that fragile. Try again.");
        //}
        //var constant = material.ShaderPackage.Constants[constantIndex];
        //var constantValue = material.GetConstantValue<float>(constant);
        //constantValue.Fill(newValue);
    }

    /*
     * Taken from Penumbra.UI.FileEditing.Materials.MaterialEditor
     */
    internal static float PseudoSquareRgb(float x)
    => x < 0.0f ? -(x * x) : x * x;

    internal static Vector3 PseudoSquareRgb(Vector3 vec)
        => new(PseudoSquareRgb(vec.X), PseudoSquareRgb(vec.Y), PseudoSquareRgb(vec.Z));
}
