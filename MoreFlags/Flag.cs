using ColossalFramework.Plugins;
using UnityEngine;

namespace MoreFlags
{
    public struct Flag
    {
        public string id;
        public string description;
        public string extendedDescripton;
        public Texture2D texture;
        public Texture2D textureLod;
        public Texture2D thumb;
        public Texture2D thumbWall;
        public PluginManager.PluginInfo plugin;
    }
}