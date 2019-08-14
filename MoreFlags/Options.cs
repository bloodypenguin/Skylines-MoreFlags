using MoreFlags.OptionsFramework.Attibutes;

namespace MoreFlags
{
    [Options("MoreFlags", "CSL-MoreFlags.xml")]
    public class Options
    {
        public Options()
        {
            replacement = string.Empty;
        }

        public string replacement { set; get; }

        [Checkbox("African flags")] public bool Africa { get; set; } = true;

        [Checkbox("American flags (w/o US & Canada)")]
        public bool America { get; set; } = true;

        [Checkbox("Flags of US, UK & Commonwealth countries")]
        public bool Anglophone { get; set; } = true;

        [Checkbox("Asian flags (w/o Commonwealth countries)")]
        public bool Asia { get; set; } = true;

        [Checkbox("European flags (+EU)")] public bool Europe { get; set; } = true;

        [Checkbox("Fictional and historical flags")]
        public bool Fictional { get; set; } = true;

        [Checkbox("Flags of movements & organisations")]
        public bool Organisations { get; set; } = true;
    }
}