using System.Numerics;
using Penumbra.GameData.Files;
using Penumbra.GameData.Structs;

namespace ModFileBulkEditor;

public static class Utils
{
    public static string GetSamplerTexturePath(MtrlFile material, uint samplerID)
    {
        var samplerIndex = material.FindSampler(samplerID);
        if (samplerIndex == -1)
        {
            throw new Exception($"Could not find sampler with ID {samplerID}.");
        }
        var textureIndex = material.ShaderPackage.Samplers[samplerIndex].TextureIndex;

        if (textureIndex < 0)
        {
            throw new Exception("Newly found sampler texture has index of -1. What?");
        }

        return material.Textures[textureIndex].Path;
    }

    public static void SetSamplerTexturePath(MtrlFile material, uint samplerID, string path)
    {
        var samplerIndex = material.FindOrAddSampler(samplerID, path);
        if (samplerIndex == -1)
        {
            throw new Exception($"Could not find or make sampler with ID {samplerID}.");
        }
        var textureIndex = material.ShaderPackage.Samplers[samplerIndex].TextureIndex;

        if (textureIndex < 0)
        {
            throw new Exception("Newly found sampler texture has index of -1. What?");
        }

        material.Textures[textureIndex].Path = path;
    }

    public static HalfColor getHalfColorFromRGB(float red, float green, float blue)
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
