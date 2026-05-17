using Penumbra.GameData.Structs;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace ModFileBulkEditor;

public static class Constants
{
    public static readonly string OutputFolder = "E:\\FFXIVModsDT\\[Scar] Statues Test";
    //public static readonly string OutputFolder = "E:\\ScarMods\\Statue\\Output";
    public static readonly string InputFolder = "E:\\ScarMods\\Statues";

    public static readonly string ModelsFolder = "Model";
    public static readonly string MaterialsFolder = "Material";
    public static readonly string NormalsFolder = "Normal";
    public static readonly string DollFolder = "Doll Material";


    public static readonly string materialsFilesOutputJSONFile = "group_001_material.json";
    public static readonly string baseTexturesOutputJSONFile = "group_002_base texture.json";
    public static readonly string dollFilesOutputJSONFile = "group_003_doll joint materials.json";
    public static readonly string requiredFilesOutputJSONFile = "group_004_required materials.json";
    public static readonly string metaOutputJSONFile = "meta.json";
    public static readonly string defaultOutputJSONFile = "default_mod.json";

    public static readonly string JadeFolder = "Jade";
    public static readonly string GoldFolder = "Gold";
    public static readonly string StoneMarbleFolder = "Stone (Marble)";
    public static readonly string StoneMatteFolder = "Stone (Matte)";

    public static readonly string ScarFolder = "scar";

    public static readonly string ScarStoneTexturePath = "scar\\texture\\stone_lighter.tex";
    public static readonly string ScarStonex4TexturePath = "scar\\texture\\stone_lighter_x4.tex";
    public static readonly string ScarStoneDarkerTexturePath = "scar\\texture\\stone_darker.tex";
    public static readonly string ScarStoneDarkerx4TexturePath = "scar\\texture\\stone_darker_x4.tex";
    public static readonly string ScarGraniteTexturePath = "scar\\texture\\granite.tex";
    public static readonly string ScarGranitex4TexturePath = "scar\\texture\\granite_x4.tex";

    public static readonly string WhiteTexturePath = "chara\\common\\texture\\white.tex";

    public static readonly string[] X4TextureSubpaths = ["_fac_a", "hir_a", "_etc_", "_acc_"];
    public static readonly string[] AccentMaterialSubpaths = ["_etc_", "_acc_"];

    public static readonly HalfColor goldHalfColor = Utils.getHalfColorFromRGB(245, 200, 65);
    public static readonly HalfColor goldDarkerHalfColour = Utils.getHalfColorFromRGB(110, 86, 38);
    public static readonly HalfColor whiteHalfColor = Utils.getHalfColorFromRGB(255, 255, 255);
    public static readonly HalfColor whiteDarkerHalfColor = Utils.getHalfColorFromRGB(180, 180, 180);
    public static readonly HalfColor jadeHalfDiffuseColour = Utils.getHalfColorFromRGB(50, 131, 99);
    public static readonly HalfColor jadeHalfDarkerColour = Utils.getHalfColorFromRGB(3, 20, 20);
    public static readonly HalfColor jadeHalfAccentColour = Utils.getHalfColorFromRGB(196, 19, 19);
    public static readonly HalfColor jadeHalfSpecularColour = Utils.getHalfColorFromRGB(172, 254, 187);

    public static readonly string CharacterPackageName = "character.shpk";

    public static readonly uint shaderTextureModeKey = 0xB616DC5A;
    public static readonly uint shaderVertexModeKey = 0xF52CCF05;
    public static readonly uint textureModeCompatability = 1611594207;
    public static readonly uint vertexModeMulti = 2815623008;

    //static readonly uint fauxWindMultiplierKey = 1611594207;
    //static readonly uint FauxWindAmplitudeKey = 2815623008;

