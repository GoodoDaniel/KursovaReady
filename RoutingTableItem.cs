using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kursova
{
    /// Представляє елемент таблиці маршрутизації.
    public class RoutingTableItem
    {
        /// Отримує або задає адресу призначення.
        public string Destination { get; set; }

        /// Отримує або задає наступний крок (адресу) для пересилання.
        public string NextHop { get; set; }
    }
}
