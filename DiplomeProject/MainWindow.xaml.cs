using KitchenManager;
using KitchenManager.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiplomeProject
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new AuthPage());
        }


        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            if (MainFrame.Content is AuthPage)
            {
                Sidebar.Visibility = Visibility.Collapsed;
            }
            else
            {
                Sidebar.Visibility = Visibility.Visible;

                // ЛОГИКА ОГРАНИЧЕНИЯ ДОСТУПА 
                // Если пользователь вошел И его роль "Менеджер"
                if (App.CurrentUser != null && App.CurrentUser.Roles.RoleName == "Менеджер")
                {

                    BtnEmployees.Visibility = Visibility.Collapsed;
                }
                else
                {

                    BtnEmployees.Visibility = Visibility.Visible;
                }
            }
        }

        private void BtnEquipment_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new EquipmentPage());
        private void BtnServices_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ServicesPage());
        private void BtnOrders_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new OrdersPage());
        private void BtnEmployees_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new EmployeesPage());
        private void BtnClients_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ClientsPage());

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            MainFrame.Navigate(new AuthPage());
        }
    }
}