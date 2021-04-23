@ModelType String
<div id="PageNavigator">
   <div id="PageNavigatorHeader"> Select a Task</div>
   @If Model = "LearnAboutSample" Then
      @<div class="navItemContainer" style="background-color: #EEEEEE;">@Html.ActionLink("Learn about Sample", "Index")</div>
   Else
      @<div class="navItemContainer">@Html.ActionLink("Learn about Sample", "Index")</div>
   End If

   @If Model = "DisplayPrinters" Then
      @<div class="navItemContainer" style="background-color: #EEEEEE;">@Html.ActionLink("Display Printers", "DisplayPrinters")</div>
   Else
      @<div class="navItemContainer">@Html.ActionLink("Display Printers", "DisplayPrinters")</div>
   End If

   @If Model = "PrintDocuments" Then
      @<div class="navItemContainer" style="background-color: #EEEEEE;">@Html.ActionLink("Print Documents", "PrintDocuments")</div>
   Else
      @<div class="navItemContainer">@Html.ActionLink("Print Documents", "PrintDocuments")</div>
   End If

   @If Model = "Settings" Then
      @<div class="navItemContainer" style="background-color: #EEEEEE;">@Html.ActionLink("Settings", "Settings")</div>
   Else
      @<div class="navItemContainer">@Html.ActionLink("Settings", "Settings")</div>
   End If
</div>
