using CommandLine;

namespace G2GFxDataTool
{
    public class Options
    {
        [Option('i', "input", SetName = "export", Required = false, HelpText = "Required. Path to the SWF file.")]
        public string inputPath { get; set; }

        [Option('o', "output", SetName = "export", Required = false, HelpText = "Required. Path to output files.")]
        public string outputPath { get; set; }
    }
}
