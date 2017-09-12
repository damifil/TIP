using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAplication
{

    class ListUser
    {
        /// <summary>
        /// przechowuje imie użytkownika
        /// </summary>
        public string name { get; set; }
        /// <summary>
        ///przechowuje wartość czy użytkownik jest aktywny
        /// </summary>
        ///<remarks>
        ///true-dla aktywnego
        ///false dla nieaktywnego
        /// </remarks>
        public string active { get; set; }
    }

}
