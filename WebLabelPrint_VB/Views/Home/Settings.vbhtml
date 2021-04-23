@Imports WebLabelPrint.WebLabelPrint.Models

<div class="contentBox">
   @Using (Html.BeginForm("UpdateSettings", "Home"))
      
      Dim webPrintServiceIFrameChecked As String = If(Settings.ClientPrintModule = Settings.ClientPrintModuleType.BarTenderWebPrintServiceIFrame, "checked", String.Empty)
      Dim webPrintServiceCORSChecked As String = If(Settings.ClientPrintModule = Settings.ClientPrintModuleType.BarTenderWebPrintServiceCORS, "checked", String.Empty)
      Dim javaAppletChecked As String = If(Settings.ClientPrintModule = Settings.ClientPrintModuleType.JavaApplet, "checked", String.Empty)

      @<p>Select which client print module type to use</p>
      @<div>
         <input type="radio" id="ClientPrintTypeWebPrintServiceIFrame" value="WebPrintServiceIFrame" name="ClientPrintType" @webPrintServiceIFrameChecked />
         <label id="ClientPrintTypeWebPrintServiceIFrameLabel" for="ClientPrintTypeWebPrintServiceIFrame">BarTender Web Print Service (communication via IFrame)</label>
      </div>
      @<div>
        <input type="radio" id="ClientPrintTypeWebPrintServiceCORS" value="WebPrintServiceCORS" name="ClientPrintType" @webPrintServiceCORSChecked />
         <label id="ClientPrintTypeWebPrintServiceCORSLabel" for="ClientPrintTypeWebPrintServiceCORS">BarTender Web Print Service (communication via CORS)</label>
      </div>
      @<div>
         <input type="radio" id="ClientPrintTypeJavaApplet" value="JavaApplet" name="ClientPrintType" @javaAppletChecked />
         <label id="ClientPrintTypeJavaAppletLabel" for="ClientPrintTypeJavaApplet">Java Applet</label>
      </div>
      
      @<div style="margin-top:20px;">
         <input type="submit" value="Save Changes" />
      </div>
      
      If (ViewBag.SavedChanges) Then
         @<div style="margin-top:20px;">
            <label>Successfully saved settings.</label>
         </div>
      End If
   End Using
</div>