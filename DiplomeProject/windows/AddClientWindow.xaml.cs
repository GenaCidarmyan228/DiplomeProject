using DiplomeProject;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KitchenManager
{
    public partial class AddClientWindow : Window
    {
        private Clients _currentClient = new Clients();

        // 1. Конструктор для НОВОГО клиента
        public AddClientWindow()
        {
            InitializeComponent();
            DataContext = _currentClient;
        }

        // 2. Конструктор для РЕДАКТИРОВАНИЯ
        public AddClientWindow(Clients selectedClient)
        {
            InitializeComponent();
            _currentClient = selectedClient;
            DataContext = _currentClient;
        }
        
        private bool _isFormatting = false;

        private void TxtPhone_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_isFormatting) return; 
            _isFormatting = true;

            TextBox txt = sender as TextBox;

           
            string digits = new string(txt.Text.Where(char.IsDigit).ToArray());

           
            if (digits.StartsWith("8"))
                digits = "7" + digits.Substring(1);

           
            if (digits.Length > 11)
                digits = digits.Substring(0, 11);

          
            string result = "";
            if (digits.Length > 0) result += "+" + digits.Substring(0, 1);
            if (digits.Length > 1) result += " (" + digits.Substring(1, Math.Min(3, digits.Length - 1));
            if (digits.Length > 4) result += ") " + digits.Substring(4, Math.Min(3, digits.Length - 4));
            if (digits.Length > 7) result += "-" + digits.Substring(7, Math.Min(2, digits.Length - 7));
            if (digits.Length > 9) result += "-" + digits.Substring(9, Math.Min(2, digits.Length - 9));

           
            txt.Text = result;

            
            txt.CaretIndex = txt.Text.Length;

           
            if (HintPhone != null)
            {
                HintPhone.Visibility = string.IsNullOrEmpty(txt.Text) ? Visibility.Visible : Visibility.Hidden;
            }

            _isFormatting = false;
        }

        
        private void TxtPhone_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(c => char.IsDigit(c) || c == '+');
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentClient.Surname) ||
                string.IsNullOrWhiteSpace(_currentClient.Name) ||
                string.IsNullOrWhiteSpace(_currentClient.Phone))
            {
                MessageBox.Show("Заполните ФИО и телефон!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            
            if (_currentClient.ID_Client == 0)
            {
                KitchenBaseEntities.GetContext().Clients.Add(_currentClient);
            }

            try
            {
                KitchenBaseEntities.GetContext().SaveChanges();
                MessageBox.Show("Сохранено!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}