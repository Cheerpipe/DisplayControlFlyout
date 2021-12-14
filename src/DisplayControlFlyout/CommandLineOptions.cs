using CommandLine;

namespace DisplayControlFlyout
{
    public class CommandLineOptions
    {
        [Option('m', "mode", Required = false, HelpText = "Apply a new display mode.")]
        public string? Mode { get; set; }

        [Option('h', "hdr", Required = false, HelpText = "Turn global HDR on/off")]
        public bool? Hdr { get; set; }
    }
}
