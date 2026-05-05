using Penumbra.GameData.Files;
using Penumbra.GameData.Files.MaterialStructs;
using Penumbra.GameData.Structs;


public class MaterialBulkEditor
{
    static readonly string GoldFolder = "E:\\FFXIVModsDT\\[Scar] Statues\\Material\\Gold";
    static readonly string StoneMarbleFolder = "E:\\FFXIVModsDT\\[Scar] Statues\\Material\\Stone (Marble)";

    static readonly string scar_stone_texture_path = "scar/textures/stone.tex";
    static readonly string scar_stone_texture_x4_path = "scar/textures/stone_x4.tex";
    static readonly string white_texture_path = "chara/common/texture/white.tex";

    static readonly string[] texture_subpaths_zoomed = ["_fac_a", "hir_a", "_etc_", "_acc_"];
    static readonly string[] material_subpaths_accents = ["_etc_", "_acc_"];

    static readonly HalfColor goldHalfColor = getHalfColorFromRGB(245, 200, 65);
    static readonly HalfColor goldDarkerHalfColour = getHalfColorFromRGB(110, 86, 38);
    static readonly HalfColor whiteHalfColor = getHalfColorFromRGB(255,255,255);
    static readonly HalfColor whiteDarkerHalfColor = getHalfColorFromRGB(180, 180, 180);
    static readonly HalfColor jadeHalfColour = getHalfColorFromRGB(255, 255, 255);

    static readonly string character_package_name = "character.shpk";

    static readonly uint shaderTextureModeKey = 0xB616DC5A;
    static readonly uint shaderVertexModeKey = 0xF52CCF05;

    static readonly uint textureModeCompatability = 1611594207;
    static readonly uint vertexModeMulti = 2815623008;

    static HalfColor getHalfColorFromRGB(int red, int green, int blue)
    {
        /*
         * Translate three 0-255 ints into thre % based Half's for a half colour
         */
        return new HalfColor((Half)(red / 255.0), (Half)(green / 255.0), (Half)(blue / 255.0));
    }

    private delegate MtrlFile MaterialConvertor(MtrlFile material, string MaterialPath);

    public static void Main(string[] args)
    {
        //convertDirectory(GoldFolder, turnMaterialGold);
        convertDirectory(StoneMarbleFolder, turnMaterialStoneMarbleWhite);
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

    static MtrlFile turnMaterialGold(MtrlFile material, string materialPath)
    {
        IColorTable? Table = material.Table;
        if (Table is ColorTable table)
        {
            if (materialPath.Contains("_etc_") || materialPath.Contains("_acc_"))
            {
                table[30].DiffuseColor = goldDarkerHalfColour;
            }
            else
            {
                table[30].DiffuseColor = goldHalfColor;
            }
            table[30].DiffuseColor = goldHalfColor;
            table[30].SpecularColor = whiteHalfColor;
            table[30].Roughness = (Half)0.15;
            table[30].Metalness = (Half)1;
        }
        return material;
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
            if (material_subpaths_accents.Any(s => materialPath.Contains(s)))
            {
                table[30].DiffuseColor = whiteDarkerHalfColor;
            }
            else
            {
                table[30].DiffuseColor = whiteHalfColor;
            }
            table[30].SpecularColor = whiteHalfColor;
            table[30].Roughness = (Half)0.25;
            table[30].Metalness = (Half)0.10;
            table[30].SheenRate = (Half)0.80;
            table[30].SheenTintRate = (Half)0.20;
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
