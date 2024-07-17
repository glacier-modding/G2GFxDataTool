using CommandLine;

namespace G2GFxDataTool
{
    public class Options
    {
        [Option('i', "input", SetName = "export", Required = false, HelpText = "Required. Path to the SWF file or directory containing SWF files.")]
        public string inputPath { get; set; }

        [Option('o', "output", SetName = "export", Required = false, HelpText = "Path to output the files (defaults to the current working directory).")]
        public string outputPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "output");

        [Option('b', "base-assembly-path", SetName = "export", Required = false, HelpText = "Base assembly path (defaults to /ui/controls/).")]
        public string baseAssemblyPath { get; set; } = "/ui/controls/";

        [Option('g', "gfxexport", SetName = "export", Required = false, HelpText = "Path to gfxexport.exe (defaults to \"gfxexport.exe\" in the folder where G2GFxDataTool.exe is located).")]
        public string gfxexportPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "gfxexport.exe";

        [Option('s', "save-paths", SetName = "export", Required = false, HelpText = "Saves Scaleform GFx, UIControl and aspect paths to scaleformgfx.txt, uicontrol.txt and aspect.txt text files in the output directory.")]
        public bool savePaths { get; set; }

        [Option("game", SetName = "export", Required = false, HelpText = "Game version. Possible options are Hitman2016, Hitman2 and Hitman3 (defaults to Hitman3).\r\nNote: these are case sensitive.")]
        public ResourceLib.Game game { get; set; } = ResourceLib.Game.Hitman3;

        [Option('v', "verbose", SetName = "export", Required = false, HelpText = "Sets output to verbose messages mode.")]
        public bool verbose { get; set; }

        [Option('l', "licenses", SetName = "licenses", Required = false, HelpText = "Prints license information for G2GFxDataTool and third party libraries that are used.")]
        public bool licenses { get; set; }
    }
}
