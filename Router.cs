using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Data.Sqlite;

namespace Kursova
{
    public class Router
    {
        private DatabaseManager databaseManager;
        private Dictionary<string, string> routingTable; // Таблиця маршрутизації

        // Властивість для доступу до таблиці маршрутизації ззовні
        public Dictionary<string, string> RoutingTable => routingTable;

        // Конструктор класу Router
        public Router(string connectionString)
        {
            // Ініціалізуємо базу даних та таблицю маршрутизації
            databaseManager = new DatabaseManager(connectionString);
            routingTable = new Dictionary<string, string>();
        }

        // Метод для завантаження таблиці маршрутизації з бази даних
        public void LoadRoutingTableFromDatabase()
        {
            // Очищення словника перед завантаженням нових даних
            routingTable.Clear();

            // Завантаження таблиці маршрутизації з бази даних
            DataTable dataTable = databaseManager.ExecuteQuery("SELECT * FROM RoutingTable");
            foreach (DataRow row in dataTable.Rows)
            {
                string destination = row["Destination"].ToString();
                string nextHop = row["NextHop"].ToString();
                routingTable[destination] = nextHop;
            }
        }

        // Метод для збереження таблиці маршрутизації в базі даних
        public void SaveRoutingTableToDatabase()
        {
            // Зберігаємо таблицю маршрутизації в базі даних
            foreach (var kvp in routingTable)
            {
                string destination = kvp.Key;
                string nextHop = kvp.Value;
                databaseManager.ExecuteQuery($"INSERT OR REPLACE INTO RoutingTable (Destination, NextHop) VALUES ('{destination}', '{nextHop}')");
            }
        }

        // Метод для отримання пакету та його пересилання
        public void ReceivePacket(Packet packet)
        {
            // Отримуємо пакет для пересилання
            string destination = packet.GetDestination();
            if (routingTable.ContainsKey(destination))
            {
                // Якщо адреса призначення є в таблиці маршрутизації, пересилаємо пакет
                string nextHop = routingTable[destination];
                ForwardPacket(packet, nextHop);
            }
            else
            {
                // Якщо адреса призначення відсутня в таблиці маршрутизації, відхиляємо пакет
                RejectPacket(packet);
            }
        }

        // Метод для додавання маршруту до таблиці маршрутизації
        public void AddRoute(string destination, string nextHop)
        {
            // Перевірка на валідність IP-адрес
            if (ValidateIPAddress(destination) && ValidateIPAddress(nextHop))
            {
                if (!routingTable.ContainsKey(destination))
                {
                    routingTable[destination] = nextHop;
                    try
                    {
                        // Додаємо маршрут до бази даних
                        databaseManager.ExecuteQuery($"INSERT INTO RoutingTable (Destination, NextHop) VALUES ('{destination}', '{nextHop}')");
                        // Повідомлення про успішне додавання маршруту
                        MessageBox.Show($"Маршрут для призначення {destination} успішно додано.", "Маршрут додано", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка додавання маршруту до бази даних: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                        // Поверніть стан до попереднього, якщо виникла помилка
                        routingTable.Remove(destination);
                    }
                }
                else
                {
                    // Повідомлення про те, що маршрут вже існує
                    MessageBox.Show($"Маршрут для призначення {destination} вже існує.", "Маршрут існує", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Недійсний формат IP-адреси.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод для відредагування маршруту
        public void EditRoute(string oldDestination, string newDestination, string newNextHop)
        {
            if (routingTable.ContainsKey(oldDestination))
            {
                // Оновлюємо значення в словнику для старого призначення
                routingTable[oldDestination] = newNextHop; // Оновлюємо значення NextHop
                routingTable.Remove(oldDestination); // Видаляємо старий запис зі словника

                try
                {
                    // Оновлюємо запис у базі даних, враховуючи нові значення для Destination та NextHop
                    databaseManager.ExecuteQuery($"UPDATE RoutingTable SET Destination = '{newDestination}', NextHop = '{newNextHop}' WHERE Destination = '{oldDestination}'");

                    // Додаємо оновлений запис до словника
                    routingTable[newDestination] = newNextHop;

                    // Виводимо повідомлення про успішну операцію
                    MessageBox.Show($"Маршрут для призначення {oldDestination} успішно відредаговано.", "Маршрут відредаговано", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при оновленні запису в базі даних: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    // Повертаємо попереднє значення, якщо сталася помилка
                    routingTable[oldDestination] = newNextHop;
                }
            }
            else
            {
                MessageBox.Show($"Маршрут для призначення {oldDestination} не існує.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод для видалення маршруту
        public void DeleteRoute(string destination)
        {
            if (RouteExists(destination))
            {
                if (routingTable.TryGetValue(destination, out string nextHop))
                {
                    // Видалення маршруту із внутрішнього словника
                    routingTable.Remove(destination);

                    // Видалення маршруту з бази даних
                    try
                    {
                        databaseManager.ExecuteQuery($"DELETE FROM RoutingTable WHERE Destination = '{destination}'");
                        MessageBox.Show($"Маршрут для призначення {destination} успішно видалено з бази даних.", "Маршрут видалено", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка видалення маршруту з бази даних: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                        // Повертаємо маршрут назад, якщо сталася помилка
                        routingTable[destination] = nextHop;
                    }
                }
                else
                {
                    MessageBox.Show($"Маршрут для призначення {destination} не існує.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show($"Маршрут для призначення {destination} не існує.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод для пересилання пакету до наступного кроку
        private void ForwardPacket(Packet packet, string nextHop)
        {
            // Реалізуємо пересилання пакету до наступного кроку
            MessageBox.Show($"Пакет переслано на {nextHop}", "Пакет переслано", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Метод для відхилення пакету
        private void RejectPacket(Packet packet)
        {
            // Реалізуємо відхилення пакету
            MessageBox.Show($"Пакет відхилений: Призначення {packet.GetDestination()} не знайдено в таблиці маршрутизації", "Пакет відхилений", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // Метод для перевірки правильності формату IP-адреси
        private bool ValidateIPAddress(string ipAddress)
        {
            // Проста перевірка на правильність формату IP-адреси за допомогою регулярного виразу
            string pattern = @"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(ipAddress);
        }

        // Метод для перевірки існування маршруту за вказаною адресою призначення
        private bool RouteExists(string destination)
        {
            if (destination != null)
            {
                return routingTable.ContainsKey(destination);
            }
            else
            {
                return false;
            }
        }
    }
}
