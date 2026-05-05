using DiplomeProject;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KitchenManager
{
    public partial class ClientsPage : Page
    {
        public ClientsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateClients();
        }

        private void UpdateClients()
        {
            DGridClients.ItemsSource = KitchenBaseEntities.GetContext().Clients.ToList();
        }

        // поиск
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = TxtSearch.Text.ToLower();
            var currentClients = KitchenBaseEntities.GetContext().Clients.ToList();

            currentClients = currentClients.Where(c =>
                c.Surname.ToLower().Contains(searchText) ||
                c.Name.ToLower().Contains(searchText) ||
                c.Phone.ToLower().Contains(searchText)).ToList();

            DGridClients.ItemsSource = currentClients;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddClientWindow win = new AddClientWindow();
            win.ShowDialog();
            UpdateClients();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = DGridClients.SelectedItem as Clients;
            if (selectedClient == null)
            {
                MessageBox.Show("Выберите клиента для редактирования!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AddClientWindow win = new AddClientWindow(selectedClient);
            win.ShowDialog();
            UpdateClients();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = DGridClients.SelectedItem as Clients;
            if (selectedClient != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить этого клиента?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        KitchenBaseEntities.GetContext().Clients.Remove(selectedClient);
                        KitchenBaseEntities.GetContext().SaveChanges();
                        UpdateClients();
                        MessageBox.Show("Клиент успешно удален!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: невозможно удалить клиента (возможно, на него оформлены заказы).\n\n" + ex.Message, "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}