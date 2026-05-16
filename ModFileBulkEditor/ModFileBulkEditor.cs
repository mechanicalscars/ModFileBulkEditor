using Penumbra.GameData.Files;

namespace ModFileBulkEditor;

public class ModFileBulkEditorMain
{
    public static void Main(string[] args)
    {
        convertDirectory(Constants.StoneMarbleFolder, MaterialConvertors.turnMaterialStoneMarble);
        convertDirectory(Constants.StoneMatteFolder, MaterialConvertors.turnMaterialStoneMatte);
        convertDirectory(Constants.GoldFolder, MaterialConvertors.turnMaterialGold);
        convertDirectory(Constants.JadeFolder, MaterialConvertors.turnMaterialJade);
        //var mappings = getMappingsFromFolder(StoneMarbleFolder);
        //var idMappings = stoneMarbleMapping.Where(x => x.Key.Contains("_id.tex")).ToDictionary();
        //var baseMappings = stoneMarbleMapping.Where(x => x.Key.Contains("_base.tex") || x.Key.Contains("_b.tex")).ToDictionary();
        //var maskMappings = stoneMarbleMapping.Where(x => x.Key.Contains("_mask.tex") || x.Key.Contains("_m.tex")).ToDictionary();
        //var jsonMapping = JsonSerializer.Serialize(new Dictionary<string, Dictionary<string, string>>{ { "id", idMappings}, {"base", baseMappings }, {"mask", maskMappings} });
        //File.WriteAllText(JSONOutputFile, jsonMapping);
    }

    static void convertDirectory(string folderPath, MaterialConvertor? materialConvertor = null, TextureConvertor? textureConvertor = null)
    {
        string[] childDirectories = Directory.GetDirectories(folderPath);
        string[] childFiles = Directory.GetFiles(folderPath);
        foreach (string directory in childDirectories)
        {
            convertDirectory(directory, materialConvertor, textureConvertor);
        }
        foreach (string file in childFiles)
        {
            if (File.Exists(file))
            {
                if (file.EndsWith(".mtrl") && materialConvertor != null)
                {
                    ConvertMaterial(file, materialConvertor);
                }
                else if(file.EndsWith(".tex") && textureConvertor != null)
                {

                }
            }
        }
    }

    static void ConvertMaterial(string materialPath, MaterialConvertor materialConvertor)
    {
        byte[] file = File.ReadAllBytes(materialPath);
        MtrlFile material = new(file);

        if (!material.IsDawntrail)
        {
            material.MigrateToDawntrail();
        }
        material.ShaderPackage.Name = Constants.CharacterPackageName;
        material.GetOrAddShaderKey(Constants.shaderVertexModeKey, Constants.vertexModeMulti);
        material.GetOrAddShaderKey(Constants.shaderTextureModeKey, Constants.textureModeCompatability);;

        material = materialConvertor(material, materialPath);
        File.WriteAllBytes(materialPath, material.Write());
    }

    static Dictionary<string, string> GetMappingsFromFolder(string folderPath)
    {
        Dictionary<string, string> textureMappings = [];
        string[] childDirectories = Directory.GetDirectories(folderPath);
        string[] childFiles = Directory.GetFiles(folderPath);
        foreach (string directory in childDirectories)
        {
            var directoryMappings = GetMappingsFromFolder(directory);
            foreach (var mapping in directoryMappings)
            {
                textureMappings[mapping.Key] = mapping.Value;
            }
        }
        foreach (string file in childFiles)
        {
            if (File.Exists(file) && file.EndsWith(".mtrl"))
            {
                var fileMappings = GetMaterialMappings(file);
                foreach (var mapping in fileMappings)
                {
                    textureMappings[mapping.Key] = mapping.Value;
                }
            }
        }
        return textureMappings;
    }

    static Dictionary<string, string> GetMaterialMappings(string materialPath)
    {
        byte[] file = File.ReadAllBytes(materialPath);
        MtrlFile material = new(file);

        var normalPath = GetNormalPath(material);

        // Assumes, niavely, that normals are only ever in the form x_norm.tex or x_n.tex .
        var diffusePath = normalPath.Contains("_norm.tex") ? normalPath.Replace("norm.tex", "base.tex") : normalPath.Replace("_n.tex", "_b.tex");
        var indexPath = normalPath.Contains("_norm.tex") ? normalPath.Replace("_norm.tex", "_id.tex") : normalPath.Replace("_n.tex", "_id.tex");
        var maskPath = normalPath.Contains("_norm.tex") ? normalPath.Replace("norm.tex", "mask.tex") : normalPath.Replace("_n.tex", "_m.tex");
        var diffuseTexturePath = Constants.X4TextureSubpaths.Any(s => materialPath.Contains(s)) ? Constants.ScarStonex4TexturePath : Constants.ScarStoneTexturePath;

        return new Dictionary<string,string>{ { diffusePath, diffuseTexturePath},
                { indexPath, Constants.WhiteTexturePath},
                { maskPath, Constants.WhiteTexturePath} };
    }

    private static string GetNormalPath(MtrlFile material)
    {
        var samplerIndex = material.FindSampler(ShpkFile.NormalSamplerId);
        if (samplerIndex == -1)
        {
            throw new Exception("Could not find Normal map sampler ID.");
        }
        var textureIndex = material.ShaderPackage.Samplers[samplerIndex].TextureIndex;

        if (textureIndex < 0)
        {
            throw new Exception("Newly found sampler texture has index of -1. What?");
        }

        return material.Textures[textureIndex].Path;
    }
}
