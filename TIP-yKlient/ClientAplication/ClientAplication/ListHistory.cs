using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAplication
{
   /// <summary>
   /// Klasa przechowywująca informacje dla jednego wpisu na liśćie historii rozmów
   /// </summary>
   public class ListHistory
    {
        /// <summary>
        /// przechowuje nazwe użytkownika do/który dzwonił
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// Pprzechowuje informacje o początku rozmowy (dzień)
        /// </summary>
        public string dayBegin { get; set; }
        /// <summary>
        /// Pprzechowuje informacje o początku rozmowy (godzina)
        /// </summary>
        public string hourBegin { get; set; }
        /// <summary>
        /// Pprzechowuje informacje o końcu rozmowy (dzień)
        /// </summary>
        public string dayEnd { get; set; }
        /// <summary>
        /// Pprzechowuje informacje o końcu rozmowy (godzina)
        /// </summary>
        public string hourEnd { get; set; }
    }
}
