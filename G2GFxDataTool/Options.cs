﻿using CommandLine;

namespace G2GFxDataTool
{
    public class Options
    {
        [Option('i', "input", SetName = "export", Required = false, HelpText = "Required. Path to the SWF file or directory containing SWF files.")]
        public string inputPath { get; set; }

        [Option('o', "output", SetName = "export", Required = false, HelpText = "Path to output the files (defaults to the current working directory).")]
        public string outputPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "output");

        [Option('s', "save-paths", SetName = "export", Required = false, HelpText = "Saves Scaleform GFx and UIControl paths to scaleformgfx.txt and uicontrol.txt text files in the output directory.")]
        public bool savePaths { get; set; }

        [Option('l', "licenses", SetName = "licenses", Required = false, HelpText = "Prints license information for G2GFxDataTool and third party libraries that are used.")]
        public bool licenses { get; set; }
    }
}
