using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Data.Sqlite;

namespace Kursova
{
    /// Представляє функціональність додатку, включаючи управління маршрутизатором.
    public class Functionality
    {
        /// Отримує або задає об'єкт маршрутизатора.
        public Router Router { get; private set; }

        public Functionality(string connectionString)
        {
            Router = new Router(connectionString);
            // Додаткова ініціалізація та конфігурація маршрутизатора
        }

        public void StartRouter()
        {
            // Запускаємо функціонування маршрутизатора
            MessageBox.Show("Маршрутизатор запущено...", "Маршрутизатор", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        
    }
}
