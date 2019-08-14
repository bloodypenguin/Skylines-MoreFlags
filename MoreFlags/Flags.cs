using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.Plugins;
using ICities;
using MoreFlags.OptionsFramework;
using UnityEngine;

namespace MoreFlags
{
    public static class Flags
    {
        public static List<Flag> CollectFlags(bool ignoreOptions)
        {
            var flagsList = (from flag in GetDefaultFlags(ignoreOptions)
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

            return flagsList;
        }


        private static IEnumerable<string[]> GetDefaultFlags(bool ignoreOptions)
        {
            var result = new List<string[]>();
            if (ignoreOptions || OptionsWrapper<Options>.Options.Africa)
            {
                result.AddRange(new[]
                {
                    new[]
                    {
                        "africa_north1", "Africa North #1",
                        "Egypt, Libya, Morocco, Tunisia, Algeria, Western Sahara flags"
                    },
                    new[]
                    {
                        "africa_south1", "Africa South #1",
                        "Namibia, Botswana, Swaziland, Lesotho, Zimbabwe, Mozambique flags"
                    },
                    new[]
                    {
                        "africa_south2", "Africa South #2",
                        "South Africa, Angola, Zambia, Malawi, Comoros, Mauritius flags"
                    }
                });
            }

            if (ignoreOptions || OptionsWrapper<Options>.Options.Europe)
            {
                result.AddRange(new[]
                {
                    new[]
                    {
                        "europe_vanilla", "Europe Vanilla", "UK, Germany, France, Belgium, Italy and Spain flags"
                    },
                    new[] {"eu", "European Union", "European Union flag"},
                    new[]
                    {
                        "europe_central", "Europe Central",
                        "Estonia, Latvia, Lithuania, Poland, Czech Republic, Slovakia flags"
                    },
                    new[] {"europe_east", "Europe East", "Ukraine, Russia, Belarus flags"},
                    new[]
                    {
                        "europe_north", "Europe North", "Norway, Finland, Iceland, Sweden, Denmark, Ireland flags"
                    },
                    new[]
                    {
                        "europe_small", "Europe Small States",
                        "Faroe Islands, Andorra, Monaco, Vatican City, San Marino, Liechtenstein flags"
                    },
                    new[]
                    {
                        "europe_south", "Europe South", "Greece, Georgia, Turkey, Armenia, Azerbaijan, Cyprus flags"
                    },
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
                        "europe_west", "Europe West",
                        "Portugal, Switzerland, Netherlands, Luxembourg, Malta, Austria flags"
                    },
                });
            }

            if (ignoreOptions || OptionsWrapper<Options>.Options.Asia)
            {
                result.AddRange(new[]
                {
                    new[] {"asia", "Asia East", "Korea, North Korea, Taiwan, China, Mongolia, Japan flags"},
                    new[]
                    {
                        "asia_central", "Asia Central",
                        "Kazakhstan, Kyrgyzstan, Afghanistan, Tajikistan, Uzbekistan, Turkmenistan flags"
                    },
                    new[]
                    {
                        "asia_middle_east1", "Asia Middle East #1",
                        "Yemen, Israel, Jordan, Lebanon, Syria, Saudi Arabia flags"
                    },
                    new[]
                    {
                        "asia_middle_east2", "Asia Middle East #2", "Iran, Iraq, Oman, Kuwait, Bahrain, UAE flags"
                    },
                    new[] {"asia_south", "Asia South", "Myanmar, Laos, Bangladesh, Bhutan, Nepal, Cambodia flags"},
                    new[]
                    {
                        "asia_southeast", "Asia Southeast",
                        "Vietnam, Indonesia, Philippines, Thailand, Malaysia, Singapore flags"
                    },
                });
            }

            if (ignoreOptions || OptionsWrapper<Options>.Options.America)
            {
                result.AddRange(new[]
                {
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
                    new[]
                    {
                        "south_america", "South America",
                        "Brazil, Argentina, Colombia, Chile, Peru, Venezuela flags"
                    },
                });
            }

            if (ignoreOptions || OptionsWrapper<Options>.Options.Fictional)
            {
                result.AddRange(new[]
                {
                    new[] {"bloody", "BloodyPenguin", "BloodyPenguin flag"},
                    new[]
                    {
                        "historical", "Historical flags",
                        "Yugoslavia, Kingdom or Ararat, Tibet, East Germany, Confederate, Russian Empire flags"
                    },
                    new[] {"mcu_teams", "Marvel Teams", "S.H.I.E.L.D, Avengers, X-Men flags"},
                    new[] {"sw", "Star Wars Factions", "Galactic Empire, Rebel Alliance, First Order flag"},
                    new[] {"su", "Soviet Union", "Soviet Union flag"},
                    new[] {"jolly_rodger", "Jolly Rodger", "Jolly Rodger flag"},
                });
            }

            if (ignoreOptions || OptionsWrapper<Options>.Options.Anglophone)
            {
                result.AddRange(new[]
                {
                    new[]
                    {
                        "uk", "United Kingdom",
                        "Union Jack, Coat of Arms, England, Scotland, Wales, Northern Ireland flags"
                    },
                    new[] {"us", "United States", "United States flag"},
                    new[]
                    {
                        "commonwealth", "Commonwealth",
                        "Canada, India, Australia, New Zealand, Papua New Guinea, Pakistan flags"
                    },
                });
            }


            if (ignoreOptions || OptionsWrapper<Options>.Options.Organisations)
            {
                result.AddRange(new[]
                {
                    new[] {"christian", "Christian", "Christian flag"},
                    new[] {"pace", "Peace", "Peace flag"},
                    new[] {"un", "United Nations", "United Nations flag"},
                    new[] {"space", "Space Agencies", "Space Agencies flag"}
                });
            }

            return result;
        }
    }
}