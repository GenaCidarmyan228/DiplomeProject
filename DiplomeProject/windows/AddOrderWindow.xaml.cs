using DiplomeProject;
using System;
using System.Linq;
using System.Windows;

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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Проверка обязательных полей
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

                
                // СКЛАДСКАЯ ЛОГИКА 
                
                if (ComboEquipment.SelectedValue != null)
                {
                    int equipmentId = (int)ComboEquipment.SelectedValue;

                   
                    var equipmentToOrder = KitchenBaseEntities.GetContext().Equipment.FirstOrDefault(eq => eq.ID_Equipment == equipmentId);

                    if (equipmentToOrder != null)
                    {
                      
                        if (equipmentToOrder.StockQuantity == null || equipmentToOrder.StockQuantity <= 0)
                        {
                            MessageBox.Show($"Оборудование «{equipmentToOrder.Title}» закончилось на складе!\nСоздание заказа невозможно.",
                                            "Нет в наличии", MessageBoxButton.OK, MessageBoxImage.Error);
                            return; 
                        }

                        // 2. СПИСАНИЕ
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
    
