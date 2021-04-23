
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web

Imports System.Web.Script.Serialization

Namespace WebLabelPrint.Models
   ''' <summary>
   ''' View Model for the Print Documents page. 
   ''' </summary>
   Public Class PrintDocumentsViewModel
      ''' <summary>
      ''' Contains the list of documents to display.
      ''' </summary>
      Public Property DocumentsList() As List(Of WebLabelPrintDocument)
         Get
            Return m_DocumentsList
         End Get
         Set(value As List(Of WebLabelPrintDocument))
            m_DocumentsList = Value
         End Set
      End Property
      Private m_DocumentsList As List(Of WebLabelPrintDocument)

      ''' <summary>
      ''' Contains the server printers.
      ''' </summary>
      Public Property ServerPrintersList() As List(Of ServerPrinterInfo)
         Get
            Return m_ServerPrintersList
         End Get
         Set(value As List(Of ServerPrinterInfo))
            m_ServerPrintersList = Value
         End Set
      End Property
      Private m_ServerPrintersList As List(Of ServerPrinterInfo)

      ''' <summary>
      ''' The index of the currently selected document.
      ''' </summary>
      Public Property SelectedDocumentIndex() As Integer
         Get
            Return m_SelectedDocumentIndex
         End Get
         Set(value As Integer)
            m_SelectedDocumentIndex = Value
         End Set
      End Property
      Private m_SelectedDocumentIndex As Integer

      ''' <summary>
      ''' Determines if we're printing to a server or client printer. Values are "Server" or "Client" or "DirectPort".
      ''' </summary>
      Public Property PrintType() As String
         Get
            Return m_PrintType
         End Get
         Set(value As String)
            m_PrintType = Value
         End Set
      End Property
      Private m_PrintType As String

      ''' <summary>
      ''' The name of the selected server printer. This is the printer that BarTender will print directly to if
      ''' PrintType is set to "Server". If PrintType is set to "Client", this is the printer that BarTender will
      ''' use to generate the print code that is ultimately sent to the client printer.
      ''' </summary>
      Public Property SelectedServerPrinterName() As String
         Get
            Return m_SelectedServerPrinterName
         End Get
         Set(value As String)
            m_SelectedServerPrinterName = Value
         End Set
      End Property
      Private m_SelectedServerPrinterName As String

      ''' <summary>
      ''' The name of the selected client printer. If PrintType is set to "Client", the client print module will
      ''' print to this printer after the job has been processed by BarTender.
      ''' </summary>
      Public Property SelectedClientPrinterName() As String
         Get
            Return m_SelectedClientPrinterName
         End Get
         Set(value As String)
            m_SelectedClientPrinterName = Value
         End Set
      End Property
      Private m_SelectedClientPrinterName As String

      ''' <summary>
      ''' The port value used when PrintType is set to "DirectPort".
      ''' </summary>
      Public Property SelectedDirectPort() As String
         Get
            Return m_SelectedDirectPort
         End Get
         Set(value As String)
            m_SelectedDirectPort = Value
         End Set
      End Property
      Private m_SelectedDirectPort As String

      ''' <summary>
      ''' The IPv4 or IPv6 address used when printing directly to an IP port.
      ''' </summary>
      Public Property DirectPortIPAddress() As String
         Get
            Return m_DirectPortIPAddress
         End Get
         Set(value As String)
            m_DirectPortIPAddress = Value
         End Set
      End Property
      Private m_DirectPortIPAddress As String

      ''' <summary>
      ''' The port number used when printing directly to an IP port.
      ''' </summary>
      Public Property DirectPortPortNumber() As String
         Get
            Return m_DirectPortPortNumber
         End Get
         Set(value As String)
            m_DirectPortPortNumber = Value
         End Set
      End Property
      Private m_DirectPortPortNumber As String

      ''' <summary>
      ''' This contains the print license to use for the selected client printer. Any time you print to a client printer,
      ''' a valid license must be generated. When the list is initially loaded, and when the selection changes,
      ''' the client print module will generate a new print license.
      ''' </summary>
      Public Property ClientPrintLicense() As String
         Get
            Return m_ClientPrintLicense
         End Get
         Set(value As String)
            m_ClientPrintLicense = Value
         End Set
      End Property
      Private m_ClientPrintLicense As String

      ''' <summary>
      ''' Contains a list of messages generated after printing. These are written out to the page after printing.
      ''' </summary>
      Public Property PrintMessages() As List(Of String)
         Get
            Return m_PrintMessages
         End Get
         Set(value As List(Of String))
            m_PrintMessages = Value
         End Set
      End Property
      Private m_PrintMessages As List(Of String)

      ''' <summary>
      ''' This contains the raw print code to send to the client printer if there was a successful client print job.
      ''' </summary>
      Public Property ClientPrintCode() As String
         Get
            Return m_ClientPrintCode
         End Get
         Set(value As String)
            m_ClientPrintCode = Value
         End Set
      End Property
      Private m_ClientPrintCode As String

      ''' <summary>
      ''' Default constructor
      ''' </summary>
      Public Sub New()
         DocumentsList = New List(Of WebLabelPrintDocument)()
         ServerPrintersList = New List(Of ServerPrinterInfo)()
         SelectedDocumentIndex = 0
         PrintType = String.Empty
         SelectedServerPrinterName = String.Empty
         SelectedClientPrinterName = String.Empty
         SelectedDirectPort = String.Empty
         DirectPortIPAddress = String.Empty
         DirectPortPortNumber = String.Empty
         ClientPrintLicense = String.Empty
         PrintMessages = New List(Of String)()
         ClientPrintCode = String.Empty
      End Sub

      ''' <summary>
      ''' Returns whether or not an IP port is selected
      ''' </summary>
      Public Function IsIPPrinterSelected() As Boolean
         If String.IsNullOrEmpty(SelectedDirectPort) Then
            Return False
         End If

         Try
            Dim selectedPortProperties As Dictionary(Of String, Object) = New JavaScriptSerializer().Deserialize(Of Dictionary(Of String, Object))(SelectedDirectPort)
            If (selectedPortProperties IsNot Nothing) AndAlso (selectedPortProperties.ContainsKey("PortType")) Then
               Return DirectCast(selectedPortProperties("PortType"), String) = "ip"
            End If
         Catch
            Return False
         End Try

         Return False
      End Function
   End Class
End Namespace