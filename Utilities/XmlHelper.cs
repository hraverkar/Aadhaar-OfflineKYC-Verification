using Aadhaar_OfflineKYC_Verification.Models;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Aadhaar_OfflineKYC_Verification.Utilities
{
    public class XmlHelper
    {
        public static bool VerifySignature(string xmlContent)
        {
            try
            {
                XmlDocument xmlDoc = LoadXmlDocument(xmlContent);
                XmlNamespaceManager nsManager = CreateNamespaceManager(xmlDoc);

                XmlNode? signatureNode = xmlDoc.SelectSingleNode("//ds:Signature", nsManager);
                XmlNode? certNode = xmlDoc.SelectSingleNode("//ds:X509Certificate", nsManager);

                if (signatureNode == null || certNode == null)
                    return false;

                X509Certificate2 cert = GetCertificate(certNode);
                return VerifyXmlSignature(xmlDoc, signatureNode, cert);
            }
            catch
            {
                return false;
            }
        }

        public static KYCUserInformation ExtractKYCUserInformation(string xmlContent)
        {
            var kycData = new KYCUserInformation();
            try
            {
                XmlDocument xmlDoc = LoadXmlDocument(xmlContent);
                XmlNode? poiElement = xmlDoc.SelectSingleNode("//UidData/Poi");
                XmlNode? poaElement = xmlDoc.SelectSingleNode("//UidData/Poa");
                XmlNode? phtElement = xmlDoc.SelectSingleNode("//UidData/Pht");

                if (poiElement != null)
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    kycData.DOB = poiElement.Attributes["dob"]?.Value ?? "N/A";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    kycData.Name = poiElement.Attributes["name"]?.Value ?? "N/A";
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    kycData.Country = poaElement.Attributes["country"]?.Value ?? "N/A";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    kycData.District = poaElement.Attributes["dist"]?.Value ?? "N/A";
                    kycData.State = poaElement.Attributes["state"]?.Value ?? "N/A";
                    kycData.Pincode = poaElement.Attributes["pc"]?.Value ?? "N/A";
                    kycData.PostOffice = poaElement.Attributes["po"]?.Value ?? "N/A";
                    kycData.SubDist = poaElement.Attributes["subdist"]?.Value ?? "N/A";
                    kycData.Address = GetFullAddress(poaElement);
                    kycData.VTC = poiElement.Attributes["vtc"]?.Value ?? "N/A";
                }
                if (phtElement != null)
                {
                    kycData.Photo = phtElement.InnerText.Trim();
                }
            }
            catch (Exception)
            {
            }

            return kycData;
        }

        private static string GetFullAddress(XmlNode poaElement)
        {
            if (poaElement == null) return string.Empty;

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string[] addressParts = new[]
            {
                poaElement.Attributes["house"]?.Value?.Trim(),
                poaElement.Attributes["landmark"]?.Value?.Trim(),
                poaElement.Attributes["street"]?.Value?.Trim()
            };
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

            return string.Join(", ", addressParts.Where(part => !string.IsNullOrEmpty(part)));
        }

        private static XmlDocument LoadXmlDocument(string xmlContent)
        {
            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            xmlDoc.LoadXml(xmlContent);
            return xmlDoc;
        }

        private static XmlNamespaceManager CreateNamespaceManager(XmlDocument xmlDoc)
        {
            var nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            nsManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            return nsManager;
        }

        public static X509Certificate2 GetCertificate(XmlNode certNode)
        {
            byte[] certBytes = Convert.FromBase64String(certNode.InnerText.Trim());
            return new X509Certificate2(certBytes);
        }

        private static bool VerifyXmlSignature(XmlDocument xmlDoc, XmlNode signatureNode, X509Certificate2 cert)
        {
            SignedXml signedXml = new SignedXml(xmlDoc);
            signedXml.LoadXml((XmlElement)signatureNode);
            return signedXml.CheckSignature(cert, true);
        }

        public static KYCData? ExtracteKYCData(string xmlContent)
        {
            try
            {
                var xmlDoc = LoadXmlDocument(xmlContent);
                var nsManager = CreateNamespaceManager(xmlDoc);
                var certNode = xmlDoc.SelectSingleNode("//ds:X509Certificate", nsManager);

                if (certNode == null || string.IsNullOrWhiteSpace(certNode.InnerText))
                    throw new InvalidOperationException("Certificate node is missing or empty in XML.");

                var cert = new X509Certificate2(Convert.FromBase64String(certNode.InnerText.Trim()));

                return new KYCData
                {
                    Issuer = cert.Issuer,
                    GetPublicKey = cert.GetPublicKeyString(),
                    Subject = cert.Subject,
                    Notbefore = cert.NotBefore,
                    Notafter = cert.NotAfter,
                    SerialNumber = cert.SerialNumber,
                    SignatureAlgoName = cert.SignatureAlgorithm.FriendlyName
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting KYC data: {ex.Message}");
                return null; // Returning null to indicate failure
            }
        }

    }
}
