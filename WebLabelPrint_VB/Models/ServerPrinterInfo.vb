
Imports System.Collections.Generic
Imports System.Linq
Imports System.Management
Imports System.Web
Imports System.Web.Script.Serialization

Namespace WebLabelPrint.Models
   ''' <summary>
   ''' Contains information about printers installed on the server.
   ''' </summary>
   Public Class ServerPrinterInfo
      Public Property PrinterName() As String
         Get
            Return m_PrinterName
         End Get
         Set(value As String)
            m_PrinterName = value
         End Set
      End Property
      Private m_PrinterName As String
      Public Property DriverName() As String
         Get
            Return m_DriverName
         End Get
         Set(value As String)
            m_DriverName = value
         End Set
      End Property
      Private m_DriverName As String
      Public Property PortName() As String
         Get
            Return m_PortName
         End Get
         Set(value As String)
            m_PortName = value
         End Set
      End Property
      Private m_PortName As String
      Public Property Location() As String
         Get
            Return m_Location
         End Get
         Set(value As String)
            m_Location = value
         End Set
      End Property
      Private m_Location As String
      Public Property IsDefault() As Boolean
         Get
            Return m_IsDefault
         End Get
         Set(value As Boolean)
            m_IsDefault = value
         End Set
      End Property
      Private m_IsDefault As Boolean

      ''' <summary>
      ''' Exports a JSON version of the server's printer information.
      ''' </summary>
      Public Function ExportToJson() As String
         Return New JavaScriptSerializer().Serialize(Me)
      End Function

      ''' <summary>
      ''' Performs a WMI query to return information about printers installed on the server.
      ''' 
      ''' We recommend that you cache the results in memory as this can be slow particularly when
      ''' querying network-connected printers.
      ''' </summary>
      Shared Function GetServerPrinters() As List(Of ServerPrinterInfo)
         Dim serverPrintersList As New List(Of ServerPrinterInfo)()

         Dim query As String = "SELECT Name,PortName,Location,DriverName,Default FROM Win32_Printer"

         Dim searcher As New ManagementObjectSearcher(query)
         Dim collection As ManagementObjectCollection = searcher.[Get]()
         For Each printer As ManagementObject In collection
            ' Printer Name
            Dim printerName As String = String.Empty
            If (printer.Properties("Name") IsNot Nothing) AndAlso (printer.Properties("Name").Value IsNot Nothing) Then
               printerName = printer.Properties("Name").Value.ToString()
            End If

            ' Driver Name (Model Name)
            Dim printerModel As String = String.Empty
            If (printer.Properties("DriverName") IsNot Nothing) AndAlso (printer.Properties("DriverName").Value IsNot Nothing) Then
               printerModel = printer.Properties("DriverName").Value.ToString()
            End If

            ' Port Name
            Dim printerPort As String = String.Empty
            If (printer.Properties("PortName") IsNot Nothing) AndAlso (printer.Properties("PortName").Value IsNot Nothing) Then
               printerPort = printer.Properties("PortName").Value.ToString()
            End If

            ' Location
            Dim printerLocation As String = String.Empty
            If (printer.Properties("Location") IsNot Nothing) AndAlso (printer.Properties("Location").Value IsNot Nothing) Then
               printerLocation = printer.Properties("Location").Value.ToString()
            End If

            ' IsDefault
            Dim isDefault As Boolean = False
            If (printer.Properties("Default") IsNot Nothing) AndAlso (printer.Properties("Default").Value IsNot Nothing) Then
               isDefault = CBool(printer.Properties("Default").Value)
            End If

            Dim printerInfo As New ServerPrinterInfo()
            printerInfo.PrinterName = printerName
            printerInfo.DriverName = printerModel
            printerInfo.PortName = printerPort
            printerInfo.Location = printerLocation
            printerInfo.IsDefault = isDefault

            serverPrintersList.Add(printerInfo)
         Next

         Return serverPrintersList
      End Function
   End Class
End Namespace