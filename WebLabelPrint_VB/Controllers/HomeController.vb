
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc

Imports WebLabelPrint.WebLabelPrint.Models
Imports System.Web.Script.Serialization

Namespace WebLabelPrint.Controllers
   Public Class HomeController
      Inherits Controller
      ''' <summary>
      ''' Default entry point to Web Label Print. Shows the "Learn about Sample" page.
      ''' </summary>
      <HttpGet()> _
      Public Function Index() As ActionResult
         ViewBag.CurrentPage = "LearnAboutSample"
         Return View()
      End Function

      ''' <summary>
      ''' Displays information about server and client printers.
      ''' </summary>
      <HttpGet()> _
      Public Function DisplayPrinters() As ActionResult
         Dim serverPrintersList As List(Of ServerPrinterInfo) = ServerPrinterInfo.GetServerPrinters()

         ViewBag.CurrentPage = "DisplayPrinters"
         Return View(serverPrintersList)
      End Function

      ''' <summary>
      ''' Displays available documents with thumbnails and allows printing to printers, client printers, and client ports.
      ''' </summary>
      <HttpGet()> _
      Public Function PrintDocuments() As ActionResult
         Dim documentsList As List(Of WebLabelPrintDocument) = WebLabelPrintDocument.GenerateDocumentsList()
         Dim serverPrintersList As List(Of ServerPrinterInfo) = ServerPrinterInfo.GetServerPrinters()

         ' Set default view model options
         Dim viewModel As New PrintDocumentsViewModel()
         viewModel.DocumentsList = documentsList
         viewModel.ServerPrintersList = serverPrintersList
         viewModel.SelectedDocumentIndex = 0
         viewModel.PrintType = "Server"
         viewModel.SelectedClientPrinterName = String.Empty

         ' Set the default server printer to the Windows default printer if one is available. Oftentimes, when a web application is hosted
         ' as an AppPool account that is different than the current user, we may not be able to get a default printer. In that case, select
         ' the first available printer.
         Dim hasDefault As Boolean = False
         For Each printerInfo As ServerPrinterInfo In serverPrintersList
            If printerInfo.IsDefault Then
               viewModel.SelectedServerPrinterName = printerInfo.PrinterName
               hasDefault = True
               Exit For
            End If
         Next
         If Not hasDefault AndAlso serverPrintersList.Count > 0 Then
            viewModel.SelectedServerPrinterName = serverPrintersList(0).PrinterName
         End If

         ViewBag.CurrentPage = "PrintDocuments"
         Return View(viewModel)
      End Function

      ''' <summary>
      ''' Prints the specified document
      ''' </summary>
      <HttpPost()> _
      Public Function PrintDocument(viewModel As PrintDocumentsViewModel) As ActionResult
         Dim documentsList As List(Of WebLabelPrintDocument) = WebLabelPrintDocument.GenerateDocumentsList()
         Dim serverPrintersList As List(Of ServerPrinterInfo) = ServerPrinterInfo.GetServerPrinters()

         ' The selected server printer actually comes back with the clientside JSON value. Parse that back
         ' into a PrinterInfo so that we can get the actual printer name back.
         If Not String.IsNullOrEmpty(viewModel.SelectedServerPrinterName) Then
            Dim printerInfo As ServerPrinterInfo = New JavaScriptSerializer().Deserialize(Of ServerPrinterInfo)(viewModel.SelectedServerPrinterName)
            viewModel.SelectedServerPrinterName = printerInfo.PrinterName
         End If

         viewModel.DocumentsList = documentsList
         viewModel.ServerPrintersList = serverPrintersList

         Dim documentFileName As String = documentsList(viewModel.SelectedDocumentIndex).FullPath

         ' Perform the print job. Client and Direct Port print jobs are treated the same on the server.
         If viewModel.PrintType = "Server" Then
            viewModel.PrintMessages = PrintSchedulerServiceSupport.PrintToServerPrinter(documentFileName, viewModel.SelectedServerPrinterName)
         Else
            Dim printCode As String = String.Empty
            viewModel.PrintMessages = PrintSchedulerServiceSupport.PrintToClientPrinter(documentFileName, viewModel.SelectedServerPrinterName, viewModel.ClientPrintLicense, printCode)

            If Not String.IsNullOrEmpty(printCode) Then
               viewModel.ClientPrintCode = printCode
            End If
         End If

         ViewBag.CurrentPage = "PrintDocuments"
         Return View("PrintDocuments", viewModel)
      End Function


      ''' <summary>
      ''' Displays settings to configure (client print module type to use)
      ''' </summary>
      <HttpGet()> _
      Public Function Settings() As ActionResult
         ViewBag.CurrentPage = "Settings"
         ViewBag.SavedChanges = False
         Return View()
      End Function

      ''' <summary>
      ''' Updates the settings (client print module type to use)
      ''' </summary>
      <HttpPost()> _
      Public Function UpdateSettings(ClientPrintType As String) As ActionResult
         Select Case ClientPrintType
            Case "JavaApplet"
               Models.Settings.ClientPrintModule = Models.Settings.ClientPrintModuleType.JavaApplet
               Exit Select
            Case "WebPrintServiceCORS"
               Models.Settings.ClientPrintModule = Models.Settings.ClientPrintModuleType.BarTenderWebPrintServiceCORS
               Exit Select
            Case "WebPrintServiceIFrame"
               Models.Settings.ClientPrintModule = Models.Settings.ClientPrintModuleType.BarTenderWebPrintServiceIFrame
               Exit Select
            Case Else
               Models.Settings.ClientPrintModule = Models.Settings.ClientPrintModuleType.BarTenderWebPrintServiceIFrame
               Exit Select
         End Select

         ViewBag.CurrentPage = "Settings"
         ViewBag.SavedChanges = True
         Return View("Settings")
      End Function
   End Class
End Namespace