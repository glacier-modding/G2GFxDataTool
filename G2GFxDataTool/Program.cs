using CommandLine;

namespace G2GFxDataTool
{
    internal class Program
    {
        public static HashSet<string> logUIControlPaths = new HashSet<string>();
        public static HashSet<string> logScaleformGFxPaths = new HashSet<string>();

        static void Main(string[] args)
        {
            var parser = Parser.Default.ParseArguments<Options>(args);

            parser
                .WithParsed(options =>
                {
                    if (options.outputPath != null)
                    {
                        if (!Directory.Exists(options.outputPath))
                        {
                            Directory.CreateDirectory(options.outputPath);
                        }
                    }

                    if (options.inputPath != null)
                    {
                        string ext = Path.GetExtension(options.inputPath);
                        if (ext == ".swf")
                        {
                            ScaleformGFxWriter.WriteScaleformGfX(options.inputPath, options.outputPath);
                            UIControlWriter.WriteUIControl(options.inputPath, options.outputPath);
                        }
                    }

                    if (options.licenses)
                    {
                        Licenses.PrintLicenses();
                    }

                    if (options.savePaths)
                    {
                        File.WriteAllLines(Path.Combine(options.outputPath, "uicontrol.txt"), logUIControlPaths);
                        File.WriteAllLines(Path.Combine(options.outputPath, "scaleformgfx.txt"), logScaleformGFxPaths);
                    }

                });
        }
    }
}