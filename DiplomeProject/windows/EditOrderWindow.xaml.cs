using DiplomeProject;
using System;
using System.Linq;
using System.Windows;

namespace KitchenManager.Windows
{
    public partial class EditOrderWindow : Window
    {
        private Orders _currentOrder; 

        public EditOrderWindow(Orders selectedOrder)
        {
            InitializeComponent();
            _currentOrder = selectedOrder;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          
            ComboStatus.ItemsSource = KitchenBaseEntities.GetContext().OrderStatus.ToList();
            ComboClients.ItemsSource = KitchenBaseEntities.GetContext().Clients.ToList();
            ComboEquipment.ItemsSource = KitchenBaseEntities.GetContext().Equipment.ToList();
            ComboServices.ItemsSource = KitchenBaseEntities.GetContext().Services.ToList();
            ComboEmployees.ItemsSource = KitchenBaseEntities.GetContext().Employees.ToList();

            
            ComboStatus.SelectedValue = _currentOrder.ID_Status;
            ComboClients.SelectedValue = _currentOrder.ID_Client;
            ComboEquipment.SelectedValue = _currentOrder.ID_Equipment;
            ComboServices.SelectedValue = _currentOrder.ID_Service;
            ComboEmployees.SelectedValue = _currentOrder.ID_Employee;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            
            if (ComboClients.SelectedItem == null)
            {
                MessageBox.Show("Клиент должен быть выбран!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
               
                _currentOrder.ID_Status = (int)ComboStatus.SelectedValue;
                _currentOrder.ID_Client = (int)ComboClients.SelectedValue;

                
                if (ComboEquipment.SelectedValue != null)
                    _currentOrder.ID_Equipment = (int)ComboEquipment.SelectedValue;

                if (ComboServices.SelectedValue != null)
                    _currentOrder.ID_Service = (int)ComboServices.SelectedValue;

                if (ComboEmployees.SelectedValue != null)
                    _currentOrder.ID_Employee = (int)ComboEmployees.SelectedValue;

                
                KitchenBaseEntities.GetContext().SaveChanges();

                MessageBox.Show("Заказ успешно обновлен!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла системная ошибка:\n" + ex.Message, "Ошибка выполнения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
