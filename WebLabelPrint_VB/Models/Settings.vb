Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Xml.Linq
Imports System.Web
Imports System.Web.Hosting

Namespace WebLabelPrint.Models
   ''' <summary>
   ''' Simple class for basic setting storage and retrieval. Values are automatically updated from config file when read (if changed),
   ''' and setting a value will update the config file.
   ''' </summary>
   Public Module Settings
      ''' <summary>
      ''' Determines the type of client print module to use
      ''' </summary>
      Public Enum ClientPrintModuleType
         ''' <summary>
         ''' The Web Print Service is installed on each client machine and acts as an HTTP REST service to facilitate client printing.
         ''' Communication between the webpage to the client printing service is achieved by passing messages to/from a hidden iframe hosted
         ''' by the service. This is recommended for maximum browser compatibility (IE8+, Chrome, Firefox).
         ''' </summary>
         BarTenderWebPrintServiceIFrame

         ''' <summary>
         ''' Similar to the above, but requests to the Web Print Service are made via CORS. This is a more direct way to communicate with the
         ''' service rather than using an iframe to pass messages, but it is not supported with IE8 and IE9. It does work with all modern browsers.
         ''' </summary>
         BarTenderWebPrintServiceCORS

         ''' <summary>
         ''' The older Java applet approach works for all desktop platforms that cannot run the Web Printing Service for whatever reason (non-
         ''' Windows desktop, security restrictions, etc).
         ''' </summary>
         JavaApplet
      End Enum

      Private _configPath As String = String.Empty
      Private _saveLock As New Object()
      Private _lastReadTime As DateTime = DateTime.Now

      Private _clientPrintModuleType As ClientPrintModuleType = ClientPrintModuleType.BarTenderWebPrintServiceIFrame

      ''' <summary>
      ''' The type of client printing module to use.
      ''' </summary>
      Public Property ClientPrintModule() As ClientPrintModuleType
         Get
            If IsUpdateRequired() Then
               InitializeValues()
            End If

            Return _clientPrintModuleType
         End Get
         Set(value As ClientPrintModuleType)
            SyncLock _saveLock
               _clientPrintModuleType = value
               Save()
            End SyncLock
         End Set
      End Property

      ''' <summary>
      ''' Initializes the config file path and reads initial values.
      ''' </summary>
      Sub New()
         ' Find the full config file path. This should be located in "~/App_Data/Config.xml".
         If String.IsNullOrEmpty(_configPath) Then
            Dim basePath As String = String.Empty

            If HttpContext.Current IsNot Nothing Then
               basePath = HttpContext.Current.Server.MapPath("~/App_Data/")
            Else
               basePath = HostingEnvironment.MapPath("~/App_Data/")
            End If

            _configPath = Path.Combine(basePath, "Config.xml")
         End If

         InitializeValues()
      End Sub

      ''' <summary>
      ''' Updates the settings XML with the current values
      ''' </summary>
      Private Sub Save()
         Dim clientPrintModuleString As String
         Select Case ClientPrintModule
            Case ClientPrintModuleType.JavaApplet
               clientPrintModuleString = "JavaApplet"
               Exit Select
            Case ClientPrintModuleType.BarTenderWebPrintServiceCORS
               clientPrintModuleString = "WebPrintServiceCORS"
               Exit Select
            Case ClientPrintModuleType.BarTenderWebPrintServiceIFrame
               clientPrintModuleString = "WebPrintServiceIFrame"
               Exit Select
            Case Else
               clientPrintModuleString = "WebPrintServiceIFrame"
               Exit Select
         End Select

         Dim doc As New XDocument(New XElement("WebLabelPrintSettings", New XElement("ClientPrintModuleType", clientPrintModuleString)))

         doc.Save(_configPath)
         File.SetLastWriteTime(_configPath, DateTime.Now)
      End Sub

      ''' <summary>
      ''' Read and parse the config settings
      ''' </summary>
      Private Sub InitializeValues()
         If String.IsNullOrEmpty(_configPath) OrElse (Not File.Exists(_configPath)) Then
            Return
         End If

         Dim configSettings As XDocument = XDocument.Load(_configPath)
         Dim rootElement As XElement = configSettings.Element("WebLabelPrintSettings")

         ' Don't set the properties directly because setting them will trigger a save. Instead set associated
         ' member variables.
         If rootElement IsNot Nothing Then
            ' Client print module type
            Dim clientPrintModuleType__1 As XElement = rootElement.Element("ClientPrintModuleType")
            If (clientPrintModuleType__1 IsNot Nothing) AndAlso (clientPrintModuleType__1.Value IsNot Nothing) Then
               Select Case clientPrintModuleType__1.Value
                  Case "JavaApplet"
                     _clientPrintModuleType = ClientPrintModuleType.JavaApplet
                     Exit Select
                  Case "WebPrintServiceCORS"
                     _clientPrintModuleType = ClientPrintModuleType.BarTenderWebPrintServiceCORS
                     Exit Select
                  Case "WebPrintServiceIFrame"
                     _clientPrintModuleType = ClientPrintModuleType.BarTenderWebPrintServiceIFrame
                     Exit Select
                  Case Else
                     _clientPrintModuleType = ClientPrintModuleType.BarTenderWebPrintServiceIFrame
                     Exit Select
               End Select
            End If

            _lastReadTime = DateTime.Now
         End If
      End Sub

      ''' <summary>
      ''' Returns true if the config file has changed since we last read it.
      ''' </summary>
      Private Function IsUpdateRequired() As Boolean
         Dim configLastWriteTime As DateTime = File.GetLastWriteTime(_configPath)
         Return configLastWriteTime > _lastReadTime
      End Function
   End Module
End Namespace
