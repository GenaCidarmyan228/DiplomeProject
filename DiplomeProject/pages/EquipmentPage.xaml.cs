using DiplomeProject;
using KitchenManager.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KitchenManager.Pages
{
    public partial class EquipmentPage : Page
    {
        public EquipmentPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }

        private void UpdateData()
        {
            DGridEquipment.ItemsSource = KitchenBaseEntities.GetContext().Equipment.ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = TxtSearch.Text.ToLower();
            var currentData = KitchenBaseEntities.GetContext().Equipment.ToList();

            currentData = currentData.Where(eq =>
                eq.Title.ToLower().Contains(searchText) ||
                (eq.Description != null && eq.Description.ToLower().Contains(searchText))).ToList();

            DGridEquipment.ItemsSource = currentData;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddEquipmentWindow win = new AddEquipmentWindow();
            win.ShowDialog();
            UpdateData();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = DGridEquipment.SelectedItem as Equipment;
            if (selectedItem == null)
            {
                MessageBox.Show("Выберите товар для редактирования!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AddEquipmentWindow win = new AddEquipmentWindow(selectedItem);
            win.ShowDialog();
            UpdateData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = DGridEquipment.SelectedItem as Equipment;
            if (selectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить товар?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        KitchenBaseEntities.GetContext().Equipment.Remove(selectedItem);
                        KitchenBaseEntities.GetContext().SaveChanges();
                        UpdateData();
                        MessageBox.Show("Товар успешно удален!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка удаления: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}