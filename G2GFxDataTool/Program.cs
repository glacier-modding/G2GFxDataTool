using CommandLine;

namespace G2GFxDataTool
{
    internal class Program
    {
        public static List<string> logPaths = new List<string>();

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
                            foreach (var file in Directory.GetFiles(options.inputPath, "*", SearchOption.AllDirectories))
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
                        File.WriteAllLines(Path.Combine(options.outputPath, "paths.txt"), logPaths);
                    }

                });
        }
    }
}