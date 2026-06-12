using DiplomeProject;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KitchenManager.Windows
{
    public partial class AddOrderWindow : Window
    {
        public AddOrderWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ComboClients.ItemsSource = KitchenBaseEntities.GetContext().Clients.ToList();
            ComboEquipment.ItemsSource = KitchenBaseEntities.GetContext().Equipment.ToList();
            ComboServices.ItemsSource = KitchenBaseEntities.GetContext().Services.ToList();
            ComboEmployees.ItemsSource = KitchenBaseEntities.GetContext().Employees.ToList();
        }

     
        private void ComboEquipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboEquipment.SelectedValue != null)
            {
                int equipmentId = (int)ComboEquipment.SelectedValue;
                var selectedEquipment = KitchenBaseEntities.GetContext().Equipment.FirstOrDefault(eq => eq.ID_Equipment == equipmentId);

                if (selectedEquipment != null)
                {
                    
                    int stock = selectedEquipment.StockQuantity;

                    TxtStockInfo.Text = $"В наличии на складе: {stock} шт.";

                    PbStock.Value = stock > 20 ? 20 : stock;

                    if (stock >= 10)
                    {
                        PbStock.Foreground = (Brush)new BrushConverter().ConvertFrom("#28A745");
                        TxtStockInfo.Foreground = Brushes.Gray;
                    }
                    else if (stock >= 3)
                    {
                        PbStock.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFC107");
                        TxtStockInfo.Foreground = Brushes.Gray;
                    }
                    else
                    {
                        PbStock.Foreground = (Brush)new BrushConverter().ConvertFrom("#DC3545");
                        TxtStockInfo.Foreground = Brushes.Red;

                        if (stock == 0)
                            TxtStockInfo.Text += " (Нет в наличии!)";
                        else
                            TxtStockInfo.Text += " (Критический остаток!)";
                    }
                }
            }
            else
            {
                TxtStockInfo.Text = "Остаток на складе: выберите товар";
                PbStock.Value = 0;
                TxtStockInfo.Foreground = Brushes.Gray;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ComboClients.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите клиента!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ComboEmployees.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника-менеджера!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Orders newOrder = new Orders();
                newOrder.OrderDate = DateTime.Now;
                newOrder.ID_Client = (int)ComboClients.SelectedValue;
                newOrder.ID_Employee = (int)ComboEmployees.SelectedValue;
                newOrder.ID_Status = 1;

                if (ComboEquipment.SelectedValue != null)
                {
                    int equipmentId = (int)ComboEquipment.SelectedValue;
                    var equipmentToOrder = KitchenBaseEntities.GetContext().Equipment.FirstOrDefault(eq => eq.ID_Equipment == equipmentId);

                    if (equipmentToOrder != null)
                    {
                       
                        if (equipmentToOrder.StockQuantity <= 0)
                        {
                            MessageBox.Show($"Оборудование «{equipmentToOrder.Title}» закончилось на складе!\nСоздание заказа невозможно.",
                                            "Нет в наличии", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        equipmentToOrder.StockQuantity -= 1;
                        newOrder.ID_Equipment = equipmentId;
                    }
                }

                if (ComboServices.SelectedValue != null)
                    newOrder.ID_Service = (int)ComboServices.SelectedValue;

                KitchenBaseEntities.GetContext().Orders.Add(newOrder);
                KitchenBaseEntities.GetContext().SaveChanges();

                MessageBox.Show("Новый заказ успешно оформлен!\nОборудование списано со склада.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Системная ошибка при оформлении заказа:\n\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}