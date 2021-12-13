//
// HID FARGO Connect - Cloud Card Issuance Solution
// Copyright (c) 2016-2020. eXtensia Technologies, All rights reserved
//

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using FargoConnect.CardServices.RestApi.Client;
using FargoConnect.CardServices.RestApi.Model;
using FargoConnect.CardServices.RestApi.Model.Parameter;
using FargoConnect.CardServices.RestApi.Model.Service;


namespace FargoConnectSdkExamples
{
  /// <summary>
  /// FARGO Connect Card Services Client SDK sample application
  /// </summary>
  public class Program
  {
    /// <summary>
    /// Server API key configured in the FARGO Connect platform
    /// </summary>
    private const string TestServerApiKey = "";

    /// <summary>
    /// Base URL of the FARGO Connect integration server
    /// </summary>
    private const string TestServerUrl = "";



    public static void Main(string[] args)
    {
      /*
       * Ensure the test server API key and URL are configured
       */
      if (String.IsNullOrWhiteSpace(TestServerApiKey) || String.IsNullOrWhiteSpace(TestServerUrl))
      {
        Console.WriteLine();
        Console.WriteLine("Test server API key and URL have not been configured");
        Console.WriteLine();
        Console.WriteLine("Press any key to close...");
        Console.ReadKey();
        return;
      }

      /*
       * Prompt the user to select the client authentication certificate
       */
      if (!SelectClientAuthCertificate(out var clientAuthCertificate))
      {
        Console.WriteLine("No client authentication certificate selected");
        return;
      }

      /*
       * Display certificate information for debug purposes
       */
      Console.WriteLine($"Using client certificate: {clientAuthCertificate.FriendlyName}");
      Console.WriteLine();

      /*
       * Create a FARGO Connect client instance
       */
      var cardServicesClient = ConfigureClient(TestServerUrl, TestServerApiKey, clientAuthCertificate);

      try
      {
        /*
         * List the available organizations and select the first one for demo purposes.
         */
        if (!SelectOrganization(cardServicesClient, out var organization))
        {
          return;
        }

        Console.WriteLine($"Using organization: {organization.Name}");
        Console.WriteLine();

        /*
         * Show information about the selected organization and devices
         */
        ShowOrganizationInfo(cardServicesClient, organization.OrganizationId);

        /*
         * List the available production profiles for the selected organization and select
         * the first production profile for demo purposes.
         */
        if (!SelectProductionProfile(cardServicesClient, organization.OrganizationId, out var productionProfile))
        {
          return;
        }

        Console.WriteLine($"Using production profile: {productionProfile.Name}");
        Console.WriteLine();

        /*
         * List the available print destinations for the selected organization and select
         * the first print destination for demo purposes.
         */
        if (!SelectPrintDestination(cardServicesClient, organization.OrganizationId, out var printDestination))
        {
          return;
        }

        Console.WriteLine($"Using print destination: {printDestination.Destination}");
        Console.WriteLine();

        /*
         * Retrieve the configuration parameters for the production profile
         */
        var profileConfig = cardServicesClient.ProductionProfileApi.GetProductionProfileParameters(productionProfile.ProfileId);

        /*
         * Process the list of profile configuration parameters. By default only a single
         * parameter named "CardType" of type ListParameter is required for card production
         * requests. The "CardType" parameter is essentially a configurable enumerated type that
         * maps a logical card type name (e.g. "Employee", "Student" etc) to a card template.
         */
        foreach (var profileParam in profileConfig.ProfileParameters)
        {
          /*
           * Fail if an unexpected parameter type is present
           */
          if (profileParam.DataType != DataType.List || string.CompareOrdinal(profileParam.Name, ProfileParamConst.CardType) != 0)
          {
            throw new Exception($"Unhandled production profile parameter type: {profileParam.DataType.ToString()}");
          }

          /*
           * Show the available card types for debug purposes
           */
          var cardTypeParam = (ListParameter) profileParam;

          Console.WriteLine("Available card types");
          cardTypeParam.Options.ToList().ForEach(cardType => Console.WriteLine($"  '{cardType}'"));
          Console.WriteLine();

          /*
           * Arbitrarily select the first card type for demo purposes. The supported card
           * types are normally known in advance and the Option property is used to validate
           * whether the desired card type is a valid option.
           */
          var selectedCardType = cardTypeParam.Options.First();
          cardTypeParam.Value = selectedCardType;

          Console.WriteLine($"Selecting CardType: {selectedCardType}");
          Console.WriteLine();
        }

        Console.WriteLine("Configuring the production request");
        Console.WriteLine();

        /*
         * Configure the production profile to obtain a production request template for
         * the previously selected "CardType" value.
         */
        var productionRequestTemplate = cardServicesClient.ProductionProfileApi.ConfigureProductionProfile(profileConfig);

        /*
         * Iterate over the services in the production request template. By default,
         * only one card production service will be present in the request template.
         */
        Console.WriteLine("Configuring service parameters");

        foreach (var service in productionRequestTemplate.Services)
        {
          /*
           * Fail if an unexpected service type is present
           */
          if (service.Type != ServiceType.CardRequest)
          {
            throw new Exception($"Unhandled production service type: {service.Type.ToString()}");
          }

          /*
           * Process the card production request parameters
           */
          var cardRequest = (CardRequestService) service;

          for (var i = 0; i < cardRequest.Parameters.Count; i++)
          {
            var dataParameter = cardRequest.Parameters[i];

            /*
             * Cast the parameter to the correct class based on DataType. An error is
             * raised for parameter types other than Text and Image since those are the
             * only types configured for the demo. 
             */
            switch (dataParameter.Data.DataType)
            {
              case DataType.Text:
                /*
                 * Set the Text parameter Value to the parameter's Name for demo purposes.
                 * The text value would normally be supplied by the application.
                 */
                var textParam = (TextParameter) dataParameter.Data;
                textParam.Value = textParam.Name;

                Console.WriteLine("  Param[{0}] ({1}): {2} -> {3}", i, textParam.DataType, textParam.Name, textParam.Value);
                break;

              case DataType.Image:
                /*
                 * Set the Image parameter Value to a test image for demo purposes. The
                 * image would normally be supplied by the application. The PreferredWidth
                 * and PreferredHeight properties indicate the preferred image dimensions
                 * and aspect ratio. The default value is -1 (indicates no preference)
                 */
                var imageParam = (ImageParameter) dataParameter.Data;
                imageParam.ImageStream = new FileStream(@"photos\testimage.png", FileMode.Open, FileAccess.Read, FileShare.Read);

                Console.WriteLine("  Param[{0}] ({1}): {2} (Pref Size: {3}x{4} pixels)", i,
                  imageParam.DataType, imageParam.Name,
                  imageParam.PreferredWidth, imageParam.PreferredHeight);
                break;

              default:
                throw new Exception($"Unexpected service data parameter: {dataParameter.Data.Name}");
            }
          }

          /*
           * Configure the print destination and job displayed on the console
           */
          cardRequest.Destination = printDestination.Destination;
          cardRequest.RequestName = "Test card request";

          /*
           * Advanced SDK feature: Override the printer input hopper selection configured in the
           * card template. The default is to use the input hopper selected in the card template
           * unless explicitly overridden by a job service option setting as shown here.
           */
          cardRequest.ServiceOptions.Add(PrinterOption.InputHopperSelect, PrinterOption.UseHopper1);
        }

        /*
         * Submit the job to the server for processing. The return job Id is typically
         * stored by the application and used to monitor job status and retrieve
         * job results using JobApi methods.
         */
        var submittedJobId = cardServicesClient.JobApi.SubmitProductionRequest(productionRequestTemplate);

        Console.WriteLine();
        Console.WriteLine("Job submitted successfully. Job unique Id = {0}", submittedJobId);

        /*
         * Query the server for a list of jobs submitted within the last 24 hours
         */
        ShowRecentJobs(cardServicesClient);

        /*
         * Query the server for details of the submitted job. The job status will be "Submitted"
         * pending completion of job.
         */
        ShowJobDetails(cardServicesClient, submittedJobId);

      }
      catch (CardServicesApiException ex)
      {
        Console.WriteLine("Client API Exception: (Error Code: {0}) -> {1}", (int) ex.StatusCode, ex.Message);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }

      Console.WriteLine();
      Console.WriteLine("Press any key to close...");
      Console.ReadKey();
    }

