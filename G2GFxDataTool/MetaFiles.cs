using System.Text.Json;

namespace G2GFxDataTool
{
    public class MetaFiles
    {
        public class MetaData
        {
            public string id { get; set; } = "";
            public string type { get; set; } = "";
            public bool compressed { get; set; } = false;
            public bool scrambled { get; set; } = false;
            public List<object> references { get; set; } = new List<object>();
        }

        internal static class JsonSerializerOptionsProvider
        {
            public static JsonSerializerOptions Options { get; }

            static JsonSerializerOptionsProvider()
            {
                Options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
            }
        }

        internal static void GenerateMeta(ref MetaData meta, string outputPath)
        {
            string MetaData = JsonSerializer.Serialize(meta, JsonSerializerOptionsProvider.Options);

            System.IO.File.WriteAllText(outputPath, MetaData);
        }
    }
}
