using System;
using System.Windows;
using Kursova;

namespace Kursova
{
    public partial class MainWindow : Window
    {
        private Functionality functionality;

        public MainWindow()
        {
            InitializeComponent();
            // Ініціалізація функціональності з рядком підключення до бази даних
            string connectionString = "Data Source=D:\\study VITI\\Projects\\Kursova\\gateway.db;";
            functionality = new Functionality(connectionString);

            // Завантаження даних з бази даних та їх відображення у списку
            LoadRoutingTableFromDatabase();

            // Додавання обробників подій для перевірки введення тексту в TextBox
            DestinationTextBox.TextChanged += TextBox_TextChanged;
            NextHopTextBox.TextChanged += TextBox_TextChanged;

            // Ініціалізація стану кнопки додавання маршруту
            UpdateAddRouteButtonState();

            // Додавання обробників подій для перевірки введення тексту в TextBox
            NewDestinationTextBox.TextChanged += NewTextBox_TextChanged;
            NewNextHopTextBox.TextChanged += NewTextBox_TextChanged;

            // Ініціалізація стану кнопки редагування маршруту
            UpdateEditRouteButtonState();
        }

        // Обробник події зміни тексту у TextBox
        private void TextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            // Оновлення стану кнопки додавання маршруту
            UpdateAddRouteButtonState();
        }

        // Метод для оновлення стану кнопки додавання маршруту
        private void UpdateAddRouteButtonState()
        {
            // Перевірка, чи обидва поля мають текст
            bool isDestinationNotEmpty = !string.IsNullOrWhiteSpace(DestinationTextBox.Text);
            bool isNextHopNotEmpty = !string.IsNullOrWhiteSpace(NextHopTextBox.Text);

            // Вимкнення кнопки, якщо хоча б одне поле порожнє
            AddRouteButton.IsEnabled = isDestinationNotEmpty && isNextHopNotEmpty;
        }

        // Обробник події зміни тексту у TextBox
        private void NewTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            // Оновлення стану кнопки редагування маршруту
            UpdateEditRouteButtonState();
        }

        // Метод для оновлення стану кнопки редагування маршруту
        private void UpdateEditRouteButtonState()
        {
            // Перевірка, чи обидва поля мають текст
            bool isNewDestinationNotEmpty = !string.IsNullOrWhiteSpace(NewDestinationTextBox.Text);
            bool isNewNextHopNotEmpty = !string.IsNullOrWhiteSpace(NewNextHopTextBox.Text);

            // Вимкнення кнопки, якщо хоча б одне поле порожнє
            EditRouteButton.IsEnabled = isNewDestinationNotEmpty && isNewNextHopNotEmpty;
        }

        // Обробник події кнопки "Старт"
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Запуск маршрутизатора
            functionality.StartRouter();
            routerRunning = true; // Встановлюємо стан маршрутизатора на запущений
        }

        // Обробник події кнопки "Додати маршрут"
        private void AddRouteButton_Click(object sender, RoutedEventArgs e)
        {
            // Логіка для додавання маршруту
            string destination = DestinationTextBox.Text;
            string nextHop = NextHopTextBox.Text;
            functionality.Router.AddRoute(destination, nextHop);

            // Оновлення даних у списку після додавання нового маршруту
            LoadRoutingTableFromDatabase();

            // Очищення полів вводу після успішного додавання маршруту
            DestinationTextBox.Text = "";
            NextHopTextBox.Text = "";
        }

        // Обробник події кнопки "Редагувати маршрут"
        private void EditRouteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = RoutingTableListView.SelectedItem;
            if (selectedItem != null)
            {
                if (selectedItem is RoutingTableItem routingTableItem)
                {
                    string oldDestination = routingTableItem.Destination; // Попереднє значення Destination
                    string newDestination = NewDestinationTextBox.Text; // Нове значення Destination
                    string newNextHop = NewNextHopTextBox.Text; // Нове значення NextHop

                    // Викликаємо метод EditRoute, передаючи нові значення Destination і NextHop
                    functionality.Router.EditRoute(oldDestination, newDestination, newNextHop);

                    // Оновлення даних у списку після редагування маршруту
                    LoadRoutingTableFromDatabase();
                }
                else
                {
                    MessageBox.Show("Вибраний елемент не є типом RoutingTableItem.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть маршрут для редагування.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обробник події кнопки "Видалити маршрут"
        private void DeleteRouteButton_Click(object sender, RoutedEventArgs e)
        {
            // Логіка для видалення маршруту
            var selectedItem = RoutingTableListView.SelectedItem;
            if (selectedItem != null)
            {
                if (selectedItem is RoutingTableItem routingTableItem)
                {
                    string destination = routingTableItem.Destination;
                    functionality.Router.DeleteRoute(destination);

                    // Оновлення даних у списку після видалення маршруту з бази даних
                    LoadRoutingTableFromDatabase();
                }
                else
                {
                    MessageBox.Show("Вибраний елемент не є типом RoutingTableItem.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть маршрут для видалення.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обробник події кнопки "Надіслати пакет"
        private void SendPacketButton_Click(object sender, RoutedEventArgs e)
        {
            if (routerRunning)
            {
                // Отримуємо вибраний елемент зі списку маршрутів
                var selectedItem = RoutingTableListView.SelectedItem;
                if (selectedItem != null)
                {
                    if (selectedItem is RoutingTableItem routingTableItem)
                    {
                        // Отримуємо адресу призначення з обраного елемента
                        string destination = routingTableItem.Destination;
                        // Створюємо новий пакет з отриманою адресою призначення
                        Packet packet = new Packet(destination);
                        // Викликаємо метод Send для відправлення пакету
                        packet.Send();
                    }
                    else
                    {
                        MessageBox.Show("Вибраний елемент не є типом RoutingTableItem.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Будь ласка, виберіть маршрут для відправлення пакету.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Маршрутизатор не запущений. Запустіть маршрутизатор, щоб відправляти пакети.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обробник події кнопки "Закрити"
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseApplication();
        }

        // Метод для закриття додатка
        private void CloseApplication()
        {
            Close();
        }
        private void LoadRoutingTableFromDatabase()
        {
            // Очищення списку перед завантаженням нових даних
            RoutingTableListView.Items.Clear();

            // Завантаження таблиці маршрутизації з бази даних
            functionality.Router.LoadRoutingTableFromDatabase();

            // Відображення даних у списку
            foreach (var kvp in functionality.Router.RoutingTable)
            {
                string destination = kvp.Key;
                string nextHop = kvp.Value;
                RoutingTableListView.Items.Add(new RoutingTableItem { Destination = destination, NextHop = nextHop });
            }
        }
        private bool routerRunning = false;
    }
}
