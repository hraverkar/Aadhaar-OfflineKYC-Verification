namespace Aadhaar_OfflineKYC_Verification.Models
{
    public class ResponseDto(string id, string message, string data)
    {

        public string Id { get; } = id;
        public string Message { get; } = message;
        public string Data { get; } = data;
    }
}
