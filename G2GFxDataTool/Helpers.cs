using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G2GFxDataTool
{
    internal class Helpers
    {
        public static string ConvertStringtoMD5(string inputString)
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

        public static string AssemblyPathDeriver(string inputPath, string resourceType)
        {
            string searchTerm = "\\construction\\";
            int index = inputPath.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase);

            if (index != -1)
            {
                string assemblyPath = inputPath.Substring(index + searchTerm.Length).Replace('\\', '/');

                assemblyPath = "[assembly:/" + assemblyPath + "].pc_" + resourceType;

                return assemblyPath;
            }

            return inputPath + ".pc_" + resourceType;
        }
    }
}
