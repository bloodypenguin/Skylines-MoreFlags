using System;
using System.Collections;
using System.IO;
using System.Linq;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MoreFlags
{
    public static class Util
    {
        public static Texture2D LoadTextureFromAssembly(string path, bool readOnly = true)
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                using (var textureStream = assembly.GetManifestResourceStream(path))
                {
                    return LoadTextureFromStream(readOnly, textureStream);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
                return null;
            }
        }

        public static Texture2D LoadTextureFromFile(string path, bool readOnly = true)
        {
            try
            {
                using (var textureStream = File.OpenRead(path))
                {
                    return LoadTextureFromStream(readOnly, textureStream);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
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
            return UIView.GetAView().defaultAtlas.material.shader;
        }

        private static Texture2D LoadTextureFromStream(bool readOnly, Stream textureStream)
        {
            var buf = new byte[textureStream.Length]; //declare arraysize
            textureStream.Read(buf, 0, buf.Length); // read from stream to byte array
            textureStream.Close();
            var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(buf);
            tex.Apply(false, readOnly);
            tex.name = Guid.NewGuid().ToString();
            return tex;
        }

        public static PropInfo ClonePrefab(PropInfo originalPrefab, string newName, Transform parentTransform)
        {
            var instance = Object.Instantiate(originalPrefab);
            instance.name = newName;
            instance.transform.SetParent(parentTransform);
            instance.transform.localPosition = new Vector3(-7500, -7500, -7500);
            instance.m_generatedInfo = Object.Instantiate(originalPrefab.m_generatedInfo);
            instance.m_generatedInfo.name = newName + " (GeneratedInfo)";
            instance.m_generatedInfo.m_propInfo = null;
            instance.gameObject.SetActive(false);
            return instance;
        }

        public static bool IsModActive(string modName)
        {
            var plugins = PluginManager.instance.GetPluginsInfo();
            return (from plugin in plugins.Where(p => p.isEnabled)
                    select plugin.GetInstances<IUserMod>() into instances
                    where instances.Any()
                    select instances[0].Name into name
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