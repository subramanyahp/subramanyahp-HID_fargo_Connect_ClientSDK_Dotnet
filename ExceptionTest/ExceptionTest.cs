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
        private const string TestServerApiKey = "6A2202CD3D5DE6919FAFBEEB6749A62116B64066BCBE335B42DE310CCFA8D36A";
        private const string TestServerUrl = "https://test.api.hfc.hidglobal.com:18443";
        private const string SSL_Cert_Path = @"C:\OneDrive\OneDrive-AssaAbloyInc\HFC Internal Share\Certificates\archive\Schlumberger-Development-Certs\C#\Schlumberger_Card_Services_Client_API_Auth_Cert.p12";
        private const string SSL_Cert_Pwd = "xgWkY3uwSDX2JX1qyvi7";

        //load the program and set the constants
        Program myclass = new(TestServerApiKey, TestServerUrl, SSL_Cert_Path, SSL_Cert_Pwd);


        //strings to verify against       
        private const string OrgNameToVerify = "junittesting_failures";
        private const string OrgIdToVerify = "ORG66EEA7F7C73141849E4D0CB4B733A0CD";
        private const string LocationNameToVerify = "subramanya H P";
        private const string LocationIdToVerify = "LOC7B00503775AF437FB2AAB161A70D2627";
        private const string ProductionProfileNameToVerify = "testing";
        private const string ProductionProfileIdToVerify = "PRAED88ED3EEA94F94B012211145E44742";
        private const string PrinterNameToVerify = "Printer1";
        private const string CardTypeToVerify = "blankcard";
        private const string FailedJobId = "JOBC9914E45D536449BB5A5A8317A344A07";
        private const string SubmittedJobId = "JOB5A65D30732CE47EE99093B43A3216E7F";
        private const string PrintedJobId = "JOB055FB17381DF4788891DD1277399F9F5";
        private const string DeletedJobId = "JOBC9914E45D536449BB5A5A8317A344A07";
        //private const string WrongJobId = "JOBC9914E45D536449BB5A5A8317A344A0B";


        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void ServerUriPortException()
        {
            var clientCertificate = new X509Certificate2(File.ReadAllBytes(myclass.SSL_Cert_Path), myclass.SSL_Cert_Pwd);
            //will throw exception
            var cardServicesClient = myclass.ConfigureClientexp(myclass.TestServerUrl + "2", myclass.TestServerApiKey, clientCertificate);
           // string serverURiexception = cardServicesClient.ToString();
          //  Console.WriteLine("serverURiPortexception:" + serverURiexception);
          //  StringAssert.Contains(serverURiexception, "Invalid URI: Invalid port specified.");
        }
      
        [TestMethod]
        [ExpectedException(typeof(FargoConnect.CardServices.RestApi.Client.CardServicesApiException))]
        public void ServerUriPrefixException()
        {
            var clientCertificate = new X509Certificate2(File.ReadAllBytes(myclass.SSL_Cert_Path), myclass.SSL_Cert_Pwd);
            var cardServicesClient = myclass.ConfigureClientexp(myclass.TestServerUrl.Replace("http", "htp"), myclass.TestServerApiKey, clientCertificate);

            //will throw exception
            int deviceCount= cardServicesClient.DeviceApi.GetDevices().Count;

         //   Console.WriteLine("ServerUriPrefixException:" + serverURiexception);
          //  StringAssert.Contains(serverURiexception, "Error getting organizations - The URI prefix is not recognized.");
        }
      
              [TestMethod]
              [ExpectedException(typeof(FargoConnect.CardServices.RestApi.Client.CardServicesApiException))]
              public void ServerUriHostException()
              {
                  var clientCertificate = new X509Certificate2(File.ReadAllBytes(myclass.SSL_Cert_Path), myclass.SSL_Cert_Pwd);
                  var cardServicesClient = myclass.ConfigureClientexp("https://somebadurljuju.hidglobal.com:1456", myclass.TestServerApiKey, clientCertificate);

            //will throw exception
            int deviceCount = cardServicesClient.DeviceApi.GetDevices().Count;

            //  string serverURiexception = cardServicesClient.ToString();
            //  Console.WriteLine("ServerUriHostException:" + serverURiexception); 
            //  StringAssert.Contains(serverURiexception, "Error getting organizations - No such host is known. (somebadurljuju.hidglobal.com:18443)");
        }
        [TestMethod]
        [ExpectedException(typeof(FargoConnect.CardServices.RestApi.Client.CardServicesApiException))]
        public void ApiException()
        {
            var clientCertificate = new X509Certificate2(File.ReadAllBytes(myclass.SSL_Cert_Path), myclass.SSL_Cert_Pwd);
            var cardServicesClient = myclass.ConfigureClientexp(myclass.TestServerUrl, myclass.TestServerApiKey + "2", clientCertificate);

            //will throw exception
            int deviceCount = cardServicesClient.DeviceApi.GetDevices().Count;

            //  string serverURiexception = cardServicesClient.ToString();
            //  Console.WriteLine("ServerUriHostException:" + serverURiexception); 
            //  StringAssert.Contains(serverURiexception, "Error getting organizations - No such host is known. (somebadurljuju.hidglobal.com:18443)");
        }

          
        [TestMethod]
        public void BadOrgId()
        {
            String badOrgId = "ORG66EEA7F7C";
            try
            {
                string OrgID = myclass.orgID(badOrgId);
            }
            catch (CardServicesApiException e)
            {
                StringAssert.Contains(e.Message, "Organization not found: '" + badOrgId);
            }
            
            
        }

        [TestMethod]
        public void BadOrgUnitId()
        {
            String badOrgUnitId = "OU66EEA7F7C";
            try
            {
                string OrgUnitID = myclass.orgUnitID(badOrgUnitId);
            }
            catch (CardServicesApiException e)
            {
                StringAssert.Contains(e.Message, "Organizational unit not found: '" + badOrgUnitId);
            }


        }

        [TestMethod]
        public void BadLocationId()
        {
            String badLocId = "LOC66EEA7F7C";
            try
            {
                string LocID = myclass.locationID(badLocId);
            }
            catch (CardServicesApiException e)
            {
                StringAssert.Contains(e.Message, "Location not found: '" + badLocId);
            }
        }

      
          [TestMethod]
          public void BadProductionProfileId()
          {
            String badProfId = "PRAED88ED";
            try
            {
                string LocID = myclass.productionProfileID(badProfId);
            }
            catch (CardServicesApiException e)
            {
                StringAssert.Contains(e.Message, "profile not found: '" + badProfId);
            }
        }

        [TestMethod]
        public void BadDeviceId()
        {
            String badDeviceId = "MFA12345";
            try
            {
                string DeviceID = myclass.deviceID(badDeviceId);
            }
            catch (CardServicesApiException e)
            {
                StringAssert.Contains(e.Message, "Device not found: '" + badDeviceId);
            }
        }

       
          [TestMethod]
          public void JobNotFound()
          {
            String badJobId = "JOB12345";
            try
            {
                string JobStatus = myclass.JobStatus(badJobId);
            }
            catch (CardServicesApiException e)
            {
                StringAssert.Contains(e.Message, "Job not found: '" + badJobId);
            }
            
          }
    }
}
