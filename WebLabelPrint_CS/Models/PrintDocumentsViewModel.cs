using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Script.Serialization;

namespace WebLabelPrint.Models
{
   /// <summary>
   /// View Model for the Print Documents page. 
   /// </summary>
   public class PrintDocumentsViewModel
   {
      /// <summary>
      /// Contains the list of documents to display.
      /// </summary>
      public List<WebLabelPrintDocument> DocumentsList { get; set; }

      /// <summary>
      /// Contains the server printers.
      /// </summary>
      public List<ServerPrinterInfo> ServerPrintersList { get; set; }

      /// <summary>
      /// The index of the currently selected document.
      /// </summary>
      public int SelectedDocumentIndex { get; set; }

      /// <summary>
      /// Determines if we're printing to a server or client printer. Values are "Server" or "Client" or "DirectPort".
      /// </summary>
      public string PrintType { get; set; }

      /// <summary>
      /// The name of the selected server printer. This is the printer that BarTender will print directly to if
      /// PrintType is set to "Server". If PrintType is set to "Client", this is the printer that BarTender will
      /// use to generate the print code that is ultimately sent to the client printer.
      /// </summary>
      public string SelectedServerPrinterName { get; set; }

      /// <summary>
      /// The name of the selected client printer. If PrintType is set to "Client", the client print module will
      /// print to this printer after the job has been processed by BarTender.
      /// </summary>
      public string SelectedClientPrinterName { get; set; }

      /// <summary>
      /// The port value used when PrintType is set to "DirectPort".
      /// </summary>
      public string SelectedDirectPort { get; set; }

      /// <summary>
      /// The IPv4 or IPv6 address used when printing directly to an IP port.
      /// </summary>
      public string DirectPortIPAddress { get; set; }

      /// <summary>
      /// The port number used when printing directly to an IP port.
      /// </summary>
      public string DirectPortPortNumber { get; set; }      

      /// <summary>
      /// This contains the print license to use for the selected client printer. Any time you print to a client printer,
      /// a valid license must be generated. When the list is initially loaded, and when the selection changes,
      /// the client print module will generate a new print license.
      /// </summary>
      public string ClientPrintLicense { get; set; }

      /// <summary>
      /// Contains a list of messages generated after printing. These are written out to the page after printing.
      /// </summary>
      public List<string> PrintMessages { get; set; }

      /// <summary>
      /// This contains the raw print code to send to the client printer if there was a successful client print job.
      /// </summary>
      public string ClientPrintCode { get; set; }

      /// <summary>
      /// Default constructor
      /// </summary>
      public PrintDocumentsViewModel()
      {
         DocumentsList = new List<WebLabelPrintDocument>();
         ServerPrintersList = new List<ServerPrinterInfo>();
         SelectedDocumentIndex = 0;
         PrintType = string.Empty;
         SelectedServerPrinterName = string.Empty;
         SelectedClientPrinterName = string.Empty;
         SelectedDirectPort = string.Empty;
         DirectPortIPAddress = string.Empty;
         DirectPortPortNumber = string.Empty;
         ClientPrintLicense = string.Empty;
         PrintMessages = new List<string>();
         ClientPrintCode = string.Empty;
      }

      /// <summary>
      /// Returns whether or not an IP port is selected
      /// </summary>
      public bool IsIPPrinterSelected()
      {
         if (string.IsNullOrEmpty(SelectedDirectPort))
            return false;

         try
         {
            Dictionary<string, object> selectedPortProperties = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(SelectedDirectPort);
            if ((selectedPortProperties != null) && (selectedPortProperties.ContainsKey("PortType")))
               return (string)selectedPortProperties["PortType"] == "ip";
         }
         catch { return false; }

         return false;
      }
   }
}