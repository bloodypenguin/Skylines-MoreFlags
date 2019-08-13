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
        public string flagName;
        public string description;
        public string extendedDescripton;

        public Texture2D texture => m_texture ? m_texture : (m_texture =
            plugin != null
                ? Util.LoadTextureFromFile(Path.Combine(plugin.modPath, $"flag_{flagName}.png"), $"flag_{id}", true)
                : InitializeTexture($"MoreFlags.flags.flag_{flagName}.png", $"flag_{id}", true));

        public Texture2D textureLod => m_textureLod ? m_textureLod : (m_textureLod =
            plugin != null
                ? Util.LoadTextureFromFile(Path.Combine(plugin.modPath, $"flag_{flagName}_lod.png"), $"flag_{id}_lod")
                : InitializeTexture($"MoreFlags.flags.flag_{flagName}_lod.png", $"flag_{id}_lod"));
        public Texture2D thumb => m_thumb ? m_thumb : (m_thumb =
            plugin != null
                ? Util.LoadTextureFromFile(Path.Combine(plugin.modPath, $"flag_{flagName}_thumb.png"), $"flag_{id}_thumb")
                : InitializeTexture($"MoreFlags.thumbs.flag_{flagName}_thumb.png", $"flag_{id}_thumb"));
        public Texture2D thumbWall => m_thumbWall ? m_thumbWall : (m_thumbWall =
            plugin!=null
                ? Util.LoadTextureFromFile(Path.Combine(plugin.modPath, $"flag_{flagName}_thumbwall.png"), $"flag_{id}_thumbwall")
                : InitializeTexture($"MoreFlags.thumbs.flag_{flagName}_thumbwall.png", $"flag_{id}_thumbwall"));
        public PluginManager.PluginInfo plugin;

        private static Texture2D InitializeTexture(string path, string textureName, bool isReadOnly = false)
        {
            return Util.LoadTextureFromAssembly(path, textureName, isReadOnly);
        }
    }
}