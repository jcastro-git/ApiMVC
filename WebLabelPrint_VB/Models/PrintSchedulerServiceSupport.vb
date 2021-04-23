
Imports System.Collections.Generic
Imports System.Linq
Imports System.IO
Imports System.Web
Imports System.Xml.Linq

Imports Seagull.Services.PrintScheduler

Namespace WebLabelPrint.Models
   ''' <summary>
   ''' Supporting functions to interact with BarTender through the Print Scheduler Service. 
   ''' </summary>
   Public Module PrintSchedulerServiceSupport
      ''' <summary>
      ''' Prints the specified filename to the specified server printer. Note that in a real-world implementation
      ''' there are many other things that you can control such as number of copies and serial numbers,
      ''' database record selection, named datasource values, data entry data, etc.
      ''' 
      ''' Returns a list of print messages to display.
      ''' </summary>
      Public Function PrintToServerPrinter(documentFileName As String, serverPrinterName As String) As List(Of String)
         If String.IsNullOrEmpty(documentFileName) OrElse String.IsNullOrEmpty(serverPrinterName) Then
            Return New List(Of String)() From { _
             "You must specify a filename to print and a printer to print to." _
            }
         End If
         If Not File.Exists(documentFileName) Then
            Return New List(Of String)() From { _
             String.Format("Document {0} does not exist.", documentFileName) _
            }
         End If

         Dim printAction As New PrintAction(documentFileName)
         printAction.Document.PrintSetup.Printer = serverPrinterName

         Try
            Dim result As PrintActionResult = printAction.RunSynchronous()

            ' Get messages to return from the action result.
            Dim printMessages As New List(Of String)()

            If result.Exception IsNot Nothing Then
               printMessages.Add(result.Exception.ToString())
            End If

            If result.Messages IsNot Nothing Then
               For Each message As ActionMessage In result.Messages
                  printMessages.Add(message.Text)
               Next
            End If

            Return printMessages
         Catch ex As Exception
            ' Note that you will receive a System.ServiceModel.EndpointNotFoundException stating that "Could not connect to net.tcp://localhost:8011/PrintScheduler"
            ' if the Print Scheduler Service is not running or not responsive. You could catch that and present a more informative error, but for the sake of brevity
            ' we just dump out any exceptions back to the print page.

            Return New List(Of String)() From { _
             String.Format("Error occurred while printing {0} to printer {1}. Error: {2}", documentFileName, serverPrinterName, ex.ToString()) _
            }
         End Try
      End Function

      ''' <summary>
      ''' Prints the specified document to a client printer. This works by doing a Print-To-File using the specified server printer and license.
      ''' We then capture the file contents into the "printCode" output parameter. The print code is posted in a client-side hidden field on the page
      ''' which the client-print module picks up to send to the destination printer. Rather than using a client-side hidden field, another potential
      ''' implementation could return the print code directly in a JSON controller action, so that you could directly get print code via AJAX calls.
      ''' </summary>
      Public Function PrintToClientPrinter(documentFileName As String, serverPrinterName As String, printLicense As String, ByRef printCode As String) As List(Of String)
         printCode = String.Empty

         If String.IsNullOrEmpty(documentFileName) OrElse String.IsNullOrEmpty(serverPrinterName) Then
            Return New List(Of String)() From { _
             "You must specify a filename to print and a server printer to print to." _
            }
         End If
         If String.IsNullOrEmpty(printLicense) Then
            Return New List(Of String)() From { _
             "You must specify a print license to print to a client printer." _
            }
         End If
         If Not File.Exists(documentFileName) Then
            Return New List(Of String)() From { _
             String.Format("Document {0} does not exist.", documentFileName) _
            }
         End If

         ' Get a full filename for where we want to send the print code to
         Dim printFileName As String = HttpContext.Current.Server.MapPath("~/App_Data/clientprintcode.prn")

         ' Note that we use a PrintAction for server printing, but we use an XML request for client printing. This is done for two reasons:
         ' 1) At time of writing, the PrintAction does not expose the Print-To-File parameters necessary for client printing to work.
         ' 2) This demonstrates different mechanisms of using the Print Scheduler API (PrintAction and XML Script approaches)

         ' Create XML request string
         Dim printRequestXMLDocument As New XDocument(New XElement("XMLScript", New XAttribute("Version", "2.0"), New XElement("Command", New XElement("Print", New XElement("Format", documentFileName), New XElement("PrintSetup", New XElement("Printer", serverPrinterName), New XElement("PrintToFile", True), New XElement("PrintToFileName", printFileName), New XElement("PrintToFileLicense", printLicense))))))
         Dim xmlWriter As New StringWriter()
         printRequestXMLDocument.Save(xmlWriter)
         Dim printRequestXML As String = xmlWriter.ToString()

         ' Execute through the Print Scheduler
         Dim xmlAction As New BtXmlAction()
         xmlAction.XmlString = printRequestXML

         Try
            Dim result As BtXmlResult = xmlAction.RunSynchronous()

            ' Get messages to return from the action result.
            Dim printMessages As New List(Of String)()

            If result.Exception IsNot Nothing Then
               printMessages.Add(result.Exception.ToString())
            End If

            If result.Messages IsNot Nothing Then
               For Each message As ActionMessage In result.Messages
                  printMessages.Add(message.Text)
               Next
            End If

            ' If successful, grab the print code from the file and then delete it
            If (result.Status = ActionStatus.Success) AndAlso File.Exists(printFileName) Then
               printCode = File.ReadAllText(printFileName)
               File.Delete(printFileName)
            End If

            Return printMessages
         Catch ex As Exception
            ' Note that you will receive a System.ServiceModel.EndpointNotFoundException stating that "Could not connect to net.tcp://localhost:8011/PrintScheduler"
            ' if the Print Scheduler Service is not running or not responsive. You could catch that and present a more informative error, but for the sake of brevity
            ' we just dump out any exceptions back to the print page.

            Return New List(Of String)() From { _
             String.Format("Error occurred while printing {0} to printer {1}. Error: {2}", documentFileName, serverPrinterName, ex.ToString()) _
            }
         End Try
      End Function
   End Module
End Namespace