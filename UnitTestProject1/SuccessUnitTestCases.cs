using Microsoft.VisualStudio.TestTools.UnitTesting;
using FargoConnectSdkExamples;
using System;

namespace SuccessTest
{
    [TestClass]
    public class SuccessUnitTestCases
    {
        private const string TestServerApiKey = "528DFB0B54B2438A5F631E94754BA4DA088E0EA1CF9700EF4ACE0B4C6D49F957";
        private const string TestServerUrl = "https://test.api.hfc.hidglobal.com:18443";
        private const string SSL_Cert_Path = @"C:\OneDrive\OneDrive-AssaAbloyInc\HFC Internal Share\Certificates\archive\Schlumberger-Development-Certs\C#\Schlumberger_Card_Services_Client_API_Auth_Cert.p12";
        private const string SSL_Cert_Pwd = "xgWkY3uwSDX2JX1qyvi7";

        //load the program and set the constants
        Program myclass = new(TestServerApiKey, TestServerUrl, SSL_Cert_Path, SSL_Cert_Pwd);


        //strings to verify against       
        private const string OrgNameToVerify = "Junittesting";
        private const string OrgIdToVerify = "ORG66EEA7F7C73141849E4D0CB4B733A0CD";
        private const string OrgUnitNameToVerify = "subramanya H P";
        private const string OrgUnitIdToVerify = "OU1BC0B36616DC4C46BE4E230FA1ECAC1A";
        private const string LocationNameToVerify = "subramanya H P";
        private const string LocationIdToVerify = "LOC7B00503775AF437FB2AAB161A70D2627";
        private const string ProductionProfileNameToVerify = "testing";
        private const string ProductionProfileIdToVerify = "PRAED88ED3EEA94F94B012211145E44742";
        private const string DeviceNameToVerify = "Andrew Console";
        private const string DviceIdToVerify = "MFA190F19BBA9634BBE850DCE852452E814";
        private const string PrinterNameToVerify = "Printer1";
        private const string CardTypeToVerify = "blankcard";
        private const string FailedJobId = "JOBC9914E45D536449BB5A5A8317A344A07";
        private const string SubmittedJobId = "JOB5A65D30732CE47EE99093B43A3216E7F";
        private const string PrintedJobId = "JOB055FB17381DF4788891DD1277399F9F5";
        private const string DeletedJobId = "JOBC9914E45D536449BB5A5A8317A344A07";
        //private const string WrongJobId = "JOBC9914E45D536449BB5A5A8317A344A0B";

        [TestMethod]
        public void OrgName()
        {
            string Orgname = myclass.orgname();
            Console.WriteLine("orgname:"+Orgname);
            StringAssert.Contains(Orgname, OrgNameToVerify);
        }

        [TestMethod]
        public void OrgUnitId()
        {
            string OrgUnitID = myclass.orgUnitID(OrgUnitIdToVerify);
            Console.WriteLine("orgUnitID:" + OrgUnitID);
            StringAssert.Contains(OrgUnitID, OrgUnitIdToVerify);
        }

        [TestMethod]
        public void OrgUnitName()
        {
            string OrgUnitname = myclass.orgUnitname(OrgIdToVerify);
            Console.WriteLine("orgUnitname:" + OrgUnitname);
            StringAssert.Contains(OrgUnitname, OrgUnitNameToVerify);
        }

        [TestMethod]
        public void OrgId()
        {
            string OrgID = myclass.orgID(OrgIdToVerify);
            Console.WriteLine("orgID:" + OrgID);
            StringAssert.Contains(OrgID, OrgIdToVerify);
        }

        [TestMethod]
        public void LocationName()
        {
            string LocationName = myclass.location();
            Console.WriteLine("LocationName:" + LocationName);
            StringAssert.Contains(LocationName, LocationNameToVerify);
        }

        [TestMethod]
        public void LocationID()
        {
            string LocationID = myclass.locationID(LocationIdToVerify);
            Console.WriteLine("LocationID:" + LocationID);
            StringAssert.Contains(LocationID, LocationIdToVerify);
        }

        [TestMethod]
        public void ProductionProfileName()
        {
            string ProductionProfileName = myclass.productionProfileName();
            Console.WriteLine("ProductionProfileName:" + ProductionProfileName);
            StringAssert.Contains(ProductionProfileName, ProductionProfileNameToVerify);
        }

        [TestMethod]
        public void ProductionProfileID()
        {
            string ProductionProfileID = myclass.productionProfileID(ProductionProfileIdToVerify);
            Console.WriteLine("ProductionProfileID:" + ProductionProfileID);
            StringAssert.Contains(ProductionProfileID, ProductionProfileIdToVerify);
        }

        [TestMethod]
        public void DeviceName()
        {
            string DeviceName = myclass.deviceName();
            Console.WriteLine("DeviceName:" + DeviceName);
            StringAssert.Contains(DeviceName, DeviceNameToVerify);
        }
        [TestMethod]
        public void DeviceID()
        {
            string DeviceID = myclass.deviceID(DviceIdToVerify);
            Console.WriteLine("DeviceD:" + DeviceID);
            StringAssert.Contains(DeviceID, DviceIdToVerify);
        }

        [TestMethod]
        public void PrinterName()
        {
            string PrinterName = myclass.PrintDest();
            Console.WriteLine("PrinterName:" + PrinterName);
            StringAssert.Contains(PrinterName, PrinterNameToVerify);
        }

        [TestMethod]
        public void CardType()
        {
            string Blankcard = myclass.GetCardType();
            Console.WriteLine("Blankcard:" + Blankcard);
            StringAssert.Contains(Blankcard, CardTypeToVerify);
        }

        [TestMethod]
        public void JobStatusFailed()
        {
            string JobStatus = myclass.JobStatus(FailedJobId);
            Console.WriteLine("JobStatusFailed:" + JobStatus);
            StringAssert.Contains(JobStatus, "Failed");
        }

        [TestMethod]
        public void JobStatusPrinted()
        {
            string JobStatus = myclass.JobStatus(PrintedJobId);
            Console.WriteLine("JobStatusPrinted:" + JobStatus);
            StringAssert.Contains(JobStatus, "Printed");
        }
        

        [TestMethod]
        public void JobStatusSubmitted()
        {
            string JobStatus = myclass.JobStatus(SubmittedJobId);
            Console.WriteLine("JobStatusSubmitted:" + JobStatus);
            StringAssert.Contains(JobStatus, "Submitted");
        }

        [TestMethod]
        public void JobStatusDeleted()
        {
            string JobStatus = myclass.JobStatus(DeletedJobId);
            Console.WriteLine("JobStatusDeleted:" + JobStatus);
            StringAssert.Contains(JobStatus, "Failed");
        }


        [TestMethod]
        public void SendPrintJob()
        {
            string JobId = myclass.SendPrint(DviceIdToVerify + "@" + PrinterNameToVerify);
            Console.WriteLine("JobId from print command:" + JobId);

            //now verify it is submitted
            string JobStatus = myclass.JobStatus(JobId);
            Console.WriteLine("JobStatusSubmitted:" + JobStatus);
            StringAssert.Contains(JobStatus, "Submitted");
        }


    }
}