    public static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        IncludeFields = true,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static readonly Dictionary<string, string> additionalMaterialMappings = new() {
        { "chara/human/c0401/obj/body/b0001/material/v0001/mt_c0401b0001_a.mtrl", "chara\\human\\c0201\\obj\\body\\b0001\\material\\v0001\\mt_c0201b0001_a.mtrl" },
        { "chara/human/c1801/obj/body/b0001/material/v0001/mt_c1801b0001_a.mtrl", "chara\\human\\c0201\\obj\\body\\b0001\\material\\v0001\\mt_c0201b0001_a.mtrl" },
        { "chara/human/c0401/obj/body/b0001/material/v0001/mt_c0401b0001_b.mtrl", "chara\\human\\c0201\\obj\\body\\b0001\\material\\v0001\\mt_c0201b0001_b.mtrl" },
        { "chara/human/c1801/obj/body/b0001/material/v0001/mt_c1801b0001_b.mtrl", "chara\\human\\c0201\\obj\\body\\b0001\\material\\v0001\\mt_c0201b0001_b.mtrl" },
        { "chara/human/c0401/obj/body/b0001/material/v0001/mt_c0401b0001_bibo.mtrl", "chara\\human\\c0201\\obj\\body\\b0001\\material\\v0001\\mt_c0201b0001_bibo.mtrl" },
        { "chara/human/c1801/obj/body/b0001/material/v0001/mt_c1801b0001_bibo.mtrl", "chara\\human\\c0201\\obj\\body\\b0001\\material\\v0001\\mt_c0201b0001_bibo.mtrl" },
        { "chara/human/c0201/obj/hair/h0052/material/v0001/mt_c0201h0052_acc_b.mtrl", "chara\\human\\c0201\\obj\\hair\\h0051\\material\\v0001\\mt_c0201h0051_acc_b.mtrl" },
        { "chara/human/c0201/obj/hair/h0053/material/v0001/mt_c0201h0053_acc_b.mtrl", "chara\\human\\c0201\\obj\\hair\\h0051\\material\\v0001\\mt_c0201h0051_acc_b.mtrl" },
        { "chara/human/c0201/obj/hair/h0052/material/v0001/mt_c0201h0052_hir_a.mtrl", "chara\\human\\c0201\\obj\\hair\\h0051\\material\\v0001\\mt_c0201h0051_hir_a.mtrl" },
        { "chara/human/c0201/obj/hair/h0053/material/v0001/mt_c0201h0053_hir_a.mtrl", "chara\\human\\c0201\\obj\\hair\\h0051\\material\\v0001\\mt_c0201h0051_hir_a.mtrl" },
        { "chara/human/c0401/obj/hair/h0061/material/v0001/mt_c0401h0061_acc_b.mtrl", "chara\\human\\c0201\\obj\\hair\\h0061\\material\\v0001\\mt_c0201h0061_acc_b.mtrl" },
        { "chara/human/c0601/obj/hair/h0061/material/v0001/mt_c0601h0061_acc_b.mtrl", "chara\\human\\c0201\\obj\\hair\\h0061\\material\\v0001\\mt_c0201h0061_acc_b.mtrl" },
        { "chara/human/c1001/obj/hair/h0061/material/v0001/mt_c1001h0061_acc_b.mtrl", "chara\\human\\c0201\\obj\\hair\\h0061\\material\\v0001\\mt_c0201h0061_acc_b.mtrl" },
        { "chara/human/c1401/obj/hair/h0061/material/v0001/mt_c1401h0061_acc_b.mtrl", "chara\\human\\c0201\\obj\\hair\\h0061\\material\\v0001\\mt_c0201h0061_acc_b.mtrl" },
        { "chara/human/c1801/obj/hair/h0061/material/v0001/mt_c1801h0061_acc_b.mtrl", "chara\\human\\c0201\\obj\\hair\\h0061\\material\\v0001\\mt_c0201h0061_acc_b.mtrl" },
        { "chara/human/c0401/obj/hair/h0061/material/v0001/mt_c0401h0061_hir_a.mtrl", "chara\\human\\c0201\\obj\\hair\\h0061\\material\\v0001\\mt_c0201h0061_hir_a.mtrl" },
        { "chara/human/c0601/obj/hair/h0061/material/v0001/mt_c0601h0061_hir_a.mtrl", "chara\\human\\c0201\\obj\\hair\\h0061\\material\\v0001\\mt_c0201h0061_hir_a.mtrl" },
        { "chara/human/c1001/obj/hair/h0061/material/v0001/mt_c1001h0061_hir_a.mtrl", "chara\\human\\c0201\\obj\\hair\\h0061\\material\\v0001\\mt_c0201h0061_hir_a.mtrl" },
        { "chara/human/c1401/obj/hair/h0061/material/v0001/mt_c1401h0061_hir_a.mtrl", "chara\\human\\c0201\\obj\\hair\\h0061\\material\\v0001\\mt_c0201h0061_hir_a.mtrl" },
        { "chara/human/c1801/obj/hair/h0061/material/v0001/mt_c1801h0061_hir_a.mtrl", "chara\\human\\c0201\\obj\\hair\\h0061\\material\\v0001\\mt_c0201h0061_hir_a.mtrl" },
        { "chara/human/c0601/obj/hair/h0052/material/v0001/mt_c0601h0052_acc_b.mtrl", "chara\\human\\c0601\\obj\\hair\\h0051\\material\\v0001\\mt_c0601h0051_acc_b.mtrl" },
        { "chara/human/c0601/obj/hair/h0053/material/v0001/mt_c0601h0053_acc_b.mtrl", "chara\\human\\c0601\\obj\\hair\\h0051\\material\\v0001\\mt_c0601h0051_acc_b.mtrl" },
        { "chara/human/c0601/obj/hair/h0052/material/v0001/mt_c0601h0052_hir_a.mtrl", "chara\\human\\c0601\\obj\\hair\\h0051\\material\\v0001\\mt_c0601h0051_hir_a.mtrl" },
        { "chara/human/c0601/obj/hair/h0053/material/v0001/mt_c0601h0053_hir_a.mtrl", "chara\\human\\c0601\\obj\\hair\\h0051\\material\\v0001\\mt_c0601h0051_hir_a.mtrl" },
        { "chara/human/c0801/obj/face/f0002/material/mt_c0801f0002_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0801/obj/face/f0003/material/mt_c0801f0003_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0801/obj/face/f0004/material/mt_c0801f0004_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0801/obj/face/f0101/material/mt_c0801f0101_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0801/obj/face/f0102/material/mt_c0801f0102_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0801/obj/face/f0103/material/mt_c0801f0103_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0801/obj/face/f0104/material/mt_c0801f0104_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1801/obj/face/f0001/material/mt_c1801f0001_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1801/obj/face/f0002/material/mt_c1801f0002_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1801/obj/face/f0003/material/mt_c1801f0003_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1801/obj/face/f0004/material/mt_c1801f0004_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1801/obj/face/f0101/material/mt_c1801f0101_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1801/obj/face/f0102/material/mt_c1801f0102_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1801/obj/face/f0103/material/mt_c1801f0103_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1801/obj/face/f0104/material/mt_c1801f0104_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1401/obj/face/f0001/material/mt_c1401f0001_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1401/obj/face/f0002/material/mt_c1401f0002_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1401/obj/face/f0003/material/mt_c1401f0003_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1401/obj/face/f0004/material/mt_c1401f0004_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1401/obj/face/f0101/material/mt_c1401f0101_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1401/obj/face/f0102/material/mt_c1401f0102_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1401/obj/face/f0103/material/mt_c1401f0103_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1401/obj/face/f0104/material/mt_c1401f0104_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0201/obj/face/f0001/material/mt_c0201f0001_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0201/obj/face/f0002/material/mt_c0201f0002_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0201/obj/face/f0003/material/mt_c0201f0003_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0201/obj/face/f0004/material/mt_c0201f0004_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0401/obj/face/f0101/material/mt_c0401f0101_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0401/obj/face/f0102/material/mt_c0401f0102_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0401/obj/face/f0103/material/mt_c0401f0103_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0401/obj/face/f0104/material/mt_c0401f0104_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0601/obj/face/f0001/material/mt_c0601f0001_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0601/obj/face/f0002/material/mt_c0601f0002_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0601/obj/face/f0003/material/mt_c0601f0003_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0601/obj/face/f0004/material/mt_c0601f0004_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0601/obj/face/f0104/material/mt_c0601f0104_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0601/obj/face/f0103/material/mt_c0601f0103_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0601/obj/face/f0102/material/mt_c0601f0102_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0601/obj/face/f0101/material/mt_c0601f0101_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1001/obj/face/f0004/material/mt_c1001f0004_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1001/obj/face/f0003/material/mt_c1001f0003_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1001/obj/face/f0002/material/mt_c1001f0002_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1001/obj/face/f0001/material/mt_c1001f0001_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1001/obj/face/f0104/material/mt_c1001f0104_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1001/obj/face/f0103/material/mt_c1001f0103_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1001/obj/face/f0102/material/mt_c1001f0102_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1001/obj/face/f0101/material/mt_c1001f0101_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0201/obj/face/f0005/material/mt_c0201f0005_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1601/obj/face/f0106/material/mt_c1601f0106_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1601/obj/face/f0105/material/mt_c1601f0105_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1601/obj/face/f0107/material/mt_c1601f0107_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1601/obj/face/f0108/material/mt_c1601f0108_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1601/obj/face/f0005/material/mt_c1601f0005_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1601/obj/face/f0006/material/mt_c1601f0006_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1601/obj/face/f0007/material/mt_c1601f0007_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c1601/obj/face/f0008/material/mt_c1601f0008_iri_a.mtrl", "chara\\human\\c0801\\obj\\face\\f0001\\material\\mt_c0801f0001_iri_a.mtrl" },
        { "chara/human/c0801/obj/hair/h0052/material/v0001/mt_c0801h0053_acc_b.mtrl", "chara\\human\\c0801\\obj\\hair\\h0052\\material\\v0001\\mt_c0801h0052_acc_b.mtrl" },
        { "chara/human/c0201/obj/hair/h0115/material/v0001/mt_c0201h0115_hir_a.mtrl", "chara\\human\\c0801\\obj\\hair\\h0115\\material\\v0001\\mt_c0801h0115_hir_a.mtrl" },
        { "chara/human/c1401/obj/body/b0001/material/v0001/mt_c1401b0001_a.mtrl", "chara\\human\\c1401\\obj\\body\\b0001\\material\\v0001\\mt_c1401b0001_a.mtrl" },
        { "chara/human/c1401/obj/body/b0001/material/v0001/mt_c1401b0001_b.mtrl", "chara\\human\\c1401\\obj\\body\\b0001\\material\\v0001\\mt_c1401b0001_b.mtrl" },
        { "chara/human/c1401/obj/body/b0001/material/v0001/mt_c1401b0001_bibo.mtrl", "chara\\human\\c1401\\obj\\body\\b0001\\material\\v0001\\mt_c1401b0001_bibo.mtrl" },
        { "chara/human/c1401/obj/face/f0101/material/mt_c1401f0101_fac_c.mtrl", "chara\\human\\c1401\\obj\\face\\f0001\\material\\mt_c1401f0001_fac_c.mtrl" },
        { "chara/human/c1401/obj/face/f0102/material/mt_c1401f0102_fac_b.mtrl", "chara\\human\\c1401\\obj\\face\\f0002\\material\\mt_c1401f0002_fac_b.mtrl" },
        { "chara/human/c1401/obj/face/f0103/material/mt_c1401f0103_fac_b.mtrl", "chara\\human\\c1401\\obj\\face\\f0003\\material\\mt_c1401f0003_fac_b.mtrl" },
        { "chara/human/c1401/obj/face/f0104/material/mt_c1401f0104_fac_c.mtrl", "chara\\human\\c1401\\obj\\face\\f0004\\material\\mt_c1401f0004_fac_c.mtrl" },
        { "chara/human/c1401/obj/tail/t0101/material/v0001/mt_c1401t0101_a.mtrl", "chara\\human\\c1401\\obj\\tail\\t0001\\material\\v0001\\mt_c1401t0001_a.mtrl" },
        { "chara/human/c1401/obj/tail/t0102/material/v0001/mt_c1401t0102_a.mtrl", "chara\\human\\c1401\\obj\\tail\\t0002\\material\\v0001\\mt_c1401t0002_a.mtrl" },
        { "chara/human/c1401/obj/tail/t0103/material/v0001/mt_c1401t0103_a.mtrl", "chara\\human\\c1401\\obj\\tail\\t0003\\material\\v0001\\mt_c1401t0003_a.mtrl" },
        { "chara/human/c1401/obj/tail/t0104/material/v0001/mt_c1401t0104_a.mtrl", "chara\\human\\c1401\\obj\\tail\\t0004\\material\\v0001\\mt_c1401t0004_a.mtrl" },
        { "chara/human/c1601/obj/face/f0005/material/mt_c1601f0005_fac_a.mtrl", "chara\\human\\c1601\\obj\\face\\f0105\\material\\mt_c1601f0105_fac_a.mtrl" },
        { "chara/human/c1601/obj/face/f0006/material/mt_c1601f0006_fac_a.mtrl", "chara\\human\\c1601\\obj\\face\\f0106\\material\\mt_c1601f0106_fac_a.mtrl" },
        { "chara/human/c1601/obj/face/f0007/material/mt_c1601f0007_fac_a.mtrl", "chara\\human\\c1601\\obj\\face\\f0107\\material\\mt_c1601f0107_fac_a.mtrl" },
        { "chara/human/c1601/obj/face/f0008/material/mt_c1601f0008_fac_a.mtrl", "chara\\human\\c1601\\obj\\face\\f0108\\material\\mt_c1601f0108_fac_a.mtrl" }
     };

