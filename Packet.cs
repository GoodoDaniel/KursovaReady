using System;
using System.Windows;

namespace Kursova
{
    public class Packet
    {
        private string destinationAddress;

        // Конструктор класу, що ініціалізує об'єкт пакету з вказаною адресою призначення.
        public Packet(string destinationAddress)
        {
            this.destinationAddress = destinationAddress;
        }

        // Метод для отримання адреси призначення пакету.
        public string GetDestination()
        {
            return destinationAddress;
        }

        // Метод для відправлення пакету із вказаною адресою призначення.
        public void Send()
        {
            // Реалізуємо відправку пакету та виводимо повідомлення.
            MessageBox.Show($"Пакет надіслано на {destinationAddress}", "Пакет відправлено", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
