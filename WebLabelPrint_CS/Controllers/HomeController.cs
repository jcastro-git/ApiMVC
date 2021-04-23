using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WebLabelPrint.Models;
using System.Web.Script.Serialization;

namespace WebLabelPrint.Controllers
{
   public class HomeController : Controller
   {
      /// <summary>
      /// Default entry point to Web Label Print. Shows the "Learn about Sample" page.
      /// </summary>
      [HttpGet]
      public ActionResult Index()
      {
         ViewBag.CurrentPage = "LearnAboutSample";
         return View();
      }

      /// <summary>
      /// Displays information about server and client printers.
      /// </summary>
      [HttpGet]
      public ActionResult DisplayPrinters()
      {
         List<ServerPrinterInfo> serverPrintersList = ServerPrinterInfo.GetServerPrinters();

         ViewBag.CurrentPage = "DisplayPrinters";
         return View(serverPrintersList);
      }

      /// <summary>
      /// Displays available documents with thumbnails and allows printing to printers, client printers, and client ports.
      /// </summary>
      [HttpGet]
      public ActionResult PrintDocuments()
      {
         List<WebLabelPrintDocument> documentsList = WebLabelPrintDocument.GenerateDocumentsList();
         List<ServerPrinterInfo> serverPrintersList = ServerPrinterInfo.GetServerPrinters();

         // Set default view model options
         PrintDocumentsViewModel viewModel = new PrintDocumentsViewModel();
         viewModel.DocumentsList = documentsList;
         viewModel.ServerPrintersList = serverPrintersList;
         viewModel.SelectedDocumentIndex = 0;
         viewModel.PrintType = "Server";
         viewModel.SelectedClientPrinterName = string.Empty;

         // Set the default server printer to the Windows default printer if one is available. Oftentimes, when a web application is hosted
         // as an AppPool account that is different than the current user, we may not be able to get a default printer. In that case, select
         // the first available printer.
         bool hasDefault = false;
         foreach (ServerPrinterInfo printerInfo in serverPrintersList)
         {
            if (printerInfo.IsDefault)
            {
               viewModel.SelectedServerPrinterName = printerInfo.PrinterName;
               hasDefault = true;
               break;
            }
         }
         if (!hasDefault && serverPrintersList.Count > 0)
            viewModel.SelectedServerPrinterName = serverPrintersList[0].PrinterName;

         ViewBag.CurrentPage = "PrintDocuments";
         return View(viewModel);
      }

      /// <summary>
      /// Prints the specified document
      /// </summary>
      [HttpPost]
      public ActionResult PrintDocument(PrintDocumentsViewModel viewModel)
      {
         List<WebLabelPrintDocument> documentsList = WebLabelPrintDocument.GenerateDocumentsList();
         List<ServerPrinterInfo> serverPrintersList = ServerPrinterInfo.GetServerPrinters();

         // The selected server printer actually comes back with the clientside JSON value. Parse that back
         // into a PrinterInfo so that we can get the actual printer name back.
         if (!string.IsNullOrEmpty(viewModel.SelectedServerPrinterName))
         {
            ServerPrinterInfo printerInfo = new JavaScriptSerializer().Deserialize<ServerPrinterInfo>(viewModel.SelectedServerPrinterName);
            viewModel.SelectedServerPrinterName = printerInfo.PrinterName;
         }

         viewModel.DocumentsList = documentsList;
         viewModel.ServerPrintersList = serverPrintersList;

         string documentFileName = documentsList[viewModel.SelectedDocumentIndex].FullPath;

         // Perform the print job. Client and Direct Port print jobs are treated the same on the server.
         if (viewModel.PrintType == "Server")
         {
            viewModel.PrintMessages = PrintSchedulerServiceSupport.PrintToServerPrinter(documentFileName, viewModel.SelectedServerPrinterName);
         }
         else
         {
            string printCode;
            viewModel.PrintMessages = PrintSchedulerServiceSupport.PrintToClientPrinter(documentFileName, viewModel.SelectedServerPrinterName,
                                                                                        viewModel.ClientPrintLicense, out printCode);

            if (!string.IsNullOrEmpty(printCode))
               viewModel.ClientPrintCode = printCode;
         }

         ViewBag.CurrentPage = "PrintDocuments";
         return View("PrintDocuments", viewModel);
      }


      /// <summary>
      /// Displays settings to configure (client print module type to use)
      /// </summary>
      [HttpGet]
      public ActionResult Settings()
      {
         ViewBag.CurrentPage = "Settings";
         ViewBag.SavedChanges = false;
         return View();
      }

      /// <summary>
      /// Updates the settings (client print module type to use)
      /// </summary>
      [HttpPost]
      public ActionResult UpdateSettings(string ClientPrintType)
      {
         switch (ClientPrintType)
         {
            case "JavaApplet":
               Models.Settings.ClientPrintModule = Models.Settings.ClientPrintModuleType.JavaApplet;
               break;
            case "WebPrintServiceCORS":
               Models.Settings.ClientPrintModule = Models.Settings.ClientPrintModuleType.BarTenderWebPrintServiceCORS;
               break;
            case "WebPrintServiceIFrame":
            default:
               Models.Settings.ClientPrintModule = Models.Settings.ClientPrintModuleType.BarTenderWebPrintServiceIFrame;
               break;
         }

         ViewBag.CurrentPage = "Settings";
         ViewBag.SavedChanges = true;
         return View("Settings");
      }
   }
}
