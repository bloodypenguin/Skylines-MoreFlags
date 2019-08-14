using System.Xml.Serialization;
using MoreFlags.OptionsFramework.Attibutes;

namespace MoreFlags
{
    [Options("MoreFlags", "CSL-MoreFlags.xml")]
    public class Options
    {
        [XmlIgnore]
        private const string EnabledBuiltInPackages = "Enabled built-in packages";
        
        public Options()
        {
            replacement = string.Empty;
        }

        public string replacement { set; get; }

        [Checkbox("African flags", EnabledBuiltInPackages)] public bool Africa { get; set; } = true;

        [Checkbox("American flags (w/o US & Canada)", EnabledBuiltInPackages)]
        public bool America { get; set; } = true;

        [Checkbox("Flags of US, UK & Commonwealth countries", EnabledBuiltInPackages)]
        public bool Anglophone { get; set; } = true;

        [Checkbox("Asian flags (w/o Commonwealth countries)", EnabledBuiltInPackages)]
        public bool Asia { get; set; } = true;

        [Checkbox("European flags (+EU)", EnabledBuiltInPackages)]
        public bool Europe { get; set; } = true;

        [Checkbox("Fictional and historical flags", EnabledBuiltInPackages)]
        public bool Fictional { get; set; } = true;

        [Checkbox("Flags of movements & organisations", EnabledBuiltInPackages)]
        public bool Organisations { get; set; } = true;
    }
}