using System.Diagnostics;
using System.Text.Json;

namespace G2GFxDataTool
{
    internal class ScaleformGFxWriter
    {
        internal static void WriteScaleformGfX(string inputPath, string outputPath, bool verbose)
        {
            string gfxexport = "gfxexport.exe";
            string gfxFileName = Path.GetFileNameWithoutExtension(inputPath);
            string tempFolderPath = Path.GetTempPath();

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = gfxexport,
                Arguments = $"\"{inputPath}\" -d {tempFolderPath} -list -lwr -i DDS",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
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
                if (textureFileName.EndsWith(".gfx")) // Some versions of GFxExport may include the .gfx file itself in the list file.
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

            var s_Generator = new ResourceLib.ResourceGenerator("GFXF", ResourceLib.Game.Hitman3);
            var s_ResourceMem = s_Generator.FromJsonStringToResourceMem(gfxfJSON);

            string assemblyPath = Helpers.AssemblyPathDeriver(inputPath, "swf");
            string assemblyPathHash = Helpers.ConvertStringtoMD5(assemblyPath);

            Program.logScaleformGFxPaths.Add(assemblyPathHash + ".GFXF," + assemblyPath);

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
