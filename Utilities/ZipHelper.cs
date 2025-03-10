using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aadhaar_OfflineKYC_Verification.Utilities
{
    public class ZipHelper
    {
        public static void ExtractZip(string zipPath, string extractPath, string password)
        {
            using var fileStream = File.OpenRead(zipPath);
            using var zipInputStream = new ZipInputStream(fileStream) { Password = password };

            ZipEntry entry;
            while ((entry = zipInputStream.GetNextEntry()) != null)
            {
                string entryPath = Path.Combine(extractPath, entry.Name);

                if (entry.IsDirectory)
                {
                    Directory.CreateDirectory(entryPath);
                    continue;
                }

                using var outputStream = File.Create(entryPath);
                zipInputStream.CopyTo(outputStream);
            }
        }

        public static void CleanupFiles(string zipFilePath, string extractPath)
        {
            if (File.Exists(zipFilePath))
                File.Delete(zipFilePath);

            if (Directory.Exists(extractPath))
                Directory.Delete(extractPath, true);
        }
    }
}
