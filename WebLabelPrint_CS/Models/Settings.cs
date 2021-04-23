using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System.Web.Hosting;

namespace WebLabelPrint.Models
{
   /// <summary>
   /// Simple class for basic setting storage and retrieval. Values are automatically updated from config file when read (if changed),
   /// and setting a value will update the config file.
   /// </summary>
   public static class Settings
   {
      /// <summary>
      /// Determines the type of client print module to use
      /// </summary>
      public enum ClientPrintModuleType
      {
         /// <summary>
         /// The Web Print Service is installed on each client machine and acts as an HTTP REST service to facilitate client printing.
         /// Communication between the webpage to the client printing service is achieved by passing messages to/from a hidden iframe hosted
         /// by the service. This is recommended for maximum browser compatibility (IE8+, Chrome, Firefox).
         /// </summary>
         BarTenderWebPrintServiceIFrame,

         /// <summary>
         /// Similar to the above, but requests to the Web Print Service are made via CORS. This is a more direct way to communicate with the
         /// service rather than using an iframe to pass messages, but it is not supported with IE8 and IE9. It does work with all modern browsers.
         /// </summary>
         BarTenderWebPrintServiceCORS,

         /// <summary>
         /// The older Java applet approach works for all desktop platforms that cannot run the Web Printing Service for whatever reason (non-
         /// Windows desktop, security restrictions, etc).
         /// </summary>
         JavaApplet
      }

      private static string                  _configPath             = string.Empty;
      private static object                  _saveLock               = new object();
      private static DateTime                _lastReadTime           = DateTime.Now;

      private static ClientPrintModuleType   _clientPrintModuleType  = ClientPrintModuleType.BarTenderWebPrintServiceIFrame;

      /// <summary>
      /// The type of client printing module to use.
      /// </summary>
      public static ClientPrintModuleType ClientPrintModule
      {
         get 
         {
            if (IsUpdateRequired())
               InitializeValues();

            return _clientPrintModuleType; 
         }
         set
         {
            lock (_saveLock)
            {
               _clientPrintModuleType = value;
               Save();
            }
         }
      }

      /// <summary>
      /// Initializes the config file path and reads initial values.
      /// </summary>
      static Settings()
      {
         // Find the full config file path. This should be located in "~/App_Data/Config.xml".
         if (string.IsNullOrEmpty(_configPath))
         {
            string basePath = string.Empty;

            if (HttpContext.Current != null)
               basePath = HttpContext.Current.Server.MapPath(@"~/App_Data/");
            else
               basePath = HostingEnvironment.MapPath(@"~/App_Data/");

            _configPath = Path.Combine(basePath, "Config.xml");
         }

         InitializeValues();
      }

      /// <summary>
      /// Updates the settings XML with the current values
      /// </summary>
      private static void Save()
      {
         string clientPrintModuleString;
         switch (ClientPrintModule)
         {
            case ClientPrintModuleType.JavaApplet:
               clientPrintModuleString = "JavaApplet";
               break;
            case ClientPrintModuleType.BarTenderWebPrintServiceCORS:
               clientPrintModuleString = "WebPrintServiceCORS";
               break;
            case ClientPrintModuleType.BarTenderWebPrintServiceIFrame:
            default:
               clientPrintModuleString = "WebPrintServiceIFrame";
               break;
         }

         XDocument doc = new XDocument(new XElement("WebLabelPrintSettings",
            new XElement("ClientPrintModuleType", clientPrintModuleString)
         ));

         doc.Save(_configPath);
         File.SetLastWriteTime(_configPath, DateTime.Now);
      }

      /// <summary>
      /// Read and parse the config settings
      /// </summary>
      private static void InitializeValues() 
      {
         if (string.IsNullOrEmpty(_configPath) || (!File.Exists(_configPath)))
            return;

         XDocument configSettings = XDocument.Load(_configPath);
         XElement rootElement = configSettings.Element("WebLabelPrintSettings");

         // Don't set the properties directly because setting them will trigger a save. Instead set associated
         // member variables.
         if (rootElement != null)
         {
            // Client print module type
            XElement clientPrintModuleType = rootElement.Element("ClientPrintModuleType");
            if ((clientPrintModuleType != null) && (clientPrintModuleType.Value != null))
            {
               switch (clientPrintModuleType.Value)
               {
                  case "JavaApplet":
                     _clientPrintModuleType = ClientPrintModuleType.JavaApplet;
                     break;
                  case "WebPrintServiceCORS":
                     _clientPrintModuleType = ClientPrintModuleType.BarTenderWebPrintServiceCORS;
                     break;
                  case "WebPrintServiceIFrame":
                  default:
                     _clientPrintModuleType = ClientPrintModuleType.BarTenderWebPrintServiceIFrame;
                     break;
               }
            }

            _lastReadTime = DateTime.Now;
         }
      }

      /// <summary>
      /// Returns true if the config file has changed since we last read it.
      /// </summary>
      private static bool IsUpdateRequired()
      {
         DateTime configLastWriteTime = File.GetLastWriteTime(_configPath);
         return configLastWriteTime > _lastReadTime;
      }
   }
}