Imports System.Collections.Generic
Imports System.Linq
Imports System.Web

Namespace WebLabelPrint.Models
   ''' <summary>
   ''' Miscellaneous supporting functions.
   ''' </summary>
   Public Module Utility
      Private _random As New Random()

      ''' <summary>
      ''' Returns a random number between min and max.
      ''' </summary>
      Public Function GetRandom(min As Integer, max As Integer) As Integer
         Return _random.[Next](min, max)
      End Function
   End Module
End Namespace