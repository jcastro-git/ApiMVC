@Imports WebLabelPrint.WebLabelPrint.Models

@ModelType List(Of ServerPrinterInfo)

@Section Scripts
   <script src="@Url.Content("~/Scripts/WebLabelPrint/display-printers.js")" type="text/javascript"></script>
End Section

<!-- Server printers -->
<div class="contentBox">
   <h2>Server Printers</h2>
   <p>These printers are installed on the web server.</p>
   <table>
      <thead>
         <tr class="TableHeader">
            <td>Printer Name</td>
            <td>Driver Name</td>
            <td>Location</td>
            <td>Port</td>
            <td>IsDefault</td>
         </tr>
      </thead>
      <tbody>
      @For Each serverPrinter As ServerPrinterInfo In Model
         @<tr> 
            <td>@serverPrinter.PrinterName</td>
            <td>@serverPrinter.DriverName</td>
            <td>@serverPrinter.Location</td>
            <td>@serverPrinter.PortName</td>
            <td>@serverPrinter.IsDefault.ToString()</td>
         </tr>
      Next

      </tbody>
   </table>
</div>

<!-- Client printers. The tbody section is filled in when the client print module has loaded. -->
<div class="contentBox">
   <h2>Client Printers</h2>
   <p>These printers are installed on the client computer.</p>

   <!-- Client print not ready message. This is hidden when the client print module calls the OnClientPrintLoad() function. -->
   <div class="messageBox" id="ClientPrintNotReadyMessage">
      @If (Settings.ClientPrintModule = Settings.ClientPrintModuleType.BarTenderWebPrintServiceCORS) Or _
         (Settings.ClientPrintModule = Settings.ClientPrintModuleType.BarTenderWebPrintServiceIFrame) Then
         
         Dim installerURL As String = Url.Content("~/Content/ClientComponents/BarTenderWebPrintService.exe")
      
         @<span>The BarTender Web Print Service has either not been installed or has not been loaded. </span>
         @<a href="@installerURL" target="_blank">Click here to launch the installer.</a>
         @<span>The included Javascript polls every 5 seconds to determine if it is ready. Note that communication
         with CORS will not work in IE8 and IE9. IE8 and IE9 may also need a browser restart after the service has
         been installed or started for the first time.</span>
         
         @<p id="WebPrintServiceOutOfDateMessage" style="display:none;">The BarTender Web Print Service has been loaded but it is out of date. Please click the link above to install
         the current version of the service.</p>
      ElseIf Settings.ClientPrintModule = Settings.ClientPrintModuleType.JavaApplet Then
         @<span>The client printing Java applet has not been loaded yet. The Java applet uses LiveConnect to call
         the OnClientPrintLoad() function after it has initialized.</span>
      End If
   </div>

   <table id="DisplayPrinters_ClientPrintersTable">
      <thead>
         <tr class="tableHeader">
            <td>Printer Name</td>
            <td>Driver Name</td>
            <td>Location</td>
            <td>Port</td>
            <td>IsDefault</td>
         </tr>
      </thead>
      <tbody>
      </tbody>
   </table>
</div>