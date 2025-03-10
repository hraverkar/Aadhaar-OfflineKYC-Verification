using Aadhaar_OfflineKYC_Verification.Interfaces;
using Aadhaar_OfflineKYC_Verification.Models;
using Aadhaar_OfflineKYC_Verification.Utilities;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Aadhaar_OfflineKYC_Verification.Services
{
    public class AadharKYCService : IAadhaarKYCService
    {

        public async Task<ResponseDto> ProcessAadhaarFileAsync(IFormFile file, string shareCode)
        {
            if (file == null || file.Length == 0)
                return new ResponseDto(Guid.NewGuid().ToString(), "Error: Please upload a valid ZIP file", string.Empty);

            string extractPath = Path.Combine(Path.GetTempPath(), "ExtractedZip");
            string zipFilePath = Path.Combine(Path.GetTempPath(), file.FileName);

            try
            {
                Directory.CreateDirectory(extractPath);

                using (var stream = new FileStream(zipFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                ZipHelper.ExtractZip(zipFilePath, extractPath, shareCode);

                var xmlFilePath = Directory.GetFiles(extractPath, "*.xml").FirstOrDefault();
                if (xmlFilePath == null)
                    return new ResponseDto(Guid.NewGuid().ToString(), "No XML file found in ZIP.", string.Empty);

                string xmlContent = await File.ReadAllTextAsync(xmlFilePath, Encoding.UTF8);
                return new ResponseDto(Guid.NewGuid().ToString(), "XML File Processed Successfully", xmlContent);
            }
            catch (Exception ex)
            {
                return new ResponseDto(Guid.NewGuid().ToString(), $"Error processing file: {ex.Message}", string.Empty);
            }
            finally
            {
                ZipHelper.CleanupFiles(zipFilePath, extractPath);
            }
        }

        public bool VerifySignature(string xmlContent)
        {
            return XmlHelper.VerifySignature(xmlContent);
        }

        public KYCUserInformation ExtractKYCUserInformation(string xmlContent)
        {
            return XmlHelper.ExtractKYCUserInformation(xmlContent);
        }

        public KYCData ExtractKYCData(string xmlContent)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return XmlHelper.ExtracteKYCData(xmlContent);
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
