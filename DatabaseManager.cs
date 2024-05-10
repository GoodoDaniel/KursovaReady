using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using Microsoft.Data.Sqlite;

namespace Kursova
{
    public class DatabaseManager
    {
        private SqliteConnection connection;

        // Конструктор класу, який ініціалізує об'єкт підключення до бази даних.
        public DatabaseManager(string connectionString)
        {
            connection = new SqliteConnection(connectionString);
            // Ініціалізація та підключення до бази даних
        }

        // Метод для відкриття з'єднання з базою даних.
        public void OpenConnection()
        {
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка відкриття з'єднання з базою даних: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод для закриття з'єднання з базою даних.
        public void CloseConnection()
        {
            try
            {
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка закриття з'єднання з базою даних: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод для виконання запиту до бази даних та повернення результатів у вигляді DataTable.
        public DataTable ExecuteQuery(string query)
        {
            DataTable dataTable = new DataTable();
            try
            {
                connection.Open(); // Відкрити з'єднання
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка виконання запиту до бази даних: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connection.Close(); // Закрити з'єднання
            }
            return dataTable;
        }
    }
}
