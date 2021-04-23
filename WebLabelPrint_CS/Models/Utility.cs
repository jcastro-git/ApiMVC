using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebLabelPrint.Models
{
   /// <summary>
   /// Miscellaneous supporting functions.
   /// </summary>
   public static class Utility
   {
      private static Random _random = new Random();

      /// <summary>
      /// Returns a random number between min and max.
      /// </summary>
      public static int GetRandom(int min, int max)
      {
         return _random.Next(min, max);
      }
   }
}