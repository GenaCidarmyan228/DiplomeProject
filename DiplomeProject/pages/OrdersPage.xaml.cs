using DiplomeProject;
using KitchenManager.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KitchenManager.Pages
{
    public partial class OrdersPage : Page
    {
        public OrdersPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateOrders();
        }

        private void UpdateOrders()
        {
            DGridOrders.ItemsSource = KitchenBaseEntities.GetContext().Orders.ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = TxtSearch.Text.ToLower();
            var currentData = KitchenBaseEntities.GetContext().Orders.ToList();

            currentData = currentData.Where(o =>
                (o.Clients != null && o.Clients.Surname.ToLower().Contains(searchText)) ||
                (o.OrderStatus != null && o.OrderStatus.StatusName.ToLower().Contains(searchText)) ||
                (o.Equipment != null && o.Equipment.Title.ToLower().Contains(searchText))).ToList();

            DGridOrders.ItemsSource = currentData;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddOrderWindow addWindow = new AddOrderWindow();
            addWindow.ShowDialog();
            UpdateOrders();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = DGridOrders.SelectedItem as Orders;
            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для редактирования!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EditOrderWindow editWindow = new EditOrderWindow(selectedOrder);
            editWindow.ShowDialog();
            UpdateOrders();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = DGridOrders.SelectedItem as Orders;
            if (selectedOrder != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить этот заказ?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        KitchenBaseEntities.GetContext().Orders.Remove(selectedOrder);
                        KitchenBaseEntities.GetContext().SaveChanges();
                        UpdateOrders();
                        MessageBox.Show("Заказ успешно удален!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении заказа:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}