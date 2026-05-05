using DiplomeProject;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace KitchenManager.Pages
{
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = TxtLogin.Text;
            string password = TxtPassword.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var userObj = KitchenBaseEntities.GetContext().Users
                    .FirstOrDefault(u => u.Login == login && u.Password == password);

                if (userObj == null)
                {
                    MessageBox.Show("Такого пользователя нет или пароль неверный!", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                   
                    App.CurrentUser = userObj;

                    MessageBox.Show($"Добро пожаловать, {userObj.Roles.RoleName}!", "Успешная авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.Navigate(new OrdersPage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла системная ошибка:\n" + ex.Message, "Ошибка выполнения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}