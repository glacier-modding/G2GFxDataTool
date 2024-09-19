using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace G2GFxDataTool
{
    internal class ScaleformGFxWriter
    {
        internal static void WriteScaleformGfX(string inputPath, string outputPath, string gfxexportPath, string baseAssemblyPath, ResourceLib.Game game, bool verbose)
        {
            string gfxFileName = Path.GetFileNameWithoutExtension(inputPath);
            string tempFolderPath = Path.GetTempPath();

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = gfxexportPath,
                Arguments = $"\"{inputPath}\" -d {tempFolderPath} -list -lwr -i DDS",
                CreateNoWindow = true,
                RedirectStandardOutput = verbose,
                UseShellExecute = false
            };

            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    if (verbose)
                    {
                        Console.WriteLine(process.StandardOutput.ReadToEnd());
                    }
                    process.WaitForExit();
                    
                    if (process.ExitCode != 0)
                    {
                        Console.WriteLine("GFxExport failed to run. Please install Microsoft Visual C++ 2010 Redistributable x64 from https://www.microsoft.com/en-au/download/details.aspx?id=26999");
                        Environment.Exit(process.ExitCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            var textureFileNamesList = File.ReadAllLines(Path.Combine(tempFolderPath, gfxFileName + ".lst"));
            var textureFileNames = new List<string>();
            var textureData = new List<string>();

            string gfxFile = Path.Combine(tempFolderPath, gfxFileName + ".gfx");
            string gfxFileData = Convert.ToBase64String(File.ReadAllBytes(gfxFile));

            foreach (var textureFileName in textureFileNamesList)
            {
                if (textureFileName.EndsWith(".gfx")) // Some versions of GFxExport include the .gfx file itself in the list file.
                {
                    continue;
                }

                string texturePath = Path.Combine(tempFolderPath, textureFileName);
                if (File.Exists(texturePath))
                {
                    textureFileNames.Add(Path.GetFileName(textureFileName));
                    textureData.Add(Convert.ToBase64String(File.ReadAllBytes(texturePath)));
                }
            }

            var jsonOutput = new
            {
                m_pSwfData = gfxFileData,
                m_pAdditionalFileNames = textureFileNames,
                m_pAdditionalFileData = textureData
            };

            string gfxfJSON = JsonSerializer.Serialize(jsonOutput);

            var s_Generator = new ResourceLib.ResourceGenerator("GFXF", game);
            var s_ResourceMem = s_Generator.FromJsonStringToResourceMem(gfxfJSON);

            //string assemblyPath = Helpers.AssemblyPathDeriver(inputPath, "swf");
            string assemblyPath = "[assembly:" + baseAssemblyPath + Path.GetFileNameWithoutExtension(inputPath) + ".swf" + "].pc_swf";
            string assemblyPathHash = Helpers.ConvertStringtoMD5(assemblyPath);

            Program.logScaleformGFxPaths.Add(assemblyPathHash + ".GFXF," + assemblyPath);

            MetaFiles.MetaData gfxfMetaData = new MetaFiles.MetaData();
            gfxfMetaData.id = assemblyPathHash;
            gfxfMetaData.type = "GFXF";
            gfxfMetaData.compressed = true;
            gfxfMetaData.scrambled = true;

            MetaFiles.GenerateMeta(ref gfxfMetaData, Path.Combine(outputPath, assemblyPathHash + ".GFXF.metadata.json"));

            if (verbose)
            {
                Console.WriteLine("Saving GFXF file as '" + Path.Combine(outputPath, assemblyPathHash + ".GFXF'"));
            }

            File.WriteAllBytes(Path.Combine(outputPath, assemblyPathHash + ".GFXF"), s_ResourceMem);

            // Cleanup temp files
            try
            {
                if (verbose)
                {
                    Console.WriteLine("\r\nCleaning up temporary files:\r\n" + gfxFile + "\r\n" + Path.Combine(tempFolderPath, gfxFileName + ".lst"));
                }

                File.Delete(gfxFile);

                File.Delete(Path.Combine(tempFolderPath, gfxFileName + ".lst"));

                foreach (var textureFileName in textureFileNamesList)
                {
                    string texturePath = Path.Combine(tempFolderPath, textureFileName);
                    if (File.Exists(texturePath))
                    {
                        if (verbose)
                        {
                            Console.WriteLine(texturePath);
                        }
                        File.Delete(texturePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cleaning up temporary files: {ex}");
            }

        }
    }
}
