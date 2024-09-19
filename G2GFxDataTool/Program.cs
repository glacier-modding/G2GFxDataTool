using CommandLine;

namespace G2GFxDataTool
{
    internal class Program
    {
        public static HashSet<string> logUIControlPaths = new HashSet<string>();
        public static HashSet<string> logScaleformGFxPaths = new HashSet<string>();
        public static HashSet<string> logAspectPaths = new HashSet<string>();

        static void Main(string[] args)
        {
            var parser = Parser.Default.ParseArguments<Options>(args);

            parser
                .WithParsed(options =>
                {
                    if (!string.IsNullOrEmpty(options.inputPath) && Path.Exists(options.inputPath))
                    {
                        if (!Directory.Exists(options.outputPath))
                        {
                            Directory.CreateDirectory(options.outputPath);
                        }

                        FileAttributes attr = File.GetAttributes(options.inputPath);

                        if (attr.HasFlag(FileAttributes.Directory))
                        {
                            foreach (var file in Directory.GetFiles(options.inputPath))
                            {
                                string ext = Path.GetExtension(file).ToLower();
                                if (ext == ".swf")
                                {
                                    ScaleformGFxWriter.WriteScaleformGfX(file, options.outputPath, options.gfxexportPath, options.baseAssemblyPath, options.game, options.verbose);
                                    UIControlWriter.WriteUIControl(file, options.outputPath, options.baseAssemblyPath, options.game, options.verbose);
                                }
                                if (ext == ".gfx")
                                {
                                    UIControlWriter.WriteUIControl(file, options.outputPath, options.baseAssemblyPath, options.game, options.verbose);
                                }
                            }
                        }
                        else
                        {
                            string ext = Path.GetExtension(options.inputPath).ToLower();
                            if (ext == ".swf")
                            {
                                ScaleformGFxWriter.WriteScaleformGfX(options.inputPath, options.outputPath, options.gfxexportPath, options.baseAssemblyPath, options.game, options.verbose);
                                UIControlWriter.WriteUIControl(options.inputPath, options.outputPath, options.baseAssemblyPath, options.game, options.verbose);
                            }
                            if (ext == ".gfx")
                            {
                                UIControlWriter.WriteUIControl(options.inputPath, options.outputPath, options.baseAssemblyPath, options.game, options.verbose);
                            }
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
                        File.WriteAllLines(Path.Combine(options.outputPath, "aspect.txt"), logAspectPaths);
                    }

                });
        }
    }
}