    /**
     * Demonstrates querying the server for jobs submitted within on a defines look-back
     * period. This is useful for displaying recent job activity. The GetJobsForDateRange
     * method can be used for the same purpose, but provides additional flexibility.
     */
    private static void ShowRecentJobs(CardServicesClient cardServicesClient)
    {
      Console.WriteLine();
      Console.WriteLine("Recent job  details");
      Console.WriteLine();
      var recentJobs = cardServicesClient.JobApi.GetJobsForTimePeriod(100, TimeSpan.FromHours(24));

      foreach (var jobDetails in recentJobs)
      {
        Console.WriteLine("Job Name..............: {0}", jobDetails.JobName);
        Console.WriteLine("Job Status............: {0}", jobDetails.JobStatus);
        Console.WriteLine("Status Message........: {0}", jobDetails.JobStatusMessage);
        Console.WriteLine("Date Submitted........: {0}", jobDetails.SubmitDate.ToLocalTime());
        Console.WriteLine();
      }
    }

    /// <summary>
    /// Queries the sever for the job corresponding to the specified unique job Id.
    /// </summary>
    /// <param name="cardServicesClient">Card services client</param>
    /// <param name="jobUniqueId">Job unique Id returned during job submission</param>
    private static void ShowJobDetails(CardServicesClient cardServicesClient, string jobUniqueId)
    {
      Console.WriteLine();
      Console.WriteLine("Retrieving job details");
      Console.WriteLine();

      var jobDetails = cardServicesClient.JobApi.GetJob(jobUniqueId);

      Console.WriteLine("Job Name..............: {0}", jobDetails.JobName);
      Console.WriteLine("Job Unique Id.........: {0}", jobDetails.JobUniqueId);
      Console.WriteLine("Job Status............: {0}", jobDetails.JobStatus);
      Console.WriteLine("Status Message........: {0}", jobDetails.JobStatusMessage);
      Console.WriteLine("Date Submitted........: {0}", jobDetails.SubmitDate.ToLocalTime());
      Console.WriteLine("Last Updated..........: {0}", jobDetails.LastUpdate.ToLocalTime());

      /*
       * Show job results if the job printed successfully
       */
      if (string.Equals(jobDetails.JobStatus, JobStatus.Printed))
      {
        var cardReadResults = jobDetails.ServiceData.CardReadResults;
        Console.WriteLine("Card Read Results.....: {0} Card Edge(s) Found", cardReadResults.CardEdges.Count);

        if (cardReadResults.CardEdges.Count > 0)
        {
          /*
           * Show the details for each of the card edges found. All card edges discovered
           * are returned. Card types enabled in the Card Read Service configuration in
           * the card template have an Enabled value of true.
           *
           * Note: Some card technologies such as HID_ICLASS support multiple frame
           *   protocols and may be reported in the results more than once. The
           *   CardSerialNumber and PACS data should be identical when this occurs.
           */
          foreach (var cardEdge in cardReadResults.CardEdges)
          {
            Console.WriteLine();
            Console.WriteLine("  Card Edge Type......: {0}", cardEdge.EdgeType);
            Console.WriteLine("  Card Protocol.......: {0}", cardEdge.CardProtocol);
            Console.WriteLine("  Card Edge Enabled...: {0}", cardEdge.Enabled);
            Console.WriteLine("  Card Read Status....: {0}", cardEdge.Status);
            Console.WriteLine("  Card Read Message...: {0}", cardEdge.StatusMessage);
            Console.WriteLine("  Card Serial Number..: {0}", cardEdge.CardSerialNumber);
            Console.WriteLine("  PACS Data Available.: {0}", cardEdge.PacsDataAvailable);

            /*
             * Display the card PACS bits and decoded PACS data for the card edge
             */
            if (cardEdge.PacsDataAvailable)
            {
              Console.WriteLine("  PACS Bit Data.......: 0x{0}", cardEdge.CardPacsBitData);
              Console.WriteLine("  PACS Bit Count......: {0}", cardEdge.CardPacsBitCount);

              /*
               * Show the PACS decode results for each card format configured for
               * this card edge in the card designer. The decoded PACS data is only
               * valid when the DecodeStatus is "Success". The application developer
               * must ensure the correct format(s) are configured in the card designer
               * and provide appropriate format selection and error handling logic.
               */
              cardEdge.PacsData.ForEach(decodeResult =>
              {
                Console.WriteLine();
                Console.WriteLine("    Card Format.......: {0}", decodeResult.FormatName);
                Console.WriteLine("    Format Bit Count..: {0}", decodeResult.FormatBitCount);
                Console.WriteLine("    Decode Status.....: {0}", decodeResult.DecodeStatus);
                Console.WriteLine("    Status Message....: {0}", decodeResult.StatusMessage);
                Console.WriteLine("    Card Number.......: {0}", decodeResult.CardNumber);
                Console.WriteLine("    PACS Data Fields:");

                /*
                 * List the data fields extracted from the PACS bits. The list
                 * of fields and their names are defined by the card format and
                 * should not be assumed to be consistent across card formats.
                 *
                 * Note: The card number is always included in this list since
                 *   is is a mandatory field, but the name of the field may vary
                 *   across card formats. Please use the CardNumber property
                 *   instead.
                 */
                foreach (var pacsField in decodeResult.PacsFields)
                {
                  Console.WriteLine("      {0} -> {1}", pacsField.Key, pacsField.Value);
                }
              });
            }

            /*
             * Show additional key/value pairs returned for the card edge. This is
             * used to return ad-hoc data values for specialized applications.
             */
            if (cardEdge.Data.Count > 0)
            {
              Console.WriteLine();
              Console.WriteLine("  Additional Data");

              foreach (var additionalData in cardEdge.Data)
              {
                Console.WriteLine("    {0} -> {1}", additionalData.Key, additionalData.Value);
              }
            }
          }
        }
      }

      //
      //  Sample Output - Dual technology MIFARE/SEOS card
      //
      //  Job Name..............: Card Read Test
      //  Job Unique Id.........: JOBA5D31540B8AC4CCDB930FF914D9228D3
      //  Job Status............: Printed
      //  Status Message........: Job printed successfully
      //  Date Submitted........: 4/9/2018 3:31:28 PM
      //  Last Updated..........: 4/9/2018 3:34:22 PM
      //  Card Read Results.....: 2 Card Edge(s) Found
      //
      //    Card Edge Type......: MIFARE_CLASSIC
      //    Card Protocol.......: ISO14443A_3
      //    Card Edge Enabled...: True
      //    Card Read Status....: Success
      //    Card Read Message...: Card edge found and processed successfully
      //    Card Serial Number..: 088931D3
      //    PACS Data Available.: False
      //
      //    Card Edge Type......: SEOS
      //    Card Protocol.......: ISO14443A
      //    Card Edge Enabled...: True
      //    Card Read Status....: Success
      //    Card Read Message...: Card edge found and processed successfully
      //    Card Serial Number..: 08A62E6B
      //    PACS Data Available.: True
      //    PACS Bit Data.......: 0x1D682250
      //    PACS Bit Count......: 35
      //
      //      Card Format.......: H234561
      //      Format Bit Count..: 26
      //      Decode Status.....: Success
      //      Status Message....: Decode succeeded
      //      Card Number.......: 679123
      //      PACS Data Fields
      //        Facility Code-> 115
      //        Card Number -> 679123    
    }

