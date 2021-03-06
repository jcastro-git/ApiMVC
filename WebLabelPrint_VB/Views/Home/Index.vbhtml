<div class="contentBox">
   <h2>Web Label Print Sample</h2>
   <p>
      The Web Label Print Sample application demonstrates how to use the BarTender Print Scheduler service along with our client printing technologies
      to print from a server to a remote internet client through the World Wide Web. The application, which inclues all source code, allows the user to:
   </p>
   <ul>
      <li>Print BarTender documents, on local printers (connected directly to the server or through a LAN or WAN) using standard Windows printing and on
      remote printers accessed via the Internet</li>
      <li>Select which type of remote printing to use: BarTender Web Print Service, or a Java applet</li>
      <li>Explore different mechanisms of communicating with the Web Print Service (IFrame or CORS)</li>
      <li>Display all printers installed on the web server and internet-accessed client computers.</li>
   </ul>

   <h2>Web Sample System Requirements</h2>
   <p>Running this sample requires the following software to be installed on the server:</p>
   <ul>
      <li>BarTender 11.0 or later activated as the Enterprise Automation edition.</li>
      <li>Visual Studio 2010 or higher</li>
      <li>Microsoft .NET Framework 4.0</li>
      <li>Internet Information Services (IIS) 7.0 or later</li>
      <li>ASP.NET MVC 4.0</li>
   </ul>

   <h2>Web Print Service (via IFrame) System Requirements</h2>
   <p>Clients running the Web Print Service (communication over IFrame) have the following requirements:</p>
   <ul>
      <li>Any Windows Desktop operating system</li>
      <li>Microsoft .NET Framework 4.0</li>
      <li>Browsers: IE8+, Chrome, Firefox. This is the most compatible approach.</li>
   </ul>

   <h2>Web Print Service (via CORS) System Requirements</h2>
   <p>Clients running the Web Print Service (communication over CORS) have the following requirements:</p>
   <ul>
      <li>Any Windows Desktop operating system</li>
      <li>Microsoft .NET Framework 4.0</li>
      <li>Browsers: IE10+, Chrome, Firefox. This is a much simpler and standards-compliant way of communicating
      with cross-domain web services, but it is not compatible with older versions of IE.</li>
   </ul>

   <h2>Java Applet system requirements</h2>
   <p>Clients running the Java applet have the following system requirements:</p>
   <ul>
      <li>Any desktop operating system (Windows, Mac, Linux). Mobile operating systems are unable to run the Java applet.</li>
      <li>Any web browser capable of running Java applets.</li>
      <li>This is the most broadly compatible approach to client printing, but it lacks many features and performance
      compared to the Web Printing Service.</li>
   </ul>
</div>