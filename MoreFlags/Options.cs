using MoreFlags.OptionsFramework.Attibutes;

namespace MoreFlags
{
    [Options("MoreFlags", "CSL-MoreFlags.xml")]
    public class Options
    {
        public Options()
        {
            replacement = LoadingExtension.Flags[0].id;
        }

        public string replacement { set; get; }
    }
}