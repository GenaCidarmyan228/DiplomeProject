using DiplomeProject;
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
using System.Windows.Shapes;

namespace KitchenManager.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddServiceWindow.xaml
    /// </summary>
    public partial class AddServiceWindow : Window
    {
        public AddServiceWindow()
        {
            InitializeComponent();
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTitle.Text) || string.IsNullOrWhiteSpace(TxtPrice.Text))
            {
                MessageBox.Show("Заполните поля!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Services newService = new Services();
                newService.Title = TxtTitle.Text;
                newService.Price = decimal.Parse(TxtPrice.Text);

                KitchenBaseEntities.GetContext().Services.Add(newService);
                KitchenBaseEntities.GetContext().SaveChanges();

                MessageBox.Show("Услуга добавлена!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Close();
            }
            catch (FormatException)
            {
                MessageBox.Show("Стоимость должна быть числом!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла системная ошибка:\n" + ex.Message, "Ошибка выполнения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
