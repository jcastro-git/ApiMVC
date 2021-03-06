@Imports  WebLabelPrint.WebLabelPrint.Models
@ModelType PrintDocumentsViewModel

@Section Scripts
   <script src="@Url.Content("~/Scripts/WebLabelPrint/print-documents.js")" type="text/javascript"></script>
End Section

<!-- After a successful client or direct port print, the page is rendered with this hidden field containing the raw client print code. 
     The included Javascript checks for this after client printing has initialized to forward to the printer. Including
     the code in a hidden field is just one possible implementation. You could also return the client print code as a JSON
     controller action, at which point you could just make an AJAX call directly from the client to pick up the print code. -->
@Html.Hidden("ClientPrintCode", Model.ClientPrintCode)

<div class="contentBox">
   <h2>Select a document</h2>
   <p>Click on a document to select it, then choose how you would like to print.</p>

   <!-- Documents list-->
   <div id="DocumentsListContainer">
      @For i As Integer = 0 To Model.DocumentsList.Count - 1
         Dim containerID As String = "document_" + i.ToString()
         Dim document As WebLabelPrintDocument = Model.DocumentsList(i)
         Dim classIfSelected As String = If(i = Model.SelectedDocumentIndex, "activeDocument", String.Empty)
         
         @<div id="@containerID" class="documentIconContainer @classIfSelected" onclick="SelectDocument('@containerID');">
            <div><img src="@Url.Content(document.ThumbnailRelativePath)" width="150px" height="150px" title="@document.DisplayName" alt="@document.DisplayName"/></div>
            <div><span>@document.DisplayName</span></div>
         </div>
      Next
   </div>
   <div style="clear:both;"></div>
</div>

<!-- If we've already attempted to print, dump out any print messages here-->
@If (Model.PrintMessages IsNot Nothing) And (Model.PrintMessages.Count > 0) Then
   @<div class="contentBox">
      <h2>Print Messages</h2>
      <ul>
      @For Each message As String In Model.PrintMessages
         @<li>@message</li>
      Next
      </ul>
   </div>
End If

<!-- Client print not ready message. This is hidden when the client print module calls the OnClientPrintLoad() function. -->
<div class="contentBox" id="ClientPrintNotReadyMessage">   
   <div class="messageBox">
      @If (Settings.ClientPrintModule <> Settings.ClientPrintModuleType.BarTenderWebPrintServiceCORS) Or (Settings.ClientPrintModule <> Settings.ClientPrintModuleType.BarTenderWebPrintServiceIFrame) Then
      
         Dim installerURL As String = Url.Content("~/Content/ClientComponents/BarTenderWebPrintService.exe")
      
         @<span>The BarTender Web Print Service has either not been installed or has not been loaded. </span>
         @<a href="@installerURL" target="_blank">Click here to launch the installer.</a>
         @<span>The included Javascript polls every 5 seconds to determine if it is ready. Note that communication
         with CORS will not work in IE8 and IE9. IE8 and IE9 may also need a browser restart after the service has
         been installed or started for the first time.</span>
         
         @<p id="WebPrintServiceOutOfDateMessage" style="display:none;">The BarTender Web Print Service has been loaded but it is out of date. Please click the link above to install
         the current version of the service.</p>
      ElseIf Settings.ClientPrintModule <> Settings.ClientPrintModuleType.JavaApplet Then
         @<span>The client printing Java applet has not been loaded yet. The Java applet uses LiveConnect to call
         the OnClientPrintLoad() function after it has initialized.</span>
      End If
   </div>
</div>

