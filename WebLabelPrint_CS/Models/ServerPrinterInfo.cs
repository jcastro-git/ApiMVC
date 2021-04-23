using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Web;
using System.Web.Script.Serialization;

namespace WebLabelPrint.Models
{
   /// <summary>
   /// Contains information about printers installed on the server.
   /// </summary>
   public class ServerPrinterInfo
   {
      public string PrinterName { get; set; }
      public string DriverName { get; set; }
      public string PortName { get; set; }
      public string Location { get; set; }
      public bool IsDefault { get; set; }

      /// <summary>
      /// Exports a JSON version of the server's printer information.
      /// </summary>
      public string ExportToJson()
      {
         return new JavaScriptSerializer().Serialize(this);
      }

      /// <summary>
      /// Performs a WMI query to return information about printers installed on the server.
      /// 
      /// We recommend that you cache the results in memory as this can be slow particularly when
      /// querying network-connected printers.
      /// </summary>
      public static List<ServerPrinterInfo> GetServerPrinters()
      {
         List<ServerPrinterInfo> serverPrintersList = new List<ServerPrinterInfo>();

         string query = "SELECT Name,PortName,Location,DriverName,Default FROM Win32_Printer";

         ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
         ManagementObjectCollection collection = searcher.Get();
         foreach (ManagementObject printer in collection)
         {
            // Printer Name
            string printerName = string.Empty;
            if ((printer.Properties["Name"] != null) && (printer.Properties["Name"].Value != null))
               printerName = printer.Properties["Name"].Value.ToString();
               
            // Driver Name (Model Name)
            string printerModel = string.Empty;
            if ((printer.Properties["DriverName"] != null) && (printer.Properties["DriverName"].Value != null))
               printerModel = printer.Properties["DriverName"].Value.ToString();

            // Port Name
            string printerPort = string.Empty;
            if ((printer.Properties["PortName"] != null) && (printer.Properties["PortName"].Value != null))
               printerPort = printer.Properties["PortName"].Value.ToString();

            // Location
            string printerLocation = string.Empty;
            if ((printer.Properties["Location"] != null) && (printer.Properties["Location"].Value != null))
               printerLocation = printer.Properties["Location"].Value.ToString();

            // IsDefault
            bool isDefault = false;
            if ((printer.Properties["Default"] != null) && (printer.Properties["Default"].Value != null))
               isDefault = ((bool)printer.Properties["Default"].Value);

            ServerPrinterInfo printerInfo = new ServerPrinterInfo();
            printerInfo.PrinterName = printerName;
            printerInfo.DriverName = printerModel;
            printerInfo.PortName = printerPort;
            printerInfo.Location = printerLocation;
            printerInfo.IsDefault = isDefault;

            serverPrintersList.Add(printerInfo);
         }

         return serverPrintersList;
      }
   }
}