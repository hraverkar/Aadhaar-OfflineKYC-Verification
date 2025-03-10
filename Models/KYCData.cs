namespace Aadhaar_OfflineKYC_Verification.Models
{
    public class KYCData
    {
        public string? Issuer { get; set; }
        public string? Subject { get; set; }
        public DateTime? Notbefore { get; set; }
        public DateTime? Notafter { get; set; }
        public string? GetPublicKey { get; set; }
        public string? SerialNumber { get; set; }
        public string? SignatureAlgoName { get; set; }
    }
}
