using DiplomeProject;
using KitchenManager.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace KitchenManager.Pages
{
    public partial class ServicesPage : Page
    {
        public ServicesPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }

        private void UpdateData()
        {
            DGridServices.ItemsSource = KitchenBaseEntities.GetContext().Services.ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = TxtSearch.Text.ToLower();
            var currentData = KitchenBaseEntities.GetContext().Services.ToList();

            currentData = currentData.Where(s => s.Title.ToLower().Contains(searchText)).ToList();

            DGridServices.ItemsSource = currentData;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddServiceWindow win = new AddServiceWindow();
            win.ShowDialog();
            UpdateData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = DGridServices.SelectedItem as Services;
            if (selectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Удалить выбранную услугу?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        KitchenBaseEntities.GetContext().Services.Remove(selectedItem);
                        KitchenBaseEntities.GetContext().SaveChanges();
                        UpdateData();
                        MessageBox.Show("Услуга успешно удалена!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка (возможно услуга используется в заказах):\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите услугу для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}