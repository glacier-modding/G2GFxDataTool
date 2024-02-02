using System.Security.AccessControl;
using System.Text;

namespace G2GFxDataTool
{
    internal class Helpers
    {
        internal static string ConvertStringtoMD5(string inputString)
        {
            // from RPKG-Tool https://github.com/glacier-modding/RPKG-Tool/blob/dd86901f4a3a64b2a358a4ea607fa144e7fc2cfa/rpkg-gui/HashCalculator.xaml.cs#L94

            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

            inputString = inputString.Trim().ToLower().Replace("\\", "/");

            byte[] stringBytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hashBytes = md5.ComputeHash(stringBytes);

            StringBuilder ioiHashsb = new StringBuilder("00", 16);
            for (int i = 1; i < 8; i++)
            {
                ioiHashsb.Append(hashBytes[i].ToString("X2"));
            }
            return ioiHashsb.ToString();
        }

        internal static string AssemblyPathDeriver(string inputPath, string resourceType)
        {
            string searchTerm = "\\construction\\";
            int index = inputPath.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase);

            if (index != -1)
            {
                string assemblyPath = inputPath.Substring(index + searchTerm.Length).Replace('\\', '/');

                assemblyPath = "[assembly:/" + assemblyPath + "].pc_" + resourceType;

                return assemblyPath.ToLower();
            }

            return inputPath + ".pc_" + resourceType;
        }

        internal static string UIControlPathDeriver(string inputPath, string className)
        {
            {
                string searchTerm = "\\construction\\";
                int index = inputPath.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase);

                if (index != -1)
                {
                    string assemblyPath = inputPath.Substring(index + searchTerm.Length).Replace('\\', '/');

                    assemblyPath = $"[assembly:/{assemblyPath}?/{className}.uic].pc";

                    return assemblyPath.ToLower();
                }

                return inputPath + ".pc_";
            }
        }

    }
}
