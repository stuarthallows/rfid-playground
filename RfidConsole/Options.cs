using CommandLine;
using CommandLine.Text;

namespace RfidConsole
{
    public class Options
    {
        [Option('d', "DwellTime", DefaultValue = 2000, HelpText = "Time to dwell in milliseconds")]
        public int DwellTime { get; set; }

        [Option('p', "PowerLevel", DefaultValue = 300, HelpText = "The power level in tenths of dBm")]
        public int PowerLevel { get; set; }

        [Option('c', "PacketCount", DefaultValue = 40, HelpText = "Number of MAC packet to handle")]
        public int PacketCount { get; set; }

        /// <summary>
        /// When performing Inventory operations, if a tag is singulated, this extension is enabled, and the additional pre-requisite inventory 
        /// controls have been configured, the tag will be suppressed in the tag population for the duration of the inventory operation.
        /// </summary>
        [Option('t', "TagFocus", DefaultValue = false, HelpText = "Enable tag focus")]
        public bool IsTagFocusEnabled { get; set; }

        /// <summary>
        /// When performing Inventory operations, if a tag is singulated and this extension is enabled, then the Tag TID memory is returned 
        /// along with the Tag EPC data.
        /// </summary>
        [Option('f', "FastId", DefaultValue = false, HelpText = "Enable fast Id")]
        public bool IsFastIdEnabled { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("RfidConsole", "1.0.0"),
                Copyright = new CopyrightInfo("Somark", 2017),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };

            help.AddPreOptionsLine("Usage: RfidConsole -d 2000 -p 300 -c 40 -t false -f false");
            help.AddOptions(this);

            return help;
        }
    }
}
