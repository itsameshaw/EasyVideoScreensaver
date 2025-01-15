using EasyVideoScreensaver.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EasyVideoScreensaver
{
    public class VLCValidator
    {
        public static bool IsValidVLCInstallation(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            try
            {
                var directoryInfo = new System.IO.DirectoryInfo(path);

                var requiredFiles = new List<string>()
                {
                    "libvlc.dll",
                    "libvlccore.dll"
                };
                if (!directoryInfo.Exists || requiredFiles.Count(file => { return directoryInfo.ContainsFile(file); }) != requiredFiles.Count)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
