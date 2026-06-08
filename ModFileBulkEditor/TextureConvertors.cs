using System;
using Penumbra.Import.Textures;
using OtterTex;

namespace ModFileBulkEditor;

public delegate void TextureConvertor(FileInfo TextureFile, string outputFilePath);

public class TextureConvertors
{
    public static void ConvertIceTextures(FileInfo TextureFile, string outputFilePath)
    {
        byte[] bytes = File.ReadAllBytes(TextureFile.FullName);
        using var textureData = new MemoryStream(bytes);
        var image = TexFileParser.Parse(textureData);
        TextureManager.SaveTex(outputFilePath, image);
    }
}
