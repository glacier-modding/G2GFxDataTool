﻿using System.Text.Json;

namespace G2GFxDataTool
{
    internal class UIControlWriter
    {
        internal static Dictionary<string, string> typeMapping = new Dictionary<string, string>()
        {
            { "void", "E_ATTRIBUTE_TYPE_VOID" },
            { "int", "E_ATTRIBUTE_TYPE_INT" },
            { "uint", "E_ATTRIBUTE_TYPE_INT" },
            { "Number", "E_ATTRIBUTE_TYPE_FLOAT" },
            { "String", "E_ATTRIBUTE_TYPE_STRING" },
            { "Boolean", "E_ATTRIBUTE_TYPE_BOOL" },
            { "Object", "E_ATTRIBUTE_TYPE_OBJECT" }
        };

        internal class UICBData
        {
            public List<Attributes> m_aAttributes { get; set; }
            public List<string> m_aSpecialMethods { get; set; }

            public UICBData()
            {
                m_aAttributes = new List<Attributes>();
                m_aSpecialMethods = new List<string>();
            }
        }

        internal class Attributes
        {
            public string m_eKind { get; set; }
            public string m_eType { get; set; }
            public string m_sName { get; set; }
        }

        internal static void WriteUIControl(string inputPath, string outputPath, string baseAssemblyPath, ResourceLib.Game game, bool verbose)
        {
            var definitions = ParseSWF.ParseAS(inputPath);

            string[] SPECIAL_METHODS = ["onAttached", "onChildrenAttached", "onSetData", "onSetSize", "onSetViewport", "onSetVisible", "onSetSelected", "onSetFocused", "onSelectedIndexChanged", "Count"];

            foreach (var definition in definitions)
            {
                UICBData data = new UICBData();

                if (verbose)
                {
                    Console.WriteLine("\r\nFound class: " + definition.className + ":");
                }

                foreach (var method in definition.classMethods)
                {
                    if (SPECIAL_METHODS.Contains(method.methodName))
                    {
                        if (!data.m_aSpecialMethods.Contains(method.methodName))
                        {
                            data.m_aSpecialMethods.Add(method.methodName);

                            if (verbose)
                            {
                                Console.WriteLine("\tFound special method: " + method.methodName);
                            }
                        }
                        continue;
                    }

                    if (method.argumentTypes.Count > 1 || (method.argumentTypes.Count == 1 && !typeMapping.ContainsKey(method.argumentTypes[0])))
                    {
                        continue;
                    }

                    Attributes pins = new Attributes
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

                    data.m_aAttributes.Add(pins);
                }

                foreach (var property in definition.classProperties)
                {
                    Attributes properties = new Attributes
                    {
                        m_sName = property.classPropertyName,
                        m_eKind = "E_ATTRIBUTE_KIND_PROPERTY",
                        m_eType = typeMapping.GetValueOrDefault(property.classPropertyType),
                    };

                    if (verbose)
                    {
                        Console.WriteLine("\tFound property: " + property.classPropertyName + " type: " + property.classPropertyType);
                    }

                    data.m_aAttributes.Add(properties);
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
                uicbMetaData.hashOffset = 22219511;
                uicbMetaData.hashSize = 2147483716;
                uicbMetaData.hashResourceType = "UICB";
                uicbMetaData.hashReferenceTableSize = 13;
                uicbMetaData.hashReferenceTableDummy = 0;
                uicbMetaData.hashSizeFinal = 114;
                uicbMetaData.hashSizeInMemory = 62;
                uicbMetaData.hashSizeInVideoMemory = 4294967295;
                uicbMetaData.hashReferenceData.Add(new
                {
                    hash = "[modules:/zuicontrolentity.class].pc_entityblueprint",
                    flag = "1F"
                });

                MetaFiles.GenerateMeta(ref uictMetaData, Path.Combine(outputPath, uictAssemblyPathHash + ".UICT.meta.json"));
                MetaFiles.GenerateMeta(ref uicbMetaData, Path.Combine(outputPath, uicbAssemblyPathHash + ".UICB.meta.json"));

                string jsonData = JsonSerializer.Serialize(data);

                var s_Generator = new ResourceLib.ResourceGenerator("UICB", game);
                var s_ResourceMem = s_Generator.FromJsonStringToResourceMem(jsonData);

                if (verbose)
                {
                    Console.WriteLine("Saving UICT file as '" + Path.Combine(outputPath, uictAssemblyPathHash + ".UICT"));
                }

                File.Create(Path.Combine(outputPath, uictAssemblyPathHash + ".UICT"));

                if (verbose)
                {
                    Console.WriteLine("Saving UICB file as '" + Path.Combine(outputPath, uicbAssemblyPathHash + ".UICB"));
                }

                File.WriteAllBytes(Path.Combine(outputPath, uicbAssemblyPathHash + ".UICB"), s_ResourceMem);
            }
        }
    }
}
