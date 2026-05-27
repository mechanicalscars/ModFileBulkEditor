namespace ModFileBulkEditor.Models
{
    public struct PenumbraMetaFile
    {
        public int FileVersion = 3;
        public string Name = "";
        public string Author = "";
        public string Description = "";
        public string Image = "";
        public string Version = "";
        public string Website = "";

        public PenumbraMetaFile() { }
    }

    public struct PenumbraDefaultSubMod
    {
        public int Version = 0;
        public Dictionary<string, string>? Files;
        public Dictionary<string, string>? FileSwaps;

        public PenumbraDefaultSubMod() { }
    }

    public struct PenumbraModFile
    {
        public int Version = 0;
        public string Name = "";
        public string Description = "";
        public string Image = "";
        public int Page = 0;
        public int Priority = 0;
        public string Type = "Single";
        public int DefaultSettings = 0;
        public List<PenumbraModOption> Options = [];

        public PenumbraModFile() { }
    }

    public struct PenumbraModOption
    {
        public string Name = "";
        public string Description = "";
        public Dictionary<string, string>? Files;
        public Dictionary<string, string>? FileSwaps;
        public int? Priority;

        public PenumbraModOption() { }
    }

    public struct MaterialTexturePaths
    {
        public HashSet<string> NormalTextures = [];
        public HashSet<string> DiffuseTextures = [];
        public HashSet<string> MaskTextures = [];
        public HashSet<string> IndexTextures = [];

        public MaterialTexturePaths() { }

        public readonly void UnionWith(MaterialTexturePaths paths)
        {
            NormalTextures.UnionWith(paths.NormalTextures);
            DiffuseTextures.UnionWith(paths.DiffuseTextures);
            MaskTextures.UnionWith(paths.MaskTextures);
            IndexTextures.UnionWith(paths.IndexTextures);
        }
    }
}
