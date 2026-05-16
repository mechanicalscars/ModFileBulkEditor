using Penumbra.GameData.Structs;
using System.Numerics;

namespace ModFileBulkEditor;

public static class Constants
{
    public static readonly string JSONOutputFile = "E:\\FFXIVModsDT\\[Scar] Statues\\bulkEditorOutput.json";

    public static readonly string JadeFolder = "E:\\FFXIVModsDT\\[Scar] Statues\\Material\\Jade";
    public static readonly string GoldFolder = "E:\\FFXIVModsDT\\[Scar] Statues\\Material\\Gold";
    public static readonly string StoneMarbleFolder = "E:\\FFXIVModsDT\\[Scar] Statues\\Material\\Stone (Marble)";
    public static readonly string StoneMatteFolder = "E:\\FFXIVModsDT\\[Scar] Statues\\Material\\Stone (Matte)";

    public static readonly string ScarStoneTexturePath = "scar\\textures\\stone.tex";
    public static readonly string ScarStonex4TexturePath = "scar\\textures\\stone_x4.tex";
    public static readonly string WhiteTexturePath = "scar\\textures\\white.tex";

    public static readonly string[] X4TextureSubpaths = ["_fac_a", "hir_a", "_etc_", "_acc_"];
    public static readonly string[] AccentMaterialSubpaths = ["_etc_", "_acc_"];

    public static readonly HalfColor goldHalfColor = getHalfColorFromRGB(245, 200, 65);
    public static readonly HalfColor goldDarkerHalfColour = getHalfColorFromRGB(110, 86, 38);
    public static readonly HalfColor whiteHalfColor = getHalfColorFromRGB(255, 255, 255);
    public static readonly HalfColor whiteDarkerHalfColor = getHalfColorFromRGB(180, 180, 180);
    public static readonly HalfColor jadeHalfDiffuseColour = getHalfColorFromRGB(50, 131, 99);
    public static readonly HalfColor jadeHalfDarkerColour = getHalfColorFromRGB(3, 20, 20);
    public static readonly HalfColor jadeHalfAccentColour = getHalfColorFromRGB(196, 19, 19);
    public static readonly HalfColor jadeHalfSpecularColour = getHalfColorFromRGB(172, 254, 187);

    public static readonly string CharacterPackageName = "character.shpk";

    public static readonly uint shaderTextureModeKey = 0xB616DC5A;
    public static readonly uint shaderVertexModeKey = 0xF52CCF05;
    public static readonly uint textureModeCompatability = 1611594207;
    public static readonly uint vertexModeMulti = 2815623008;

    //static readonly uint fauxWindMultiplierKey = 1611594207;
    //static readonly uint FauxWindAmplitudeKey = 2815623008;


    static HalfColor getHalfColorFromRGB(float red, float green, float blue)
    {
        var pseudoSquared = PseudoSquareRgb(new Vector3((red / 255), (green / 255), (blue / 255)));
        return (HalfColor)pseudoSquared;
    }

    /*
     * Taken from Penumbra.UI.FileEditing.Materials.MaterialEditor
     */
    internal static float PseudoSquareRgb(float x)
    => x < 0.0f ? -(x * x) : x * x;

    internal static Vector3 PseudoSquareRgb(Vector3 vec)
        => new(PseudoSquareRgb(vec.X), PseudoSquareRgb(vec.Y), PseudoSquareRgb(vec.Z));
}
