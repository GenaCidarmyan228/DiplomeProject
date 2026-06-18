using DiplomeProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace KitchenManager.Windows
{
    public class StockColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int stock)
            {
                if (stock >= 10) return (Brush)new BrushConverter().ConvertFrom("#28A745");
                if (stock >= 3) return (Brush)new BrushConverter().ConvertFrom("#FFC107");
                return (Brush)new BrushConverter().ConvertFrom("#DC3545");
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class AddOrderWindow : Window
    {
        public AddOrderWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ComboClients.ItemsSource = KitchenBaseEntities.GetContext().Clients.ToList();
            ListEquipment.ItemsSource = KitchenBaseEntities.GetContext().Equipment.ToList();
            ListServices.ItemsSource = KitchenBaseEntities.GetContext().Services.ToList();
            ListEmployees.ItemsSource = KitchenBaseEntities.GetContext().Employees.ToList(); // Загружаем список сотрудников
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            
            if (ComboClients.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите клиента!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

           
            var selectedEquipments = ListEquipment.SelectedItems.Cast<Equipment>().ToList();
            var selectedServices = ListServices.SelectedItems.Cast<Services>().ToList();
            var selectedEmployees = ListEmployees.SelectedItems.Cast<Employees>().ToList();

            if (selectedEmployees.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одного ответственного сотрудника!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedEquipments.Count == 0 && selectedServices.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы один товар или услугу для оформления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            
            List<Equipment> equipmentToOrder = new List<Equipment>();
            foreach (var eq in selectedEquipments)
            {
                int qty = eq.SelectedQuantity;
                if (qty <= 0) qty = 1;

                if (eq.StockQuantity < qty)
                {
                    MessageBox.Show($"Недостаточно товара «{eq.Title}» на складе!\nВ наличии: {eq.StockQuantity} шт.\nЗапрошено: {qty} шт.", "Ошибка склада", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                for (int i = 0; i < qty; i++)
                {
                    equipmentToOrder.Add(eq);
                }
            }

            try
            {
                
                int maxCount = Math.Max(Math.Max(equipmentToOrder.Count, selectedServices.Count), selectedEmployees.Count);

               
                DateTime currentOrderTime = DateTime.Now;

                for (int i = 0; i < maxCount; i++)
                {
                    Orders newOrder = new Orders();
                   
                    newOrder.OrderDate = currentOrderTime;
                    newOrder.ID_Client = (int)ComboClients.SelectedValue;
                    newOrder.ID_Status = 1;

                    if (i < equipmentToOrder.Count)
                    {
                        var eq = equipmentToOrder[i];
                        newOrder.ID_Equipment = eq.ID_Equipment;
                        eq.StockQuantity -= 1;
                    }

                    if (i < selectedServices.Count)
                    {
                        newOrder.ID_Service = selectedServices[i].ID_Service;
                    }

                    newOrder.ID_Employee = selectedEmployees[i % selectedEmployees.Count].ID_Employee;

                    KitchenBaseEntities.GetContext().Orders.Add(newOrder);
                }

                KitchenBaseEntities.GetContext().SaveChanges();

                MessageBox.Show($"Заказ успешно оформлен!\nСоздано позиций: {maxCount} шт.\nТовары списаны со склада.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Системная ошибка при сохранении данных:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}