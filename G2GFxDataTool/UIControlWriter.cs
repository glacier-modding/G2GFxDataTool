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
            public List<Pin> m_aPins { get; set; }
            public List<Property> m_aProperties { get; set; }

            public UICBData()
            {
                m_aPins = new List<Pin>();
                m_aProperties = new List<Property>();
            }
        }

        internal class Pin
        {
            public string m_eKind { get; set; } = "E_ATTRIBUTE_KIND_INPUT_PIN";
            public string m_eType { get; set; }
            public string m_sName { get; set; }
        }

        internal class Property
        {
            public string m_eKind { get; set; } = "E_ATTRIBUTE_KIND_PROPERTY";
            public string m_eType { get; set; }
            public string m_sName { get; set; }
            public int m_nPropertyOffset { get; set; } = 0;
            public int m_nPropertyId { get; set; } = 0;
        }

        internal static void WriteUIControl(string inputPath, string outputPath, string baseAssemblyPath, bool verbose)
        {
            var definitions = ParseSWF.ParseAS(inputPath);

            foreach (var definition in definitions)
            {
                UICBData data = new UICBData();

                if (verbose)
                {
                    Console.WriteLine("\r\nFound class: " + definition.className + ":");
                }

                foreach (var method in definition.classMethods)
                {

                    if (method.argumentTypes.Count > 1 || (method.argumentTypes.Count == 1 && !typeMapping.ContainsKey(method.argumentTypes[0])))
                    {
                        continue;
                    }

                    Pin pins = new Pin
                    {
                        m_sName = method.methodName,
                        m_eKind = "E_ATTRIBUTE_KIND_INPUT_PIN",
                        m_eType = "E_ATTRIBUTE_TYPE_VOID",
                    };

                    if (Helpers.IsOutputPin(method.methodName, out string pinName))
                    {
                        pins.m_sName = pinName;
                        pins.m_eKind = "E_ATTRIBUTE_KIND_OUTPUT_PIN";
                    }

                    if (method.argumentTypes.Count == 1)
                    {
                        pins.m_eType = typeMapping.GetValueOrDefault(method.argumentTypes[0]);
                    }

                    if (verbose)
                    {
                        Console.WriteLine("\tFound pin: " + pins.m_sName + " type: " + pins.m_eType + " kind: " + pins.m_eKind);
                    }

                    data.m_aPins.Add(pins);
                }

                foreach (var property in definition.classProperties)
                {
                    Property properties = new Property
                    {
                        m_sName = property.classPropertyName,
                        m_eKind = "E_ATTRIBUTE_KIND_PROPERTY",
                        m_eType = typeMapping.GetValueOrDefault(property.classPropertyType),
                        m_nPropertyOffset = 0,
                        m_nPropertyId = 0
                    };

                    if (verbose)
                    {
                        Console.WriteLine("\tFound property: " + property.classPropertyName + " type: " + property.classPropertyType);
                    }

                    data.m_aProperties.Add(properties);
                }

                //string uictAssemblyPath = Helpers.UIControlPathDeriver(inputPath, definition.className) + "entitytype";
                //string uicbAssemblyPath = Helpers.UIControlPathDeriver(inputPath, definition.className) + "entityblueprint";
                string uictAssemblyPath = "[assembly:" + baseAssemblyPath + Path.GetFileNameWithoutExtension(inputPath) + ".swf?/" + definition.className + ".uic].pc_entitytype";
                string uicbAssemblyPath = "[assembly:" + baseAssemblyPath + Path.GetFileNameWithoutExtension(inputPath) + ".swf?/" + definition.className + ".uic].pc_entityblueprint";

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

                if (verbose)
                {
                    Console.WriteLine("Saving UICT file as '" + Path.Combine(outputPath, uictAssemblyPathHash + ".UICT"));
                }

                File.Create(Path.Combine(outputPath, uictAssemblyPathHash + ".UICT"));

                if (verbose)
                {
                    Console.WriteLine("Saving UICB file as '" + Path.Combine(outputPath, uicbAssemblyPathHash + ".UICB.json"));
                }

                File.WriteAllText(Path.Combine(outputPath, uicbAssemblyPathHash + ".UICB.json"), jsonData);
            }
        }
    }
}