    /// <summary>
    /// Lists the available organizations and arbitrarily selects and returns the first 
    /// organization for demo purposes. Organization identifiers are designed to be stable 
    /// over time and can be stored and reused.
    /// </summary>
    /// <param name="cardServicesClient">Card services client</param>
    /// <param name="organization">Selected organization</param>
    /// <returns>True if an organization was selected</returns>
    private static bool SelectOrganization(CardServicesClient cardServicesClient, out Organization organization)
    {
      var organizations = cardServicesClient.OrganizationApi.GetOrganizations();

      if (organizations.Count < 1)
      {
        Console.WriteLine("No organizations found");
        organization = null;
        return false;
      }

      Console.WriteLine("Available organizations");
      organizations.ForEach(org => Console.WriteLine($"  {org.OrganizationId} -> {org.Name}"));
      Console.WriteLine();

      organization = organizations[0];
      return true;
    }

    /// <summary>
    /// Lists the organizational units and locations within the specified organization. The
    /// organizational structure is loosely based on the X.500 directory model and has a fixed
    /// three-tier hierarchy of Organization, Organizational Unit and Location. Devices are
    /// defined at the Location level of the hierarchy. Organizational identifiers are designed 
    /// to be stable over time and can be stored and reused.
    /// </summary>
    /// <param name="cardServicesClient">Card services client</param>
    /// <param name="organizationId">Organization unique Id</param>
    private static void ShowOrganizationInfo(CardServicesClient cardServicesClient, string organizationId)
    {
      /*
       * Enumerate the organizational units within the organization
       */
      Console.WriteLine("Organizational Units for Organization {0}", organizationId);
      var organizationalUnits = cardServicesClient.OrganizationApi.GetOrganizationalUnits(organizationId);
      organizationalUnits.ForEach(orgUnit => Console.WriteLine("  {0} -> {1}", orgUnit.OrganizationUnitId, orgUnit.Name));
      Console.WriteLine();

      /*
       * Enumerate the locations for the organization. Note that Locations exist within
       * Organizational Units, but are listed here across all Organizational Units for
       * simplicity. Use the GetOrganizationUnitLocations method to query Locations by
       * organizational unit.
       */
      Console.WriteLine("Locations for Organization {0}", organizationId);
      var locations = cardServicesClient.OrganizationApi.GetOrganizationLocations(organizationId);
      locations.ForEach(location => Console.WriteLine("  {0} -> {1}", location.LocationId, location.LocationName));
      Console.WriteLine();
    }

