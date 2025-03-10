using Aadhaar_OfflineKYC_Verification.Models;
using Microsoft.AspNetCore.Http;

namespace Aadhaar_OfflineKYC_Verification.Interfaces
{
    public interface IAadhaarKYCService
    {
        Task<ResponseDto> ProcessAadhaarFileAsync(IFormFile file, string shareCode);
        bool VerifySignature(string xmlContent);
        KYCUserInformation ExtractKYCUserInformation(string xmlContent);
        KYCData ExtractKYCData(string xmlContent);
    }
}