    public static readonly Dictionary<string, string> additionalDollMappings = new() {
        {"chara/human/c1601/obj/body/b0001/material/v0002/mt_c1601b0001_doll.mtrl", "chara\\human\\c1601\\obj\\body\\b0001\\material\\v0001\\mt_c1601b0001_DOLL.mtrl"},
        {"chara/human/c0201/obj/body/b0001/material/v0001/mt_c0201b0001_doll.mtrl", "chara\\human\\c1601\\obj\\body\\b0001\\material\\v0001\\mt_c1601b0001_DOLL.mtrl"},
        {"chara/human/c0401/obj/body/b0001/material/v0001/mt_c0401b0001_doll.mtrl", "chara\\human\\c1601\\obj\\body\\b0001\\material\\v0001\\mt_c1601b0001_DOLL.mtrl"},
        {"chara/human/c1401/obj/body/b0001/material/v0001/mt_c1401b0001_doll.mtrl", "chara\\human\\c1601\\obj\\body\\b0001\\material\\v0001\\mt_c1601b0001_DOLL.mtrl"},
        {"chara/human/c1401/obj/body/b0101/material/v0001/mt_c1401b0101_doll.mtrl", "chara\\human\\c1601\\obj\\body\\b0001\\material\\v0001\\mt_c1601b0001_DOLL.mtrl"},
        {"chara/human/c1601/obj/body/b0001/material/v0004/mt_c1601b0001_doll.mtrl", "chara\\human\\c1601\\obj\\body\\b0001\\material\\v0001\\mt_c1601b0001_DOLL.mtrl"},
        {"chara/human/c1601/obj/body/b0001/material/v0005/mt_c1601b0001_doll.mtrl", "chara\\human\\c1601\\obj\\body\\b0001\\material\\v0001\\mt_c1601b0001_DOLL.mtrl"},
        {"chara/human/c1801/obj/body/b0001/material/v0001/mt_c1801b0001_doll.mtrl", "chara\\human\\c1601\\obj\\body\\b0001\\material\\v0001\\mt_c1601b0001_DOLL.mtrl"},
    };

    public static readonly string StatueModDescription = "Statues for All! Now With DOLL2/AB/Gen3 Support!\n" +
        "Minor Bug:\n" +
        "If you run into shadows (example: Vanilla Small Clothes or Galatea Bustier), \n" +
        "that is unfortunately a side effect of swapping the shaders from skin to character. \n" +
        "To fix that, you will have to remove the vertex colours on the 3D model on a case by case basis. Might be better just to find an upscale somewhere, apologies.";
}
