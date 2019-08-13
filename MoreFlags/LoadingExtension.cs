using System;
using System.Collections.Generic;
using System.IO;
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
        private static List<Flag> m_flags;

        private static readonly string[][] DefaultFlags =
        {
            new[] {"europe_vanilla", "Europe Vanilla", "UK, Germany, France, Belgium, Italy and Spain flags"},
            new[] {"africa_north1", "Africa North #1", "Egypt, Lybia, Morocco, Tunisia, Algeria, Western Sahara flags"},
            new[]
            {
                "africa_south1", "Africa South #1", "Namibia, Botswana, Swaziland, Lesotho, Zimbabwe, Mozambique flags"
            },
            new[]
            {
                "africa_south2", "Africa South #2", "South Africa, Angola, Zambia, Malawi, Comoros, Mauritius flags"
            },
            new[]
            {
                "central_america", "America Central #1",
                "Cuba, Mexico, Jamaica, Dominican Republic, Bahamas, Panama flags"
            },
            new[]
            {
                "america_central2", "America Central #2",
                "Honduras, Guatemala, Belize, Costa Rica, El Salvador, Nicaragua flags"
            },
            new[]
            {
                "america_central3", "America Central #3",
                "Haiti, Antigua and Barbuda, Trinidad and Tobago, Barbados, Grenada, Dominica flags"
            },
            new[] {"asia", "Asia East", "Korea, North Korea, Taiwan, China, Mongolia, Japan flags"},
            new[]
            {
                "asia_central", "Asia Central",
                "Kazakhstan, Kyrgyzstan, Afghanistan, Tajikistan, Uzbekistan, Turkmenistan flags"
            },
            new[]
            {
                "asia_middle_east1", "Asia Middle East #1", "Yemen, Israel, Jordan, Lebannon, Syria, Saudi Arabia flags"
            },
            new[] {"asia_middle_east2", "Asia Middle East #2", "Iran, Iraq, Oman, Kuwait, Bahrain, UAE flags"},
            new[] {"asia_south", "Asia South", "Myanmar, Laos, Bangladesh, Bhutan, Nepal, Cambodia flags"},
            new[]
            {
                "asia_southeast", "Asia Southeast",
                "Vietnam, Indonesia, Philippines, Thailand, Malaysia, Singapore flags"
            },
            new[] {"bloody", "BloodyPenguin", "BloodyPenguin flag"},
            new[] {"christian", "Christian", "Christian flag"},
            new[]
            {
                "commonwealth", "Commonwealth",
                "Canada, India, Australia, New Zealand, Papua New Guinea, Pakistan flags"
            },
            new[] {"eu", "European Union", "European Union flag"},
            new[]
            {
                "europe_central", "Europe Central", "Estonia, Latvia, Lithuania, Poland, Czech Republic, Slovakia flags"
            },
            new[] {"europe_east", "Europe East", "Ukraine, Russia, Belarus flags"},
            new[] {"europe_north", "Europe North", "Norway, Finland, Iceland, Sweden, Denmark, Ireland flags"},
            new[]
            {
                "europe_small", "Europe Small States",
                "Faroe Islands, Andorra, Monaco, Vatican City, San Marino, Liechtenstein flags"
            },
            new[] {"europe_south", "Europe South", "Greece, Georgia, Turkey, Armenia, Azerbadian, Cyprus flags"},
            new[]
            {
                "europe_southeast1", "Europe Southeast #1",
                "Hungary, Bulgaria, Serbia, Romania,  Macedonia, Moldova flags"
            },
            new[]
            {
                "europe_southeast2", "Europe Southeast #2",
                "Slovenia, Bosnia & Herzegovina, Kosovo, Albania, Montenegro, Croatia flags"
            },
            new[]
            {
                "europe_west", "Europe West", "Portugal, Switzerland, Netherlands, Luxembourg, Malta, Austria flags"
            },
            new[]
            {
                "historical", "Historical flags",
                "Yugoslavia, Kingdom or Ararat, Tibet, East Germany, Confederate, Russian Empire flags"
            },
            new[] {"jolly_rodger", "Jolly Rodger", "Jolly Rodger flag"},
            new[] {"mcu_teams", "Marvel Teams", "S.H.I.E.L.D, Avengers, X-Men flags"},
            new[] {"pace", "Peace", "Peace flag"},
            new[] {"south_america", "South America", "Brazil, Argentina, Colombia, Chile, Peru, Venezuela flags"},
            new[] {"su", "Soviet Union", "Soviet Union flag"},
            new[] {"sw", "Star Wars Factions", "Galactic Empire, Rebel Alliance, First Order flag"},
            new[] {"uk", "United Kingdom", "Union Jack, Coat of Arms, England, Scotland, Wales, Nothern Ireland flags"},
            new[] {"un", "United Nations", "United Nations flag"},
            new[] {"us", "United States", "United States flag"}
        };

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
                foreach (var flag in Flags)
                {
                    sprites.Add(flag.thumb);
                    sprites.Add(flag.thumbWall);
                }

                m_atlas = Util.CreateAtlas(sprites.ToArray());
                return m_atlas;
            }
        }

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
                        flagName = id,
                        description = flag[1],
                        extendedDescripton = flag[2],
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
                        var customFlags = (string[][]) methodInfo?.Invoke(instance, new object[] { });
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
                                flagName = id,
                                description = flag[1]
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

                m_flags = flagsList;
                return m_flags;
            }
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
            var flags = Flags.Where(flag => flag.plugin == null || flag.plugin.isEnabled).ToArray();
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
                foreach (var flag in Flags.Where(flag =>
                    (flag.plugin == null || flag.plugin.isEnabled) &&
                    flag.id == OptionsWrapper<Options>.Options.replacement))
                {
                    Replace(prop, flag);
                    break;
                }
            }
        }

        public void OnPostPropInit(PropInfo prop)
        {
            if (OptionsWrapper<Options>.Options.replacement == string.Empty)
            {
                return;
            }

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
            SetupMainMaterial(prop, modification, clone);
            SetupLodMaterial(prop, modification);

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
            ApplyRenderDistanceHack(clone);
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

        private static void SetupLodMaterial(PropInfo prop, Flag modification)
        {
            var lodMaterial = prop.m_lodObject.GetComponent<Renderer>().material;
            lodMaterial.mainTexture = modification.textureLod;
            lodMaterial.SetTexture("_XYSMap", Util.CloneTexture(lodMaterial, "_XYSMap", false));
            lodMaterial.SetTexture("_ACIMap", Util.CloneTexture(lodMaterial, "_ACIMap", false));
            lodMaterial.name = $"{prop.m_lodObject.GetComponent<Renderer>().material.name}_{modification.id}";
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