using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using MoreFlags.OptionsFramework;
using PrefabHook;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MoreFlags
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private static List<Flag> flagCache;
        
        private static UITextureAtlas m_atlas;

        private static UITextureAtlas Atlas
        {
            get
            {
                if (m_atlas != null)
                {
                    return m_atlas;
                }

                var sprites = new List<Texture2D>();
                foreach (var flag in flagCache)
                {
                    sprites.Add(flag.thumb);
                    sprites.Add(flag.thumbWall);
                }

                m_atlas = Util.CreateAtlas(sprites.ToArray());
                return m_atlas;
            }
        }

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            flagCache = Flags.CollectFlags(false);
            if (!IsHooked())
            {
                return;
            }

            PropInfoHook.OnPreInitialization += OnPrePropInit;
            PropInfoHook.Deploy();
        }


        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (IsHooked())
            {
                return;
            }

            UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage(
                "Missing dependency",
                "'More Flags' mod requires the 'Prefab Hook' mod to work properly. Please subscribe to the mod and restart the game!",
                false);
        }

        public override void OnReleased()
        {
            base.OnReleased();
            if (!IsHooked())
            {
                return;
            }

            PropInfoHook.Revert();
        }

        public void OnPrePropInit(PropInfo prop)
        {
            if (prop == null)
            {
                return;
            }

            if (prop.name != "flag_pole_wall" && prop.name != "flag_pole")
            {
                return;
            }

            var isWall = "flag_pole_wall".Equals(prop.name);
            var counter = prop.m_UIPriority;
            var flags = flagCache.Where(flag => flag.plugin == null || flag.plugin.isEnabled).ToArray();
            foreach (var flag in flags)
            {
                var newPrefab = Clone(prop, flag, isWall);
                newPrefab.m_UIPriority = ++counter;
            }

            if (OptionsWrapper<Options>.Options.replacement == string.Empty)
            {
                return;
            }

            {
                foreach (var flag in flagCache.Where(flag =>
                    (flag.plugin == null || flag.plugin.isEnabled) &&
                    flag.id == OptionsWrapper<Options>.Options.replacement))
                {
                    Replace(prop, flag);
                    break;
                }
            }
        }

        private static void Replace(PropInfo prop, Flag modification)
        {
            var material = prop.GetComponent<Renderer>().material;
            material.mainTexture = modification.texture;
            var lodMaterial = prop.m_lodObject.GetComponent<Renderer>().material;
            lodMaterial.mainTexture = modification.textureLod;
        }

        private static PropInfo Clone(PropInfo prop, Flag modification, bool isWall)
        {
            var gameObject = GameObject.Find("MoreFlags") ?? new GameObject("MoreFlags");
            var collection = gameObject.GetComponent<FlagsCollection>() ?? gameObject.AddComponent<FlagsCollection>();
            var instance = Object.Instantiate(prop.gameObject);
            var clone = instance.GetComponent<PropInfo>();
            var name = $"{prop.name}_{modification.id}";
            clone.name = name;
            instance.name = name;
            instance.transform.parent = gameObject.transform;

            clone.m_lodObject = Object.Instantiate(prop.m_lodObject);
            clone.m_lodObject.transform.parent = instance.transform;
            clone.m_lodObject.name = prop.m_lodObject.name + $"_{modification.id}";
            var renderer = clone.m_lodObject.GetComponent<MeshRenderer>();
            Object.DestroyImmediate(renderer);
            renderer = clone.m_lodObject.AddComponent<MeshRenderer>();
            renderer.material= new Material(prop.m_lodObject.GetComponent<Renderer>().sharedMaterial)
            {
                mainTexture = modification.textureLod,
                name = $"{prop.m_lodObject.GetComponent<Renderer>().sharedMaterial.name}_{modification.id}"
            };
            
            SetupMainMaterial(prop, modification, clone);
            SetupLodMaterial(prop, modification, clone);

            clone.m_placementStyle = ItemClass.Placement.Manual;
            clone.m_createRuining = false;
            clone.m_Atlas = Atlas;
            clone.m_InfoTooltipAtlas = Atlas;
            var thumb = isWall ? modification.thumbWall : modification.thumb;
            if (thumb != null)
            {
                clone.m_Thumbnail = thumb.name;
                clone.m_InfoTooltipThumbnail = thumb.name;
            }

            PrefabCollection<PropInfo>.InitializePrefabs("MoreFlags", new[] {clone}, null);
            AddLocale(modification, isWall, name);
            collection.flags.Add(clone);
            return clone;
        }

        private static void SetupMainMaterial(PropInfo prop, Flag modification, PropInfo clone)
        {
            var renderer = clone.GetComponent<Renderer>();
            var material = renderer.material;
            material.mainTexture = modification.texture;
//TODO: is this needed at all?            
//            material.SetTexture("_XYSMap", Util.CloneTexture(material, "_XYSMap"));
//            material.SetTexture("_ACIMap", Util.CloneTexture(material, "_ACIMap"));
            material.name = $"{prop.GetComponent<Renderer>().material.name}_{modification.id}";
        }

        private static void SetupLodMaterial(PropInfo prop, Flag modification, PropInfo clone)
        {
            var lodMaterial = clone.m_lodObject.GetComponent<Renderer>().material;
            lodMaterial.mainTexture = modification.textureLod;
            lodMaterial.SetTexture("_XYSMap", Util.CloneTexture(lodMaterial, "_XYSMap", false));
            lodMaterial.SetTexture("_ACIMap", Util.CloneTexture(lodMaterial, "_ACIMap", false));
            lodMaterial.name = $"{prop.m_lodObject.GetComponent<Renderer>().material.name}_{modification.id}";
        }

        private static void AddLocale(Flag modification, bool isWall, string name)
        {
            var versionStr = isWall ? "wall" : "ground";
            var extendedDescription =
                modification.extendedDescripton == string.Empty
                    ? modification.description
                    : modification.extendedDescripton;
            Util.AddLocale("PROPS", name, $"{modification.description} ({versionStr} version)",
                $"{extendedDescription} ({versionStr} version)");
        }

        private static bool IsHooked()
        {
            return Util.IsModActive("Prefab Hook");
        }
    }
}