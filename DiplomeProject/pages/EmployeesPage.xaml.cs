using DiplomeProject;
using KitchenManager.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KitchenManager
{
    public partial class EmployeesPage : Page
    {
        public EmployeesPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateEmployees();
        }

        private void UpdateEmployees()
        {
            DGridEmployees.ItemsSource = KitchenBaseEntities.GetContext().Employees.ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = TxtSearch.Text.ToLower();
            var currentData = KitchenBaseEntities.GetContext().Employees.ToList();

            currentData = currentData.Where(emp =>
                emp.Surname.ToLower().Contains(searchText) ||
                emp.Name.ToLower().Contains(searchText) ||
                (emp.Positions != null && emp.Positions.PositionName.ToLower().Contains(searchText))).ToList();

            DGridEmployees.ItemsSource = currentData;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddEmployeeWindow win = new AddEmployeeWindow();
            win.ShowDialog();
            UpdateEmployees();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmp = DGridEmployees.SelectedItem as Employees;
            if (selectedEmp == null)
            {
                MessageBox.Show("Выберите сотрудника для редактирования!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AddEmployeeWindow win = new AddEmployeeWindow(selectedEmp);
            win.ShowDialog();
            UpdateEmployees();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmp = DGridEmployees.SelectedItem as Employees;
            if (selectedEmp != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить сотрудника?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        KitchenBaseEntities.GetContext().Employees.Remove(selectedEmp);
                        KitchenBaseEntities.GetContext().SaveChanges();
                        UpdateEmployees();
                        MessageBox.Show("Сотрудник удален!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка удаления (возможно сотрудник привязан к заказам):\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnAddPosition_Click(object sender, RoutedEventArgs e)
        {
            AddPositionWindow win = new AddPositionWindow();
            win.ShowDialog();
        }
    }
}