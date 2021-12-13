using Microsoft.VisualStudio.TestTools.UnitTesting;
using FargoConnectSdkExamples;
using System;

namespace SuccessTest
{
    [TestClass]
    public class SuccessUnitTestCases
    {
        Program myclass = new Program();

        [TestMethod]
        public void Orgname()
        {
            string Orgname = myclass.orgname();
            Console.WriteLine("orgname:"+Orgname);
            StringAssert.Contains(Orgname, "Junittesting");
        }

        [TestMethod]
        public void Orgnames()
        {
            string OrgID = myclass.orgID();
            Console.WriteLine("orgID:" + OrgID);
            StringAssert.Contains(OrgID, "ORG66EEA7F7C73141849E4D0CB4B733A0CD");
        }

        [TestMethod]
        public void LocationName()
        {
            string LocationName = myclass.location();
            Console.WriteLine("LocationName:" + LocationName);
            StringAssert.Contains(LocationName, "subramanya H P");
        }

        [TestMethod]
        public void LocationID()
        {
            string LocationID = myclass.locationID();
            Console.WriteLine("LocationID:" + LocationID);
            StringAssert.Contains(LocationID, "LOC7B00503775AF437FB2AAB161A70D2627");
        }

        [TestMethod]
        public void ProductionProfileName()
        {
            string ProductionProfileName = myclass.productionProfileName();
            Console.WriteLine("ProductionProfileName:" + ProductionProfileName);
            StringAssert.Contains(ProductionProfileName, "testing");
        }

        [TestMethod]
        public void ProductionProfileID()
        {
            string ProductionProfileID = myclass.productionProfileID();
            Console.WriteLine("ProductionProfileID:" + ProductionProfileID);
            StringAssert.Contains(ProductionProfileID, "PRAED88ED3EEA94F94B012211145E44742");
        }

        [TestMethod]
        public void PrinterName()
        {
            string PrinterName = myclass.PrintDest();
            Console.WriteLine("PrinterName:" + PrinterName);
            StringAssert.Contains(PrinterName, "Printer1");
        }

        [TestMethod]
        public void ProductionProfile()
        {
            string Blankcard = myclass.Prod();
            Console.WriteLine("Blankcard:" + Blankcard);
            StringAssert.Contains(Blankcard, "blankcard");
        }

        [TestMethod]
        public void JobStatusFailed()
        {
            string JobStatusFailed = myclass.JobStatusFailed();
            Console.WriteLine("JobStatusFailed:" + JobStatusFailed);
            StringAssert.Contains(JobStatusFailed, "Failed");
        }

        [TestMethod]
        public void JobStatusPrinted()
        {
            string JobStatusPrinted = myclass.JobStatusPrinted();
            Console.WriteLine("JobStatusPrinted:" + JobStatusPrinted);
            StringAssert.Contains(JobStatusPrinted, "Printed");
        }
        

        [TestMethod]
        public void JobStatusSubmitted()
        {
            string JobStatusPrinted = myclass.JobStatusSubmitted();
            Console.WriteLine("JobStatusSubmitted:" + JobStatusPrinted);
            StringAssert.Contains(JobStatusPrinted, "Submitted");
        }

    }
}
