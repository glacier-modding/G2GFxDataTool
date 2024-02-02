using CommandLine;

namespace G2GFxDataTool
{
    internal class Program
    {
        public static List<string> errorLogs = new List<string>();

        static void Main(string[] args)
        {
            var parser = Parser.Default.ParseArguments<Options>(args);

            parser
                .WithParsed(options =>
                {
                    if (options.inputPath != null)
                    {
                        string ext = Path.GetExtension(options.inputPath);
                        if (ext == ".swf")
                        {
                            ScaleformGFxWriter.WriteScaleformGfX(options.inputPath, options.outputPath);
                            UIControlWriter.WriteUIControl(options.inputPath, options.outputPath);
                        }
                    }
                });
        }
    }
}