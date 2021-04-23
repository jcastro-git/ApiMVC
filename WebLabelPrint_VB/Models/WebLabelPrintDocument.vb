Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports System.IO
Imports System.Web

Imports Seagull.BarTender.Print

Namespace WebLabelPrint.Models
   Public Class WebLabelPrintDocument
      Public Property FullPath() As String
         Get
            Return m_FullPath
         End Get
         Set(value As String)
            m_FullPath = Value
         End Set
      End Property
      Private m_FullPath As String
      Public Property ThumbnailRelativePath() As String
         Get
            Return m_ThumbnailRelativePath
         End Get
         Set(value As String)
            m_ThumbnailRelativePath = Value
         End Set
      End Property
      Private m_ThumbnailRelativePath As String
      Public Property DisplayName() As String
         Get
            Return m_DisplayName
         End Get
         Set(value As String)
            m_DisplayName = Value
         End Set
      End Property
      Private m_DisplayName As String

      ''' <summary>
      ''' Finds all BarTender documents in the server's Documents directory, generates thumbnails for them,
      ''' and returns a list of valid files. In a real application, we recommend implementing some form of caching.
      ''' Thumbnail generation can be slow, so our own implementation keeps track of file modification times to
      ''' avoid re-generating thumbnails unnecessarily.
      ''' </summary>
      Public Shared Function GenerateDocumentsList() As List(Of WebLabelPrintDocument)
         Dim documentsList As New List(Of WebLabelPrintDocument)()

         Dim documentsFullPath As String = HttpContext.Current.Server.MapPath("~/Documents")
         If Not Directory.Exists(documentsFullPath) Then
            Return documentsList
         End If

         For Each fileName As String In Directory.GetFiles(documentsFullPath)
            ' Filter for BarTender documents (.btw files)
            If Not fileName.ToLowerInvariant().EndsWith("btw") Then
               Continue For
            End If

            Dim thumbnailFileName As String = fileName.Substring(0, fileName.Length - 3) + "png"

            ' Use the BarTender .NET Print SDK to generate a thumbnail. Note that this does not go through a Print Engine like many
            ' of the other Print SDK functions. Communication with BarTender occurs via the Print Scheduler service.
            Using thumbnailImage As Image = LabelFormatThumbnail.Create(fileName, Color.Transparent, 150, 150)
               If thumbnailImage IsNot Nothing Then
                  thumbnailImage.Save(thumbnailFileName)

                  Dim document As New WebLabelPrintDocument()
                  document.FullPath = fileName
                  document.DisplayName = Path.GetFileName(fileName)
                  document.ThumbnailRelativePath = "~/Documents/" + document.DisplayName.Substring(0, document.DisplayName.Length - 3) + "png"

                  documentsList.Add(document)
               End If
            End Using
         Next

         Return documentsList
      End Function
   End Class
End Namespace