using CommandLine;

namespace G2GFxDataTool
{
    public class Options
    {
        [Option('i', "input", SetName = "export", Required = false, HelpText = "Required. Path to the SWF file.")]
        public string inputPath { get; set; }

        [Option('o', "output", SetName = "export", Required = false, HelpText = "Required. Path to output files.")]
        public string outputPath { get; set; }

        [Option('l', "licenses", SetName = "licenses", Required = false, HelpText = "Prints license information for G2GFxDataTool and third party libraries that are used.")]
        public bool licenses { get; set; }
    }
}
