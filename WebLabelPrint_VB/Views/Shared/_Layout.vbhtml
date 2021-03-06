@Imports  WebLabelPrint.WebLabelPrint.Models

<!DOCTYPE html>
<html>
<head>
   <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
   <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
   <meta charset="utf-8" />
   <meta name="viewport" content="width=device-width" />
   <title>Web Label Print</title>
   @Styles.Render("~/Content/css")

   @Scripts.Render("~/bundles/modernizr")
</head>
<body>
   <!-- Page contents -->
   <div id="MainHeader" style="background-image:url('@Url.Content("~/Content/images/SeaBannerBG.gif")')">
      <img src="@Url.Content("~/Content/images/SeaBannerLeft.gif")" alt="Web Label Print Sample" title="Web Label Print Sample" style="border-width: 0" />
   </div>
   <div id="MainContentWrapper">
      @Html.Partial("NavigatorPartialView", ViewBag.CurrentPage.ToString())
      <div id="MainContentContainer">
         @RenderBody()
      </div>
   </div>

   <!-- Default jquery bundle included with ASP.NET MVC -->
   @Scripts.Render("~/bundles/jquery")

   <!-- Make sure to include the correct javascript file for the intended client print module. You could put this into your script
   bundle if desired. -->
   @If Settings.ClientPrintModule = Settings.ClientPrintModuleType.BarTenderWebPrintServiceIFrame Then
      @<script src="@Url.Content("~/Scripts/WebLabelPrint/clientprint-web-print-service-iframe.js")" type="text/javascript"></script>
   ElseIf (Settings.ClientPrintModule = Settings.ClientPrintModuleType.BarTenderWebPrintServiceCORS) Then
      @<script src="@Url.Content("~/Scripts/WebLabelPrint/clientprint-web-print-service-cors.js")" type="text/javascript"></script>
   ElseIf (Settings.ClientPrintModule = Settings.ClientPrintModuleType.JavaApplet) Then
      @<script src="@Url.Content("~/Scripts/WebLabelPrint/clientprint-java.js")" type="text/javascript"></script>
   End If
   
   @RenderSection("scripts", required:=False)

   @If (Settings.ClientPrintModule = Settings.ClientPrintModuleType.BarTenderWebPrintServiceIFrame) Then
   
      @<!-- If using the web print service via IFrame, make sure to include the iframe from the client (localhost).
      The random number used here is purely to avoid browser caching. It's not strictly required to include that parameter, but do
      note that if the page loads prior to the user having installed the web print service, the iframe itself will actually contain
      the web browser's default 404 error page. The client-side javascript periodically reloads the iframe until it is ready, with a
      new random number to avoid the risk of loading the cached 404 page. There is CSS defined to hide the iframe contents. Note that
      we set it's height greater than 0, and we set visibility and overflow to hidden, rather than the more typical approach of setting
      CSS display to "none". Some anti-virus software complains if we have a truly 0-height iframe, and the iframe and container needs to
      remain in the DOM, which won't happen if CSS display is set to "none". -->

      @<div id="BarTenderWebPrintServiceIframeContainerDiv">
         <iframe src="http://localhost:8632/BarTenderWebPrintService/Landing?rand=@Utility.GetRandom(0, 10000).ToString()" id="BarTenderWebPrintServiceIframe" width="100%" height="20px">
         </iframe>
      </div>
   
   ElseIf (Settings.ClientPrintModule = Settings.ClientPrintModuleType.JavaApplet) Then
      @<!-- The Javascript calling the Java applet needs to know  what the base URL is to location of the JAR file. Store that in a hidden field. -->
      @Html.Hidden("ClientComponentsPath", Url.Content("~/Content/ClientComponents"))
   End If
</body>
</html>
