using SwfOp;
using SwfOp.Data;

namespace G2GFxDataTool
{
    internal class ParseSWF
    {
        internal class ASClass
        {
            public string className { get; set; }

            public List<ASClassProperties> classProperties { get; set; }

            public List<ASClassMethods> classMethods { get; set; }
        }

        internal struct ASClassProperties
        {
            public string classPropertyName { get; set; }

            public string classPropertyType { get; set; }
        }

        internal struct ASClassMethods
        {
            public string methodName { get; set; }

            public List<string> argumentTypes { get; set; }

            public string returnType { get; set; }
        }

        internal static List<ASClass> ParseAS(string filename)
        {
            ContentParser parser = new ContentParser(filename);
            parser.Run();
            List<ASClass> classDefinitions = new List<ASClass>();

            foreach (Abc abc in parser.Abcs)
            {
                foreach (Traits trait in abc.classes.Where(t => t.itraits != null))
                {
                    List<Traits> hierarchy = new List<Traits>();
                    Traits currentTrait = trait;
                    do
                    {
                        hierarchy.Add(currentTrait);
                        currentTrait = abc.classes.FirstOrDefault(t => t.itraits != null && t.itraits.name == currentTrait.itraits.baseName);
                    }
                    while (currentTrait != null);

                    if (hierarchy.Any(t => t.itraits.name.ToTypeString().EndsWith("BaseControl")))
                    {
                        var relevantTraits = hierarchy.Where(t => !t.itraits.name.ToTypeString().EndsWith("BaseControl"));
                        var members = relevantTraits.SelectMany(t => t.itraits.members);

                        ASClass def = new ASClass
                        {
                            className = trait.itraits.name.ToTypeString(),
                            classProperties = new List<ASClassProperties>(),
                            classMethods = new List<ASClassMethods>()
                        };

                        foreach (MemberInfo info in members)
                        {
                            if (info.name.uri == "private")
                            {
                                continue;
                            }
                            if (info is MethodInfo)
                            {
                                MethodInfo method = info as MethodInfo;
                                if (method.kind == TraitMember.Method)
                                {
                                    ASClassMethods minfo = new ASClassMethods
                                    {
                                        methodName = method.name.localName,
                                        argumentTypes = new List<string>(method.paramTypes.Select((QName q) => q.ToString())),
                                        returnType = method.returnType.ToString()
                                    };
                                    def.classMethods.Add(minfo);
                                }
                                if (method.kind == TraitMember.Setter)
                                {
                                    ASClassProperties property = new ASClassProperties
                                    {
                                        classPropertyName = method.name.localName,
                                        classPropertyType = method.paramTypes[0].ToTypeString()
                                    };
                                    def.classProperties.Add(property);
                                }
                            }
                        }

                        classDefinitions.Add(def);
                    }
                }
            }

            return classDefinitions;
        }
    }
}
