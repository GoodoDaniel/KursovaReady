using System;
using System.Collections.Generic;
using System.Windows;

namespace Kursova
{
    public class RoutingTable
    {
        private Dictionary<string, string> routingEntries; // Адреса призначення -> Наступний крок

        public RoutingTable()
        {
            routingEntries = new Dictionary<string, string>();
        }

        // Додавання запису до таблиці маршрутизації
        public void AddEntry(string destinationAddress, string nextHop)
        {
            routingEntries[destinationAddress] = nextHop;

            // Оновлення відображення таблиці маршрутів у інтерфейсі
            UpdateRoutingTable();
        }

        // Видалення запису з таблиці маршрутизації
        public void RemoveEntry(string destinationAddress)
        {
            routingEntries.Remove(destinationAddress);

            // Оновлення відображення таблиці маршрутів у інтерфейсі
            UpdateRoutingTable();
        }

        // Оновлення запису в таблиці маршрутизації
        public void UpdateEntry(string destinationAddress, string nextHop)
        {
            routingEntries[destinationAddress] = nextHop;

            // Оновлення відображення таблиці маршрутів у інтерфейсі
            UpdateRoutingTable();
        }

        // Перевірка наявності адреси призначення у таблиці маршрутизації
        public bool ContainsDestination(string destinationAddress)
        {
            return routingEntries.ContainsKey(destinationAddress);
        }

        // Отримання наступного кроку для вказаної адреси призначення
        public string GetNextHop(string destinationAddress)
        {
            string nextHop = null;
            routingEntries.TryGetValue(destinationAddress, out nextHop);
            return nextHop;
        }

        // Оновлення відображення таблиці маршрутів у інтерфейсі
        private void UpdateRoutingTable()
        {
            // Отримання посилання на єдиний екземпляр вікна через властивість Instance
            MainWindow mainWindow = MainWindow.Instance;

            if (mainWindow != null)
            {
                mainWindow.UpdateRoutingTable();
            }
            else
            {
                MessageBox.Show("Сталася помилка при оновленні таблиці маршрутів: головне вікно не ініціалізовано.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
