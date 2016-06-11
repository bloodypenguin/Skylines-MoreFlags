using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using PrefabHook;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MoreFlags
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private static readonly FieldInfo LocaleField = typeof(LocaleManager).GetField("m_Locale", BindingFlags.NonPublic | BindingFlags.Instance);
        private static List<Flag> m_flags;
        private static readonly string[][] DefaultFlags = {
            new[]{ "europe_vanilla", "Europe Vanilla", "UK, Germany, France, Belgium, Italy and Spain flags"},
            new[]{ "africa_north1", "Africa North #1", "Egypt, Lybia, Morocco, Tunisia, Algeria, Western Sahara flags"},
            new[]{ "africa_south1", "Africa South #1", "Namibia, Botswana, Swaziland, Lesotho, Zimbabwe, Mozambique flags"},
            new[]{ "africa_south2", "Africa South #2", "South Africa, Angola, Zambia, Malawi, Comoros, Mauritius flags"},
            new[]{ "asia", "Asia East", "Korea, North Korea, Taiwan, China, Mongolia, Japan flags"},
            new[]{ "asia_central", "Asia Central", "Kazakhstan, Kyrgyzstan, Afghanistan, Tajikistan, Uzbekistan, Turkmenistan flags"},
            new[]{ "asia_middle_east1", "Asia Middle East #1", "Yemen, Israel, Jordan, Lebannon, Syria, Saudi Arabia flags"},
            new[]{ "asia_middle_east2", "Asia Middle East #2", "Iran, Iraq, Oman, Kuwait, Bahrain, UAE flags"},
            new[]{ "asia_south", "Asia South", "Myanmar, Laos, Bangladesh, Bhutan, Nepal, Cambodia flags"},
            new[]{ "asia_southeast", "Asia Southeast", "Vietnam, Indonesia, Philippines, Thailand, Malaysia, Singapore flags"},
            new[]{ "bloody", "BloodyPenguin", "BloodyPenguin flag"},
            new[]{ "central_america", "Central America", "Cuba, Mexico, Jamaica, Dominican Republic, Bahamas, Panama flags"},
            new[]{ "christian", "Christian", "Christian flag"},
            new[]{ "commonwealth", "Commonwealth", "Canada, India, Australia, New Zealand, Papua New Guinea, Pakistan flags"},
            new[]{ "eu", "European Union", "European Union flag"},
            new[]{ "europe_central", "Europe Central", "Estonia, Latvia, Lithuania, Poland, Czech Republic, Slovakia flags"},
            new[]{ "europe_east", "Europe East", "Ukraine, Russia, Belarus flags"},
            new[]{ "europe_north", "Europe North", "Norway, Finland, Iceland, Sweden, Denmark, Ireland flags"},
            new[]{ "europe_south", "Europe South", "Greece, Georgia, Turkey, Armenia, Azerbadian, Cyprus flags"},
            new[]{ "europe_southeast1", "Europe Southeast #1", "Hungary, Bulgaria, Serbia, Romania,  Macedonia, Moldova flags"},
            new[]{ "europe_southeast2", "Europe Southeast #2", "Slovenia, Bosnia & Herzegovina, Kosovo, Albania, Montenegro, Croatia flags"},
            new[]{ "europe_west", "Europe West", "Portugal, Switzerland, Netherlands, Luxembourg, Malta, Austria flags"},
            new[]{ "jolly_rodger", "Jolly Rodger", "Jolly Rodger flag"},
            new[]{ "mcu_teams", "Marvel Teams", "S.H.I.E.L.D, Avengers, X-Men flags"},
            new[]{ "pace", "Peace", "Peace flag"},
            new[]{ "south_america", "South America", "Brazil, Argentina, Colombia, Chile, Peru, Venezuela flags"},
            new[]{ "su", "Soviet Union", "Soviet Union flag"},
            new[]{ "sw", "Star Wars Factions", "Galactic Empire, Rebel Alliance, First Order flag"},
            new[]{ "uk", "United Kingdom", "Union Jack, Coat of Arms, England, Scotland, Wales, Nothern Ireland flags"},
            new[]{ "un", "United Nations", "United Nations flag"},
            new[]{ "us", "United States", "United States flag"}
        };

        private static UITextureAtlas _atlas;

        public static List<Flag> Flags
        {
            get
            {
                if (m_flags != null)
                {
                    return m_flags;
                }
                var flagsList = (from flag in DefaultFlags
                                 let id = flag[0]
                                 select new Flag
                                 {
                                     id = id,
                                     description = flag[1],
                                     extendedDescripton = flag[2],
                                     texture = InitializeTexture($"MoreFlags.flags.flag_{id}.png"),
                                     textureLod = InitializeTexture($"MoreFlags.flags.flag_{id}_lod.png"),
                                     thumb = InitializeTexture($"MoreFlags.thumbs.flag_{id}_thumb.png"),
                                     thumbWall = InitializeTexture($"MoreFlags.thumbs.flag_{id}_thumbwall.png")
                                 }).ToList();
                try
                {
                    var plugins = PluginManager.instance.GetPluginsInfo();
                    foreach (var plugin in plugins)
                    {
                        var instances = plugin.GetInstances<IUserMod>();
                        if (instances.Length != 1)
                        {
                            continue;
                        }
                        var instance = instances[0];
                        var methodInfo = instance.GetType().GetMethod("CustomFlags");
                        var customFlags = (string[][])methodInfo?.Invoke(instance, new object[] { });
                        if (customFlags == null)
                        {
                            continue;
                        }
                        foreach (var flag in customFlags)
                        {
                            if (flag.Length < 2)
                            {
                                continue;
                            }
                            var id = flag[0];
                            var flagInstance = new Flag
                            {
                                plugin = plugin,
                                id = plugin.publishedFileID + "." + id,
                                description = flag[1],
                                texture = Util.LoadTextureFromFile(Path.Combine(plugin.modPath, $"flag_{id}.png"), true),
                                textureLod = Util.LoadTextureFromFile(Path.Combine(plugin.modPath, $"flag_{id}_lod.png"), false),
                                thumb = Util.LoadTextureFromFile(Path.Combine(plugin.modPath, $"flag_{id}_thumb.png"), false),
                                thumbWall = Util.LoadTextureFromFile(Path.Combine(plugin.modPath, $"flag_{id}_thumbwall.png"), false)
                            };
                            if (flag.Length >= 3)
                            {
                                flagInstance.extendedDescripton = flag[2];
                            }
                            flagsList.Add(flagInstance);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                var sprites = new List<Texture2D>();
                foreach (var flag in flagsList)
                {
                    sprites.Add(flag.thumb);
                    sprites.Add(flag.thumbWall);
                }
                _atlas = Util.CreateAtlas(sprites.ToArray());
                m_flags = flagsList;
                return m_flags;
            }
        }

        private static Texture2D InitializeTexture(string path)
        {
            return Util.LoadTextureFromAssembly(path, false);
        }

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            if (!IsHooked())
            {
                return;
            }
            PropInfoHook.OnPreInitialization += OnPrePropInit;
            PropInfoHook.OnPostInitialization += OnPostPropInit;
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
            foreach (var prefab in Flags
                .Where(flag => flag.plugin == null || flag.plugin.isEnabled)
                .Select(flag => Clone(prop, flag, isWall)))
            {
                prefab.m_UIPriority = ++counter;
            }
            if (OptionsHolder.Options.replacement != string.Empty)
            {
                foreach (var flag in Flags.Where(flag => (flag.plugin == null || flag.plugin.isEnabled) && flag.id == OptionsHolder.Options.replacement))
                {
                    Replace(prop, flag);
                    break;
                }
            }
        }

        public void OnPostPropInit(PropInfo prop)
        {
            if (prop == null)
            {
                return;
            }
            if (prop.name != "flag_pole_wall" && prop.name != "flag_pole")
            {
                return;
            }
            ApplyRenderDistanceHack(prop);
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
            clone.GetComponent<Renderer>().material.mainTexture = modification.texture;
            clone.GetComponent<Renderer>().material.name = $"{prop.GetComponent<Renderer>().material.name}_{modification.id}";
//            clone.m_lodObject = Object.Instantiate(prop.m_lodObject);
//            clone.m_lodObject.transform.parent = instance.transform;
//            clone.m_lodObject.name = prop.m_lodObject.name + $"_{modification.id}";
//            var renderer = clone.m_lodObject.GetComponent<MeshRenderer>();
//            Object.DestroyImmediate(renderer);
//            renderer = clone.m_lodObject.AddComponent<MeshRenderer>();
//            renderer.material= new Material(prop.m_lodObject.GetComponent<Renderer>().sharedMaterial)
//            {
//                mainTexture = modification.textureLod,
//                name = $"{prop.m_lodObject.GetComponent<Renderer>().sharedMaterial.name}_{modification.id}"
//            };
            clone.m_placementStyle = ItemClass.Placement.Manual;
            clone.m_createRuining = false;
            clone.m_Atlas = _atlas;
            clone.m_InfoTooltipAtlas = _atlas;
            var thumb = isWall ? modification.thumbWall : modification.thumb;
            if (thumb != null)
            {
                clone.m_Thumbnail = thumb.name;
                clone.m_InfoTooltipThumbnail = thumb.name;
            }
            PrefabCollection<PropInfo>.InitializePrefabs("MoreFlags", new[] { clone }, null);
            ApplyRenderDistanceHack(clone);
            AddLocale(modification, isWall, name);
            collection.flags.Add(clone);
            return clone;
        }

        //hack to always render main model instad of LOD. Should be after initializing
        private static void ApplyRenderDistanceHack(PropInfo flag)
        {
            flag.m_maxRenderDistance = 590; 
            flag.m_lodRenderDistance = 590;
        }

        private static void AddLocale(Flag modification, bool isWall, string name)
        {
            var versionStr = isWall ? "wall" : "ground";
            var extendedDescription =
                modification.extendedDescripton == string.Empty ? modification.description : modification.extendedDescripton;
            Util.AddLocale("PROPS", name, $"{modification.description} ({versionStr} version)",
                $"{extendedDescription} ({versionStr} version)");
        }

        private static bool IsHooked()
        {
            return Util.IsModActive("Prefab Hook");
        }
    }
}