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

        [Checkbox("Flags of countries in Africa", EnabledBuiltInPackages)] public bool Africa { get; set; } = true;

        [Checkbox("Flags of countries in Latin America", EnabledBuiltInPackages)]
        public bool America { get; set; } = true;

        [Checkbox("Flags of US, UK & Commonwealth countries", EnabledBuiltInPackages)]
        public bool Anglophone { get; set; } = true;

        [Checkbox("Flags of countries in Asia (w/o Commonwealth countries)", EnabledBuiltInPackages)]
        public bool Asia { get; set; } = true;

        [Checkbox("Flags of countries in Europe flags (plus EU flag)", EnabledBuiltInPackages)]
        public bool Europe { get; set; } = true;

        [Checkbox("Flags of fictional and historical entities", EnabledBuiltInPackages)]
        public bool Fictional { get; set; } = true;

        [Checkbox("Flags of movements & organisations", EnabledBuiltInPackages)]
        public bool Organisations { get; set; } = true;
    }
}