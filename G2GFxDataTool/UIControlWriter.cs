using System.Text.Json;

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

                string uictAssemblyPath = "[assembly:" + baseAssemblyPath + Path.GetFileNameWithoutExtension(inputPath) + ".swf?/" + definition.className + ".uic].pc_entitytype";
                string uicbAssemblyPath = "[assembly:" + baseAssemblyPath + Path.GetFileNameWithoutExtension(inputPath) + ".swf?/" + definition.className + ".uic].pc_entityblueprint";

                string uictAssemblyPathHash = Helpers.ConvertStringtoMD5(uictAssemblyPath);
                string uicbAssemblyPathHash = Helpers.ConvertStringtoMD5(uicbAssemblyPath);

                Program.logUIControlPaths.Add(definition.className + ":");
                Program.logUIControlPaths.Add(uictAssemblyPathHash + ".UICT," + uictAssemblyPath);
                Program.logUIControlPaths.Add(uicbAssemblyPathHash + ".UICB," + uicbAssemblyPath + "\r\n");

                MetaFiles.MetaData uictMetaData = new MetaFiles.MetaData();
                uictMetaData.id = uictAssemblyPathHash;
                uictMetaData.type = "UICT";
                uictMetaData.compressed = false;
                uictMetaData.scrambled = true;
                uictMetaData.references.Add(new
                {
                    resource = "002C4526CC9753E6",
                    type = "install"
                });
                uictMetaData.references.Add(new
                {
                    resource = uicbAssemblyPathHash,
                    type = "install"
                });

                MetaFiles.MetaData uicbMetaData = new MetaFiles.MetaData();
                uicbMetaData.id = uicbAssemblyPathHash;
                uicbMetaData.type = "UICB";
                uicbMetaData.compressed = true;
                uicbMetaData.scrambled = true;
                uicbMetaData.references.Add(new
                {
                    resource = "00578D925459143F",
                    type = "install"
                });

                MetaFiles.GenerateMeta(ref uictMetaData, Path.Combine(outputPath, uictAssemblyPathHash + ".UICT.metadata.json"));
                MetaFiles.GenerateMeta(ref uicbMetaData, Path.Combine(outputPath, uicbAssemblyPathHash + ".UICB.metadata.json"));

                string asetAssemblyPath = "[assembly:/templates/aspectdummy.aspect]([assembly:" + baseAssemblyPath + Path.GetFileNameWithoutExtension(inputPath) + ".swf?/" + definition.className + ".uic].entitytype,[modules:/zuicontrollayoutlegacyaspect.class].entitytype).pc_entitytype";
                string asebAssemblyPath = "[assembly:/templates/aspectdummy.aspect]([assembly:" + baseAssemblyPath + Path.GetFileNameWithoutExtension(inputPath) + ".swf?/" + definition.className + ".uic].entitytype,[modules:/zuicontrollayoutlegacyaspect.class].entitytype).pc_entityblueprint";

                string asetAssemblyPathHash = Helpers.ConvertStringtoMD5(asetAssemblyPath);
                string asebAssemblyPathHash = Helpers.ConvertStringtoMD5(asebAssemblyPath);

                Program.logAspectPaths.Add(definition.className + ":");
                Program.logAspectPaths.Add(asetAssemblyPathHash + ".ASET," + asetAssemblyPath);
                Program.logAspectPaths.Add(asebAssemblyPathHash + ".ASEB," + asebAssemblyPath + "\r\n");

                MetaFiles.MetaData asetMetaData = new MetaFiles.MetaData();
                asetMetaData.id = asetAssemblyPathHash;
                asetMetaData.type = "ASET";
                asetMetaData.references.Add(new
                {
                    resource = uictAssemblyPathHash,
                    type = "install"
                });
                asetMetaData.references.Add(new
                {
                    resource = "002F0C25E6E34D14",
                    type = "install"
                });
                asetMetaData.references.Add(new
                {
                    resource = asebAssemblyPathHash,
                    type = "install"
                });

                MetaFiles.MetaData asebMetaData = new MetaFiles.MetaData();
                asebMetaData.id = asebAssemblyPathHash;
                asebMetaData.type = "ASEB";
                asebMetaData.references.Add(new
                {
                    resource = uicbAssemblyPathHash,
                    type = "install"
                });
                asebMetaData.references.Add(new
                {
                    resource = "00B388AB33E1DAC0",
                    type = "install"
                });

                MetaFiles.GenerateMeta(ref asetMetaData, Path.Combine(outputPath, asetAssemblyPathHash + ".ASET.metadata.json"));
                MetaFiles.GenerateMeta(ref asebMetaData, Path.Combine(outputPath, asebAssemblyPathHash + ".ASEB.metadata.json"));

                byte[] asetBytes = Helpers.GenerateAspect(asetMetaData.references.Count());
                File.WriteAllBytes(Path.Combine(outputPath, asetAssemblyPathHash + ".ASET"), asetBytes);

                byte[] asebBytes = Helpers.GenerateAspect(asebMetaData.references.Count());
                File.WriteAllBytes(Path.Combine(outputPath, asebAssemblyPathHash + ".ASEB"), asebBytes);

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