    /// <summary>
    /// Lists the available production profiles for the given organization and arbitrarily 
    /// selects and returns the first production profile for demo purposes. Production
    /// profile identifiers are designed to be stable over time and can be stored and reused.
    /// </summary>
    /// <param name="cardServicesClient">Card services client</param>
    /// <param name="organizationId">Organization unique Id</param>
    /// <param name="productionProfile">Selected production profile</param>
    /// <returns>True if a production profile was selected</returns>
    private static bool SelectProductionProfile(CardServicesClient cardServicesClient,
      string organizationId, out ProductionProfile productionProfile)
    {
      var productionProfiles = cardServicesClient.ProductionProfileApi.GetProductionProfiles(organizationId);

      if (productionProfiles.Count < 1)
      {
        Console.WriteLine("No production profiles found");
        productionProfile = null;
        return false;
      }

      Console.WriteLine("Available production profiles");
      productionProfiles.ForEach(profile => Console.WriteLine($"  {profile.ProfileId} -> {profile.Name}"));
      Console.WriteLine();

      productionProfile = productionProfiles[0];
      return true;
    }

    /// <summary>
    /// Lists the available print destinations for the given organization and arbitrarily 
    /// selects and returns the first print destination for demo purposes. Print destination
    /// identifiers are designed to be stable over time and can be stored and reused.
    /// </summary>
    /// <param name="cardServicesClient">Card services client</param>
    /// <param name="organizationId">Organization unique Id</param>
    /// <param name="printDestination">Selected print destination</param>
    /// <returns>True if a production profile was selected</returns>
    private static bool SelectPrintDestination(CardServicesClient cardServicesClient,
      string organizationId, out PrintDestination printDestination)
    {
      var printDestinations = cardServicesClient.DeviceApi.GetPrintDestinations(organizationId);

      if (printDestinations.Count < 1)
      {
        Console.WriteLine("No print destinations found");
        printDestination = null;
        return false;
      }

      Console.WriteLine("Available print destinations");
      printDestinations.ForEach(destination => Console.WriteLine($"  {destination.Destination} -> {destination.PrinterName}"));
      Console.WriteLine();

      printDestination = printDestinations[0];
      return true;
    }

