using Microsoft.VisualStudio.TestTools.UnitTesting;
using FargoConnectSdkExamples;
using System;
using System.IO;
using FargoConnect.CardServices.RestApi.Client;
using System.Security.Cryptography.X509Certificates;

namespace ExceptionTest
{
    [TestClass]
    public class ExceptionTestCases
    {
        Program myclass = new Program();

        [TestMethod]      
        public void APIkeyException()
        {
          string apiexception=  myclass.Apikey_Exception();
            Console.WriteLine("apiexception:" + apiexception);
            StringAssert.Contains(apiexception, "Test server API key and URL have not been configured");
        }

        [TestMethod]
        public void ServerUriException()
        {
            string serverURiexception = myclass.Apikey_Exception();
            Console.WriteLine("serverURiexception:" + serverURiexception);
            StringAssert.Contains(serverURiexception, "Test server API key and URL have not been configured");
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void DirectoryException()
        {
            myclass.DirectoryException();
        }

        [TestMethod]
        [ExpectedException(typeof(TypeLoadException))]
        public void LoadException()
        {
            myclass.LoadException();
        }
        

         [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void ServerUriPortException()
        {
            var clientCertificate = new X509Certificate2(File.ReadAllBytes(@"C:\HFCCertification\Schlumberger-Development-Certs\Java\Schlumberger_Card_Services_Client_API_Auth_Cert.p12"), "xgWkY3uwSDX2JX1qyvi7");
            var cardServicesClient = myclass.ConfigureClientexp("https://test.api.hfc.hidglobal.com:123456", "528DFB0B54B2438A5F631E94754BA4DA088E0EA1CF9700EF4ACE0B4C6D49F957", clientCertificate);
            string serverURiexception = cardServicesClient.ToString();
            Console.WriteLine("serverURiPortexception:" + serverURiexception);
            StringAssert.Contains(serverURiexception, "Invalid URI: Invalid port specified.");
        }

        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void ServerUriPrefixException()
        {
            var clientCertificate = new X509Certificate2(File.ReadAllBytes(@"C:\HFCCertification\Schlumberger-Development-Certs\Java\Schlumberger_Card_Services_Client_API_Auth_Cert.p12"), "xgWkY3uwSDX2JX1qyvi7");
            var cardServicesClient = myclass.ConfigureClientexp("ht://test.api.hfc.hidglobal.com:123456", "528DFB0B54B2438A5F631E94754BA4DA088E0EA1CF9700EF4ACE0B4C6D49F957", clientCertificate);
            string serverURiexception = cardServicesClient.ToString();
            Console.WriteLine("ServerUriPrefixException:" + serverURiexception);
            StringAssert.Contains(serverURiexception, "Error getting organizations - The URI prefix is not recognized.");
        }

        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void ServerUriHostException()
        {
            var clientCertificate = new X509Certificate2(File.ReadAllBytes(@"C:\HFCCertification\Schlumberger-Development-Certs\Java\Schlumberger_Card_Services_Client_API_Auth_Cert.p12"), "xgWkY3uwSDX2JX1qyvi7");
            var cardServicesClient = myclass.ConfigureClientexp("ht://somebadurljuju.hidglobal.com:123456", "528DFB0B54B2438A5F631E94754BA4DA088E0EA1CF9700EF4ACE0B4C6D49F957", clientCertificate);
            string serverURiexception = cardServicesClient.ToString();
            Console.WriteLine("ServerUriHostException:" + serverURiexception); 
            StringAssert.Contains(serverURiexception, "Error getting organizations - No such host is known. (somebadurljuju.hidglobal.com:18443)");
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void certpathException()
        {
            var clientCertificate = new X509Certificate2(File.ReadAllBytes(@"C:\HFCCertificatio\Schlumberger-Development-Certs\Java\Schlumberger_Card_Services_Client_API_Auth_Cert.p12"), "xgWkY3uwSDX2JX1qyvi7");
            var cardServicesClient = myclass.ConfigureClientexp("https://test.api.hfc.hidglobal.com:18443", "528DFB0B54B2438A5F631E94754BA4DA088E0EA1CF9700EF4ACE0B4C6D49F957", clientCertificate);
            string certpathexception = cardServicesClient.ToString();
            Console.WriteLine("certpathException:" + certpathexception);
            StringAssert.Contains(certpathexception, "Exception for Client CA certificate file not found:");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void certPWDException()
        {
            var clientCertificate = new X509Certificate2(File.ReadAllBytes(@"C:\HFCCertification\Schlumberger-Development-Certs\Java\Schlumberger_Card_Services_Clien_API_Auth_Cert.p12"), "xgWkY3uwSDX2JX1qyvi7gfsagf");
            var cardServicesClient = myclass.ConfigureClientexp("https://test.api.hfc.hidglobal.com:18443", "528DFB0B54B2438A5F631E94754BA4DA088E0EA1CF9700EF4ACE0B4C6D49F957", clientCertificate);
            string certPWDException = cardServicesClient.ToString();
            Console.WriteLine("certPWDException:" + certPWDException);
            StringAssert.Contains(certPWDException, "Exception for Client Incorrect Paswword:'xgWkY3uwSDX2JX1qyvi7gfsagf'");
        }


        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void Orgnameexp()
        {
            string Orgname = myclass.orgnameexp();
            Console.WriteLine("orgname:" + Orgname);
            StringAssert.Contains(Orgname, "No organizations found");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void LocationName()
        {
            string LocationName = myclass.location();
            Console.WriteLine("LocationName:" + LocationName);
            StringAssert.Contains(LocationName, "absdcdn");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ProductionProfileName()
        {
            string ProductionProfileName = myclass.productionProfileName();
            Console.WriteLine("ProductionProfileName:" + ProductionProfileName);
            StringAssert.Contains(ProductionProfileName, "absdcdn");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void PrinterName()
        {
            string PrinterName = myclass.PrintDest();
            Console.WriteLine("PrinterName:" + PrinterName);
            StringAssert.Contains(PrinterName, "dgfgs");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void cardtype()
        {
            string Blankcard = myclass.Prod();
            Console.WriteLine("Blankcard:" + Blankcard);
            StringAssert.Contains(Blankcard, "blankcard");
        }

    }
}
