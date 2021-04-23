using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Web;

using Seagull.BarTender.Print;

namespace WebLabelPrint.Models
{
   public class WebLabelPrintDocument
   {
      public string FullPath { get; set; }
      public string ThumbnailRelativePath { get; set; }
      public string DisplayName { get; set; }

      /// <summary>
      /// Finds all BarTender documents in the server's Documents directory, generates thumbnails for them,
      /// and returns a list of valid files. In a real application, we recommend implementing some form of caching.
      /// Thumbnail generation can be slow, so our own implementation keeps track of file modification times to
      /// avoid re-generating thumbnails unnecessarily.
      /// </summary>
      public static List<WebLabelPrintDocument> GenerateDocumentsList()
      {
         List<WebLabelPrintDocument> documentsList = new List<WebLabelPrintDocument>();

         string documentsFullPath = HttpContext.Current.Server.MapPath("~/Documents");
         if (!Directory.Exists(documentsFullPath))
            return documentsList;

         foreach (string fileName in Directory.GetFiles(documentsFullPath))
         {
            // Filter for BarTender documents (.btw files)
            if (!fileName.ToLowerInvariant().EndsWith("btw"))
               continue;

            string thumbnailFileName = fileName.Substring(0, fileName.Length - 3) + "png";

            // Use the BarTender .NET Print SDK to generate a thumbnail. Note that this does not go through a Print Engine like many
            // of the other Print SDK functions. Communication with BarTender occurs via the Print Scheduler service.
            using (Image thumbnailImage = LabelFormatThumbnail.Create(fileName, Color.Transparent, 150, 150))
            {
               if (thumbnailImage != null)
               {
                  thumbnailImage.Save(thumbnailFileName);

                  WebLabelPrintDocument document = new WebLabelPrintDocument();
                  document.FullPath = fileName;
                  document.DisplayName = Path.GetFileName(fileName);
                  document.ThumbnailRelativePath = "~/Documents/" + document.DisplayName.Substring(0, document.DisplayName.Length - 3) + "png";

                  documentsList.Add(document);
               }
            }
         }

         return documentsList;
      }
   }
}