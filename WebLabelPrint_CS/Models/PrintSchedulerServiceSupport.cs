using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Xml.Linq;

using Seagull.Services.PrintScheduler;

namespace WebLabelPrint.Models
{
   /// <summary>
   /// Supporting functions to interact with BarTender through the Print Scheduler Service. 
   /// </summary>
   public static class PrintSchedulerServiceSupport
   {
      /// <summary>
      /// Prints the specified filename to the specified server printer. Note that in a real-world implementation
      /// there are many other things that you can control such as number of copies and serial numbers,
      /// database record selection, named datasource values, data entry data, etc.
      /// 
      /// Returns a list of print messages to display.
      /// </summary>
      public static List<string> PrintToServerPrinter(string documentFileName, string serverPrinterName)
      {
         if (string.IsNullOrEmpty(documentFileName) || string.IsNullOrEmpty(serverPrinterName))
            return new List<string>() { "You must specify a filename to print and a printer to print to." };
         if (!File.Exists(documentFileName))
            return new List<string>() { string.Format("Document {0} does not exist.", documentFileName) };

         PrintAction printAction = new PrintAction(documentFileName);
         printAction.Document.PrintSetup.Printer = serverPrinterName;

         try
         {
            PrintActionResult result = printAction.RunSynchronous();

            // Get messages to return from the action result.
            List<string> printMessages = new List<string>();

            if (result.Exception != null)
               printMessages.Add(result.Exception.ToString());

            if (result.Messages != null)
            {
               foreach (ActionMessage message in result.Messages)
                  printMessages.Add(message.Text);
            }

            return printMessages;
         }
         catch (Exception ex)
         {
            // Note that you will receive a System.ServiceModel.EndpointNotFoundException stating that "Could not connect to net.tcp://localhost:8011/PrintScheduler"
            // if the Print Scheduler Service is not running or not responsive. You could catch that and present a more informative error, but for the sake of brevity
            // we just dump out any exceptions back to the print page.

            return new List<string>() { string.Format("Error occurred while printing {0} to printer {1}. Error: {2}", documentFileName, serverPrinterName, ex.ToString()) };
         }
      }

      /// <summary>
      /// Prints the specified document to a client printer. This works by doing a Print-To-File using the specified server printer and license.
      /// We then capture the file contents into the "printCode" output parameter. The print code is posted in a client-side hidden field on the page
      /// which the client-print module picks up to send to the destination printer. Rather than using a client-side hidden field, another potential
      /// implementation could return the print code directly in a JSON controller action, so that you could directly get print code via AJAX calls.
      /// </summary>
      public static List<string> PrintToClientPrinter(string documentFileName, string serverPrinterName, string printLicense, out string printCode)
      {
         printCode = string.Empty;

         if (string.IsNullOrEmpty(documentFileName) || string.IsNullOrEmpty(serverPrinterName))
            return new List<string>() { "You must specify a filename to print and a server printer to print to." };
         if (string.IsNullOrEmpty(printLicense))
            return new List<string>() { "You must specify a print license to print to a client printer." };
         if (!File.Exists(documentFileName))
            return new List<string>() { string.Format("Document {0} does not exist.", documentFileName) };

         // Get a full filename for where we want to send the print code to
         string printFileName = HttpContext.Current.Server.MapPath("~/App_Data/clientprintcode.prn");

         // Note that we use a PrintAction for server printing, but we use an XML request for client printing. This is done for two reasons:
         // 1) At time of writing, the PrintAction does not expose the Print-To-File parameters necessary for client printing to work.
         // 2) This demonstrates different mechanisms of using the Print Scheduler API (PrintAction and XML Script approaches)

         // Create XML request string
         XDocument printRequestXMLDocument = new XDocument(
            new XElement("XMLScript", new XAttribute("Version", "2.0"),
               new XElement("Command",
                  new XElement("Print",
                     new XElement("Format", documentFileName),
                     new XElement("PrintSetup",
                        new XElement("Printer", serverPrinterName),
                        new XElement("PrintToFile", true),
                        new XElement("PrintToFileName", printFileName),
                        new XElement("PrintToFileLicense", printLicense))
                     )
                  )
               )
            );
         StringWriter xmlWriter = new StringWriter();
         printRequestXMLDocument.Save(xmlWriter);
         string printRequestXML = xmlWriter.ToString();

         // Execute through the Print Scheduler
         BtXmlAction xmlAction = new BtXmlAction();
         xmlAction.XmlString = printRequestXML;

         try
         {
            BtXmlResult result = xmlAction.RunSynchronous();

            // Get messages to return from the action result.
            List<string> printMessages = new List<string>();

            if (result.Exception != null)
               printMessages.Add(result.Exception.ToString());

            if (result.Messages != null)
            {
               foreach (ActionMessage message in result.Messages)
                  printMessages.Add(message.Text);
            }

            // If successful, grab the print code from the file and then delete it
            if ((result.Status == ActionStatus.Success) && File.Exists(printFileName))
            {
               printCode = File.ReadAllText(printFileName);
               File.Delete(printFileName);
            }

            return printMessages;
         }
         catch (Exception ex)
         {
            // Note that you will receive a System.ServiceModel.EndpointNotFoundException stating that "Could not connect to net.tcp://localhost:8011/PrintScheduler"
            // if the Print Scheduler Service is not running or not responsive. You could catch that and present a more informative error, but for the sake of brevity
            // we just dump out any exceptions back to the print page.

            return new List<string>() { string.Format("Error occurred while printing {0} to printer {1}. Error: {2}", documentFileName, serverPrinterName, ex.ToString()) };
         }
      }
   }
}