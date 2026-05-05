using DiplomeProject;
using System;
using System.Windows;

namespace KitchenManager.Windows
{
    public partial class AddEquipmentWindow : Window
    {
        private Equipment _currentEquipment = new Equipment();

        // Конструктор 1: Новый товар
        public AddEquipmentWindow()
        {
            InitializeComponent();
            DataContext = _currentEquipment;
        }

        // Конструктор 2: Редактирование
        public AddEquipmentWindow(Equipment selectedEquipment)
        {
            InitializeComponent();
            _currentEquipment = selectedEquipment;
            DataContext = _currentEquipment;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentEquipment.Title))
            {
                MessageBox.Show("Введите название товара!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            
            if (_currentEquipment.ID_Equipment == 0)
            {
                KitchenBaseEntities.GetContext().Equipment.Add(_currentEquipment);
            }

            try
            {
                KitchenBaseEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла системная ошибка:\n" + ex.Message, "Ошибка выполнения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}