@Using (Html.BeginForm("PrintDocument", "Home", FormMethod.Post, New With {.id = "PrintForm"}))
   Dim serverPrinterChecked As String = If(Model.PrintType = "Server", "checked", String.Empty)
   Dim clientPrinterChecked As String = If(Model.PrintType = "Client", "checked", String.Empty)
   Dim directPortChecked As String = If(Model.PrintType = "DirectPort", "checked", String.Empty)
         
   @<!-- Contains the index of the document to print. -->
   @Html.Hidden("SelectedDocumentIndex", Model.SelectedDocumentIndex)

   @<!-- Print Type --> 
   @<div class="contentBox" id="printTypeContainer">      
      <h2>Print Type</h2>
      <div>
         <input type="radio" id="SelectedPrintTypeServerPrinter" value="Server" name="PrintType" @serverPrinterChecked onclick="this.blur();" onchange="UpdatePrintGroupVisibility();" />
         <label id="SelectedPrintTypeServerPrinterLabel" for="SelectedPrintTypeServerPrinter">Print to a Server Printer</label>
      </div>
      <div>
         <input type="radio" id="SelectedPrintTypeClientPrinter" value="Client" name="PrintType" @clientPrinterChecked onclick="this.blur();" onchange="UpdatePrintGroupVisibility();" />
         <label id="SelectedPrintTypeClientPrinterLabel" for="SelectedPrintTypeClientPrinter">Print to a Client Printer</label>
      </div>
      <div>
         <input type="radio" id="SelectedPrintTypeDirectPort" value="DirectPort" name="PrintType" @directPortChecked onclick="this.blur();" onchange="UpdatePrintGroupVisibility();" />
         <label id="SelectedPrintTypeDirectPortLabel" for="SelectedPrintTypeDirectPort">Print directly to a port on the client</label>
      </div>
   </div>
   
   @<!-- Server Printer Selection-->
   @<div class="contentBox" id="serverPrinterContainer">      
      <h2>Server Printer selection</h2>
      <p>When printing to a server printer, this is the printer that BarTender will print directly to. When printing to a client
      printer, or directly to a port, this is the printer that BarTender will use to generate the print code that is ultimately
      sent to the client printer.</p>
         
      <label for="SelectedServerPrinterName">Server Printer:</label>
      <select id="SelectedServerPrinterName" name="SelectedServerPrinterName">
         @For Each serverPrinterInfo As ServerPrinterInfo In Model.ServerPrintersList
               Dim selectedAttribute As String = If(serverPrinterInfo.PrinterName = Model.SelectedServerPrinterName, "selected", String.Empty)
               @<option value="@serverPrinterInfo.ExportToJson()" @selectedAttribute>@serverPrinterInfo.PrinterName</option>
         Next
      </select>
   </div>
   
   @<!-- Client Printer (each dropdown entry is added in clientside Javascript) -->
   @<div class="contentBox" id="clientPrinterContainer">      
      <h2>Client Printer selection</h2>
      <p>When printing to a server printer or directly to a port, this is unused. When printing to a client printer, this is the printer
      that the client print module will print to after the print code has been generated by BarTender using the selected server printer.
      Note that printing to a client printer requires that a valid Print License is generated. When the client printers list on this page
      is initially loaded, or when the selection changes, the Javascript for this page makes a call to the client print module to generate
      a license for the selected client printer. It is then stored in a hidden form field so that it can be processed on the server.</p>
         
      <label for="SelectedClientPrinterName">Client Printer:</label>
      <select id="SelectedClientPrinterName" name="SelectedClientPrinterName">
      </select>
   </div>
   
   @<!-- Direct Port information (each dropdown entry is added in clientside Javascript) -->
   @<div class="contentBox" id="directPortContainer">      
      <h2>Direct Port information</h2>
      <p>This is only used when the Print Type is set to &quot;Print directly to a port on the client&quot;. </p>

      <label for="SelectedDirectPort">Select Port:</label>
      <select id="SelectedDirectPort" name="SelectedDirectPort" onchange="UpdatePrintGroupVisibility();">
      </select>

      <!-- TCP / IP Port Entry-->
      <div id="directPortIPEntry" style="margin-top:10px;">
         <label for="DirectPortIPAddress">IP Address (IPv4 or IPv6):</label>
         <input type="text" id="DirectPortIPAddress" name="DirectPortIPAddress" value="@Model.DirectPortIPAddress"/>

         <label for="DirectPortPortNumber">Port Number:</label>
         <input type="text" id="DirectPortPortNumber" name="DirectPortPortNumber" value="@Model.DirectPortPortNumber" />
      </div>
   </div>
   
   @<!-- Contains the print license generated client-side for the selected client printer or port. -->
   @Html.Hidden("ClientPrintLicense", Model.ClientPrintLicense)
   
   @<!-- These are used to maintain selection when the page is rerendered -->
   @Html.Hidden("LastSelectedClientPrinterName", Model.SelectedClientPrinterName)
   @Html.Hidden("IsIPPrinterSelected", Model.IsIPPrinterSelected())
   @Html.Hidden("SelectedDirectPortValue", Model.SelectedDirectPort)

   @<!-- Submit button-->
   @<div class="contentBox">
      <button type="button" onclick="SetPrintLicenseAndPrint();">Print Document</button>
   </div>
End Using
