using System.IO;
using ColossalFramework.Plugins;
using UnityEngine;

namespace MoreFlags
{
    public struct Flag
    {
        private Texture2D m_texture;
        private Texture2D m_textureLod;
        private Texture2D m_thumb;
        private Texture2D m_thumbWall;

        public string id;
        public string description;
        public string extendedDescripton;

        public Texture2D texture => m_texture ??
                                    (m_texture =
                                        id.Contains(".")
                                            ? Util.LoadTextureFromFile(Path.Combine(plugin.modPath, $"flag_{id}.png"), true)
                                            : InitializeTexture($"MoreFlags.flags.flag_{id}.png"));

        public Texture2D textureLod => m_textureLod ??
                                    (m_textureLod =
                                        id.Contains(".")
                                            ? Util.LoadTextureFromFile(Path.Combine(plugin.modPath, $"flag_{id}_lod.png"), false)
                                            : InitializeTexture($"MoreFlags.flags.flag_{id}_lod.png"));
        public Texture2D thumb => m_thumb ??
                                    (m_thumb =
                                        id.Contains(".")
                                            ? Util.LoadTextureFromFile(Path.Combine(plugin.modPath, $"flag_{id}_thumb.png"), false)
                                            : InitializeTexture($"MoreFlags.thumbs.flag_{id}_thumb.png"));
        public Texture2D thumbWall => m_thumbWall ??
                                    (m_thumbWall =
                                        id.Contains(".")
                                            ? Util.LoadTextureFromFile(Path.Combine(plugin.modPath, $"flag_{id}_thumbwall.png"), false)
                                            : InitializeTexture($"MoreFlags.thumbs.flag_{id}_thumbwall.png"));
        public PluginManager.PluginInfo plugin;

        private static Texture2D InitializeTexture(string path)
        {
            return Util.LoadTextureFromAssembly(path, false);
        }
    }
}