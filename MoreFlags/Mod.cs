using System;
using System.Linq;
using ICities;

namespace MoreFlags
{
    public class Mod : IUserMod
    {
        private static bool _optionsLoaded;

        public string Name
        {
            get
            {
                if (!_optionsLoaded)
                {
                    OptionsLoader.LoadOptions();
                    _optionsLoaded = true;
                }
                return "More Flags";
            }
        }

        public string Description => "More Flags";

        public void OnSettingsUI(UIHelperBase helper)
        {
            var group = helper.AddGroup("MoreFlags");
            var defaultIndex = 0;
            if (OptionsHolder.Options.replacement != string.Empty)
            {
                for (var i = 0; i < LoadingExtension.Flags.Count; i++)
                {
                    var flag = LoadingExtension.Flags[i];
                    if (!flag.id.Equals(OptionsHolder.Options.replacement))
                    {
                        continue;
                    }
                    defaultIndex = i + 1;
                    break;
                }
                if (defaultIndex == 0)
                {
                    OptionsHolder.Options.replacement = string.Empty;
                    OptionsLoader.SaveOptions();
                }
            }

            group.AddDropdown("Replace stock Flags with",
                new[] { "-----" }.Concat(LoadingExtension.Flags.Select(flag => flag.description)).ToArray(), defaultIndex, sel =>
                 {
                     OptionsHolder.Options.replacement = sel == 0 ? string.Empty : LoadingExtension.Flags[sel - 1].id;
                     OptionsLoader.SaveOptions();
                 });
        }
    }
}
