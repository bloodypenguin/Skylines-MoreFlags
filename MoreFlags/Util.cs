using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace MoreFlags
{
    public static class Util
    {
        public static void AddLocale(string idBase, string key, string title, string description)
        {
            var localeField =
                typeof(LocaleManager).GetField("m_Locale", BindingFlags.NonPublic | BindingFlags.Instance);
            var locale = (Locale) localeField.GetValue(SingletonLite<LocaleManager>.instance);
            var localeKey = new Locale.Key() {m_Identifier = $"{idBase}_TITLE", m_Key = key};
            if (!locale.Exists(localeKey))
            {
                locale.AddLocalizedString(localeKey, title);
            }

            localeKey = new Locale.Key() {m_Identifier = $"{idBase}_DESC", m_Key = key};
            if (!locale.Exists(localeKey))
            {
                locale.AddLocalizedString(localeKey, description);
            }

            localeKey = new Locale.Key() {m_Identifier = $"{idBase}", m_Key = key};
            if (!locale.Exists(localeKey))
            {
                locale.AddLocalizedString(localeKey, description);
            }
        }

        public static Texture2D LoadTextureFromAssembly(string path, string textureName, bool readOnly = true)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (var textureStream = assembly.GetManifestResourceStream(path))
                {
                    return LoadTextureFromStream(readOnly, textureName, textureStream);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public static Texture2D LoadTextureFromFile(string path, string textureName, bool readOnly = false)
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"More Flags - Texture file {path} doesn't exist!");
                return null;
            }

            try
            {
                using (var textureStream = File.OpenRead(path))
                {
                    return LoadTextureFromStream(readOnly, textureName, textureStream);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public static UITextureAtlas CreateAtlas(Texture2D[] sprites)
        {
            UITextureAtlas atlas = new UITextureAtlas();
            atlas.material = new Material(GetUIAtlasShader());

            Texture2D texture = new Texture2D(0, 0);
            Rect[] rects = texture.PackTextures(sprites, 0);

            for (int i = 0; i < rects.Length; ++i)
            {
                Texture2D sprite = sprites[i];
                if (sprite == null)
                {
                    continue;
                }

                Rect rect = rects[i];

                UITextureAtlas.SpriteInfo spriteInfo = new UITextureAtlas.SpriteInfo();
                spriteInfo.name = sprite.name;
                spriteInfo.texture = sprite;
                spriteInfo.region = rect;
                spriteInfo.border = new RectOffset();

                atlas.AddSprite(spriteInfo);
            }

            atlas.material.mainTexture = texture;
            return atlas;
        }

        private static Shader GetUIAtlasShader()
        {
            return Shader.Find("UI/Default UI Shader"); //UIView.GetAView().defaultAtlas.material.shader;
        }

        private static Texture2D LoadTextureFromStream(bool readOnly, string textureName, Stream textureStream)
        {
            var buf = new byte[textureStream.Length]; //declare arraysize
            textureStream.Read(buf, 0, buf.Length); // read from stream to byte array
            textureStream.Close();
            var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(buf);
            tex.name = textureName;
            tex.filterMode = FilterMode.Bilinear;
            tex.Compress(false);
            tex.Apply(false, readOnly);
            return tex;
        }

        public static Texture2D CloneTexture(Material material, string textureName, bool readOnly = true)
        {
            var texture = (Texture2D) material.GetTexture(textureName);
            var tex = new Texture2D(texture.width, texture.height, texture.format, false);
            try
            {
                tex.SetPixels(texture.GetPixels(0, 0, texture.width, texture.height));
            }
            catch
            {
                tex.SetPixels(MakeReadable(texture).GetPixels(0, 0, texture.width, texture.height));
            }
            tex.name = textureName;
            tex.filterMode = texture.filterMode;
            tex.Compress(false);
            tex.Apply(false, readOnly);
            return tex;
        }

        private static Texture2D MakeReadable(this Texture texture)
        {
            var rt = RenderTexture.GetTemporary(texture.width, texture.height, 0);
            Graphics.Blit(texture, rt);
            var tex = ToTexture2D(rt);
            RenderTexture.ReleaseTemporary(rt);
            return tex;
        }

        private static Texture2D ToTexture2D(this RenderTexture rt)
        {
            var oldRt = RenderTexture.active;
            RenderTexture.active = rt;
            var tex = new Texture2D(rt.width, rt.height);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();
            RenderTexture.active = oldRt;
            return tex;
        }

        public static bool IsModActive(string modName)
        {
            var plugins = PluginManager.instance.GetPluginsInfo();
            return (from plugin in plugins.Where(p => p.isEnabled)
                select plugin.GetInstances<IUserMod>()
                into instances
                where instances.Any()
                select instances[0].Name
                into name
                where name == modName
                select name).Any();
        }

        public static IEnumerator ActionWrapper(Action a)
        {
            a.Invoke();
            yield break;
        }
    }
}