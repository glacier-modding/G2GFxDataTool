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

        internal static void WriteUIControl(string inputPath, string outputPath)
        {
            var definitions = ParseSWF.ParseAS(inputPath);

            foreach (var definition in definitions)
            {
                var uicbData = new
                {
                    m_aPins = definition.classMethods.Select(method => new
                    {
                        m_eKind = "E_ATTRIBUTE_KIND_INPUT_PIN",
                        m_eType = method.argumentTypes != null && method.argumentTypes.Count == 1
                          ? typeMapping.GetValueOrDefault(method.argumentTypes[0], "E_ATTRIBUTE_TYPE_VOID")
                          : "E_ATTRIBUTE_TYPE_VOID",
                        m_sName = method.methodName
                    }).ToList(),
                    m_aProperties = definition.classProperties.Select(prop => new
                    {
                        eKind = "E_ATTRIBUTE_KIND_PROPERTY",
                        eType = typeMapping.GetValueOrDefault(prop.classPropertyType, "E_ATTRIBUTE_TYPE_OBJECT"),
                        m_sName = prop.classPropertyName,
                        m_nPropertyOffset = 0,
                        m_nPropertyId = 0
                    }).ToList()
                };


                string uictAssemblyPath = Helpers.UIControlPathDeriver(inputPath, definition.className) + "entitytype";
                string uicbAssemblyPath = Helpers.UIControlPathDeriver(inputPath, definition.className) + "entityblueprint";

                string uictAssemblyPathHash = Helpers.ConvertStringtoMD5(uictAssemblyPath);
                string uicbAssemblyPathHash = Helpers.ConvertStringtoMD5(uicbAssemblyPath);

                Program.logUIControlPaths.Add(uictAssemblyPathHash + ".UICT," + uictAssemblyPath);
                Program.logUIControlPaths.Add(uicbAssemblyPathHash + ".UICB," + uicbAssemblyPath);

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

                string jsonData = JsonSerializer.Serialize(uicbData);

                File.Create(Path.Combine(outputPath, uictAssemblyPathHash + ".UICT"));
                File.WriteAllText(Path.Combine(outputPath, uicbAssemblyPathHash + ".UICB.json"), jsonData);
            }
        }
    }
}
