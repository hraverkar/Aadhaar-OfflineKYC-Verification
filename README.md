# Aadhaar Offline KYC Verification

## Description
This NuGet package enables secure and efficient Offline Aadhaar KYC Verification for Indian citizens. It allows developers to extract and validate Aadhaar KYC data from the ZIP file provided by UIDAI's offline Aadhaar e-KYC service. The package handles ZIP decryption using the share code, extracts XML data, verifies the digital signature, and parses relevant user information like name, address, date of birth, and more.

## Key Features
- ✔️ **Secure Aadhaar KYC Verification** – Extracts and validates Aadhaar KYC data from UIDAI's offline ZIP.
- ✔️ **Digital Signature Validation** – Ensures data authenticity using X.509 certificate verification.
- ✔️ **ZIP Extraction with Password** – Securely extracts XML from the Aadhaar ZIP using the share code.
- ✔️ **KYC Data Extraction** – Parses user details such as name, DOB, address, and photograph.
- ✔️ **Easy Integration** – Lightweight package with a simple API for seamless integration in .NET applications.
- ✔️ **Error Handling & Logging** – Robust error handling and logging for better debugging.

## How to Use it
Register as dependency injection on program.cs

    // Progarm.cs
	builder.Services.AddSingleton<IAadhaarKYCService, AadharKYCService>();
	
	Normal, any class where need to do dependency indejection.
    public class KYCVerificationCommandHandler(IAadhaarKYCService aadharKYCService)
    {
        private readonly IAadhaarKYCService _aadharKYCService = aadharKYCService;
    }
    
    # here File is type of IFile and Password As String
    public sampleClass()
    {
        var result =   await _aadharKYCService.ProcessAadhaarFileAsync(File, Password);
        var isValidSignature = _aadharKYCService.VerifySignature(result.Data);
        var KYCUserInformation = _aadharKYCService.ExtractKYCUserInformation(result.Data);
        var KYCCertificateInformation = _aadharKYCService.ExtractKYCData(result.Data);
    }

## Usage Scenarios
- Implement Aadhaar-based KYC verification in banking, finance, telecom, and e-commerce applications.
- Automate KYC verification in digital onboarding processes.
- Verify Aadhaar authenticity for regulatory compliance.

## Supported Frameworks
- .NET 6.0 and above
- ASP.NET Core applications
- Console applications