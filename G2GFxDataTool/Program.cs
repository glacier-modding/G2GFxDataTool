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
                        FileAttributes attr = File.GetAttributes(options.inputPath);

                        if (attr.HasFlag(FileAttributes.Directory))
                        {
                            foreach (var file in Directory.GetFiles(options.inputPath))
                            {
                                string ext = Path.GetExtension(file);
                                if (ext == ".swf")
                                {
                                    ScaleformGFxWriter.WriteScaleformGfX(file, options.outputPath);
                                    UIControlWriter.WriteUIControl(file, options.outputPath);
                                }
                            }
                        }
                        else
                        {
                            string ext = Path.GetExtension(options.inputPath);
                            if (ext == ".swf")
                            {
                                ScaleformGFxWriter.WriteScaleformGfX(options.inputPath, options.outputPath);
                                UIControlWriter.WriteUIControl(options.inputPath, options.outputPath);
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
                    }

                });
        }
    }
}