    /// <summary>
    /// Prompts the user to select a client authentication certificate using the system 
    /// certificate selection dialog. This selection method was chosen purely for demo 
    /// purposes. Typical applications perform certificate selection programmatically 
    /// without prompting the user.
    /// </summary>
    /// <param name="clientCertificate">Selected certificate or null if none</param>
    /// <returns>True if a certificate was selected</returns>
    private static bool SelectClientAuthCertificate(out X509Certificate2 clientCertificate)
    {
      var certificateStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);

      certificateStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

      var storeCertCollection = certificateStore.Certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
      certificateStore.Close();

      var clientCertSelection = X509Certificate2UI.SelectFromCollection(storeCertCollection,
        "Client Authentication Certificate", "Please select the FARGO Connect Client authentication certificate",
        X509SelectionFlag.SingleSelection);

      if (clientCertSelection.Count < 1)
      {
        clientCertificate = null;
        return false;
      }

      clientCertificate = clientCertSelection[0];

      //
      // DEVELOPMENT NOTE:
      //
      // When compiling the sample application using .Net Core, a build error will occur since
      // X509Certificate2UI is not supported by the target .Net environment. The issue can be
      // resolved by replacing the above code with logic to programmatically locate and load
      // the client authentication certificate. The line below, for example, loads a certificate
      // in PKCS#12 format from the local file system.
      // 
      // clientCertificate = new X509Certificate2(File.ReadAllBytes(@"C:\My\Certificate\Path\MyAuthCertificate.p12"), "certificate_password");

      return true;
    }

    /// <summary>
    /// Creates and configures a new client instance
    /// </summary>
    /// <param name="serverUrl">Server base URL</param>
    /// <param name="apiKey">Server API key</param>
    /// <param name="clientCert">Client authentication cert</param>
    /// <returns></returns>
    private static CardServicesClient ConfigureClient(string serverUrl, string apiKey, X509Certificate2 clientCert)
    {
      var clientConfig = new CardServicesClientConfig
      {
        ApiKey = apiKey,
        ServerBaseUri = new Uri(serverUrl),
        AuthenticationCert = clientCert
      };

      return new CardServicesClient(clientConfig);
    }
  }
}