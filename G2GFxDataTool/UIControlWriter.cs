using System.Text.Json;

namespace G2GFxDataTool
{
    internal class UIControlWriter
    {
        internal static Dictionary<string, string> typeMapping = new Dictionary<string, string>()
        {
            { "void", "E_ATTRIBUTE_TYPE_VOID" },
            { "int", "E_ATTRIBUTE_TYPE_INT" },
            { "Number", "E_ATTRIBUTE_TYPE_FLOAT" },
            { "String", "E_ATTRIBUTE_TYPE_STRING" },
            { "Boolean", "E_ATTRIBUTE_TYPE_BOOL" },
            { "Object", "E_ATTRIBUTE_TYPE_OBJECT" }
        };

        internal class UICBData
        {
            public List<Pin> Pins { get; set; }
            public List<Property> Properties { get; set; }

            public UICBData()
            {
                Pins = new List<Pin>();
                Properties = new List<Property>();
            }
        }

        internal class Pin
        {
            public string Name { get; set; }
            public string m_eKind { get; set; } = "E_ATTRIBUTE_KIND_INPUT_PIN";
            public string m_eType { get; set; }
        }

        internal class Property
        {
            public string Name { get; set; }
            public string m_eKind { get; set; } = "E_ATTRIBUTE_KIND_PROPERTY";
            public string m_eType { get; set; }
            public int m_nPropertyOffset { get; set; } = 0;
            public int m_nPropertyId { get; set; } = 0;
        }

        internal static void WriteUIControl(string inputPath, string outputPath)
        {
            var definitions = ParseSWF.ParseAS(inputPath);

            foreach (var definition in definitions)
            {
                UICBData data = new UICBData();

                foreach (var method in definition.classMethods)
                {

                    if (method.argumentTypes.Count > 1 || (method.argumentTypes.Count == 1 && !typeMapping.ContainsKey(method.argumentTypes[0])))
                    {
                        continue;
                    }

                    Pin pins = new Pin
                    {
                        Name = method.methodName,
                        m_eKind = "E_ATTRIBUTE_KIND_INPUT_PIN",
                        m_eType = "E_ATTRIBUTE_TYPE_VOID",
                    };

                    if (Helpers.IsOutputPin(method.methodName, out string pinName))
                    {
                        pins.Name = pinName;
                        pins.m_eKind = "E_ATTRIBUTE_KIND_OUTPUT_PIN";
                    }

                    if (method.argumentTypes.Count == 1)
                    {
                        pins.m_eType = typeMapping.GetValueOrDefault(method.argumentTypes[0]);
                    }

                    data.Pins.Add(pins);
                }

                foreach (var property in definition.classProperties)
                {
                    Property properties = new Property
                    {
                        Name = property.classPropertyName,
                        m_eKind = "E_ATTRIBUTE_KIND_PROPERTY",
                        m_eType = typeMapping.GetValueOrDefault(property.classPropertyType),
                        m_nPropertyOffset = 0,
                        m_nPropertyId = 0
                    };

                    data.Properties.Add(properties);
                }

                string uictAssemblyPath = Helpers.UIControlPathDeriver(inputPath, definition.className) + "entitytype";
                string uicbAssemblyPath = Helpers.UIControlPathDeriver(inputPath, definition.className) + "entityblueprint";

                string uictAssemblyPathHash = Helpers.ConvertStringtoMD5(uictAssemblyPath);
                string uicbAssemblyPathHash = Helpers.ConvertStringtoMD5(uicbAssemblyPath);

                Program.logUIControlPaths.Add(definition.className + ":");
                Program.logUIControlPaths.Add(uictAssemblyPathHash + ".UICT," + uictAssemblyPath);
                Program.logUIControlPaths.Add(uicbAssemblyPathHash + ".UICB," + uicbAssemblyPath + "\r\n");

                MetaFiles.MetaData uictMetaData = new MetaFiles.MetaData();
                uictMetaData.hashValue = uictAssemblyPathHash;
                uictMetaData.hashOffset = 22219579;
                uictMetaData.hashSize = 2147483648;
                uictMetaData.hashResourceType = "UICT";
                uictMetaData.hashReferenceTableSize = 22;
                uictMetaData.hashReferenceTableDummy = 0;
                uictMetaData.hashSizeFinal = 0;
                uictMetaData.hashSizeInMemory = 4294967295;
                uictMetaData.hashSizeInVideoMemory = 4294967295;
                uictMetaData.hashReferenceData.Add(new
                {
                    hash = "[modules:/zuicontrolentity.class].pc_entitytype",
                    flag = "1F"
                });
                uictMetaData.hashReferenceData.Add(new
                {
                    hash = uicbAssemblyPath,
                    flag = "1F"
                });

                MetaFiles.MetaData uicbMetaData = new MetaFiles.MetaData();
                uicbMetaData.hashValue = uicbAssemblyPathHash;
                uictMetaData.hashOffset = 22219511;
                uictMetaData.hashSize = 2147483716;
                uictMetaData.hashResourceType = "UICB";
                uictMetaData.hashReferenceTableSize = 13;
                uictMetaData.hashReferenceTableDummy = 0;
                uictMetaData.hashSizeFinal = 114;
                uictMetaData.hashSizeInMemory = 62;
                uictMetaData.hashSizeInVideoMemory = 4294967295;
                uicbMetaData.hashReferenceData.Add(new
                {
                    hash = "[modules:/zuicontrolentity.class].pc_entityblueprint",
                    flag = "1F"
                });

                MetaFiles.GenerateMeta(ref uictMetaData, Path.Combine(outputPath, uictAssemblyPathHash + ".UICT.meta.json"));
                MetaFiles.GenerateMeta(ref uicbMetaData, Path.Combine(outputPath, uicbAssemblyPathHash + ".UICB.meta.json"));

                string jsonData = JsonSerializer.Serialize(data);

                File.Create(Path.Combine(outputPath, uictAssemblyPathHash + ".UICT"));
                File.WriteAllText(Path.Combine(outputPath, uicbAssemblyPathHash + ".UICB.json"), jsonData);
            }
        }
    }
}
