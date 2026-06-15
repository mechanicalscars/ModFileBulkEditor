using Penumbra.Import.Textures;
using OtterTex;
using System.Numerics;

namespace ModFileBulkEditor;

public delegate void TextureConvertor(FileInfo TextureFile, string outputFilePath);

public class TextureConvertors
{
    // Matrixes are of the form BGRA, *not* Penubmra UI's RGBA
    private static readonly Matrix4x4 seventyFivePercentBlue = new Matrix4x4(0.75f, 0.0f, 0.0f, 0.0f,
                                                                             0.0f, 1.0f, 0.0f, 0.0f,
                                                                             0.0f, 0.0f, 1.0f, 0.0f,
                                                                             0.0f, 0.0f, 0.0f, 1.0f);
    private static readonly Matrix4x4 alphaToBlue =            new Matrix4x4(0.0f, 0.0f, 0.0f, 0.0f,
                                                                             0.0f, 1.0f, 0.0f, 0.0f,
                                                                             0.0f, 0.0f, 1.0f, 0.0f,
                                                                             1.0f, 0.0f, 0.0f, 1.0f);

    public static void ConvertIceTextures(FileInfo TextureFile, string outputFilePath)
    {
        byte[] bytes = File.ReadAllBytes(TextureFile.FullName);
        using var textureData = new MemoryStream(bytes);
        ScratchImage oldScratch = TexFileParser.Parse(textureData);
        var bgra = ChangePixelsByMatrix(oldScratch.Pixels, alphaToBlue);
        bgra = ChangePixelsByMatrix(bgra, seventyFivePercentBlue);
        ScratchImage newScratch = ScratchImage.FromRGBA(bgra, oldScratch.Images[0].Width, oldScratch.Images[0].Height);
        newScratch.OverrideFormat(DXGIFormat.B8G8R8A8UNorm);
        TextureManager.SaveTex(outputFilePath, newScratch);
    }

    private static ReadOnlySpan<byte> ChangePixelsByMatrix(ReadOnlySpan<byte> bgra, Matrix4x4 matrix)
    {
        byte[] newBGRA = new byte[bgra.Length];
        for (int i = 0; i < newBGRA.Length; i += 4)
        {
            newBGRA[i]   = (byte)(bgra[i]*matrix.M11 + bgra[i+1]*matrix.M21 + bgra[i+2]*matrix.M31 + bgra[i+3]*matrix.M41);                                                 
            newBGRA[i+1] = (byte)(bgra[i]*matrix.M12 + bgra[i+1]*matrix.M22 + bgra[i+2]*matrix.M32 + bgra[i+3]*matrix.M42);                                                
            newBGRA[i+2] = (byte)(bgra[i]*matrix.M13 + bgra[i+1]*matrix.M23 + bgra[i+2]*matrix.M33 + bgra[i+3]*matrix.M43);                                           
            newBGRA[i+3] = (byte)(bgra[i]*matrix.M14 + bgra[i+1]*matrix.M24 + bgra[i+2]*matrix.M34 + bgra[i+3]*matrix.M44);
        }
        return new ReadOnlySpan<byte>(newBGRA);
    }
}
