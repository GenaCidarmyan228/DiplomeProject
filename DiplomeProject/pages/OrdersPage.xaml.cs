using DiplomeProject;
using System.ComponentModel;
using System.Windows.Data;
using Word = Microsoft.Office.Interop.Word;
using KitchenManager.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace KitchenManager.Pages
{
    public partial class OrdersPage : Page
    {
        public OrdersPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateOrders();
        }
        private void BtnWord_Click(object sender, RoutedEventArgs e)
        {
           
            var selectedOrder = DGridOrders.SelectedItem as Orders;
            if (selectedOrder == null)
            {
                MessageBox.Show("Сначала выберите заказ в таблице!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ContractTemplate.docx");

            if (!File.Exists(templatePath))
            {
                MessageBox.Show("Файл шаблона ContractTemplate.docx не найден в папке с программой!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                
                var wordApp = new Word.Application();
                wordApp.Visible = false; 

               
                var wordDoc = wordApp.Documents.Open(templatePath);

                
                string clientFullName = selectedOrder.Clients != null ? $"{selectedOrder.Clients.Surname} {selectedOrder.Clients.Name}" : "Не указан";
                string eqTitle = selectedOrder.Equipment != null ? selectedOrder.Equipment.Title : "Без оборудования";
                string srvTitle = selectedOrder.Services != null ? selectedOrder.Services.Title : "Без услуг";

              
                ReplaceWordStub(wordDoc, "{OrderNumber}", selectedOrder.ID_Order.ToString());
                ReplaceWordStub(wordDoc, "{OrderDate}", selectedOrder.OrderDate.ToString("dd.MM.yyyy"));
                ReplaceWordStub(wordDoc, "{ClientName}", clientFullName);
                ReplaceWordStub(wordDoc, "{EquipmentName}", eqTitle);
                ReplaceWordStub(wordDoc, "{ServiceName}", srvTitle);
                ReplaceWordStub(wordDoc, "{TotalSum}", selectedOrder.TotalPrice.ToString("N2"));

                
                wordApp.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при формировании договора: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        
        private void ReplaceWordStub(Word.Document wordDocument, string stubToReplace, string text)
        {
            var range = wordDocument.Content;
            range.Find.ClearFormatting();

            
            range.Find.Execute(
                FindText: stubToReplace,
                ReplaceWith: text,
                Replace: Word.WdReplace.wdReplaceAll
            );
        }
        private void UpdateOrders()
        {
            var allOrders = KitchenBaseEntities.GetContext().Orders.ToList();

          
            ICollectionView view = CollectionViewSource.GetDefaultView(allOrders);

            view.GroupDescriptions.Clear();
           
            view.GroupDescriptions.Add(new PropertyGroupDescription("OrderDate"));

          
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription("OrderDate", ListSortDirection.Descending));

            DGridOrders.ItemsSource = view;

            
            if (allOrders != null)
            {
                TxtTotalOrdersCount.Text = allOrders.Count.ToString();
                decimal totalSum = allOrders.Sum(o => o.TotalPrice);
                TxtTotalRevenue.Text = $"{totalSum:N2} ₽";

                int activeCount = allOrders.Count(o => o.OrderStatus != null &&
                                                       o.OrderStatus.StatusName != "Выполнен" &&
                                                       o.OrderStatus.StatusName != "Отменен");
                TxtActiveOrdersCount.Text = activeCount.ToString();
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = TxtSearch.Text.ToLower().Trim();
            var currentData = KitchenBaseEntities.GetContext().Orders.ToList();

            if (!string.IsNullOrEmpty(searchText))
            {
                currentData = currentData.Where(o =>
                    o.ID_Order.ToString().Contains(searchText) ||
                    o.OrderDate.ToString("dd.MM.yyyy").Contains(searchText) ||
                    (o.Clients != null && o.Clients.Surname.ToLower().Contains(searchText)) ||
                    (o.Employees != null && o.Employees.Surname.ToLower().Contains(searchText)) ||
                    (o.OrderStatus != null && o.OrderStatus.StatusName.ToLower().Contains(searchText)) ||
                    (o.Equipment != null && o.Equipment.Title.ToLower().Contains(searchText)) ||
                    (o.Services != null && o.Services.Title.ToLower().Contains(searchText)) ||
                    o.TotalPrice.ToString().Contains(searchText)
                ).ToList();
            }

            
            ICollectionView view = CollectionViewSource.GetDefaultView(currentData);
            view.GroupDescriptions.Clear();
            view.GroupDescriptions.Add(new PropertyGroupDescription("OrderDate"));

            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription("OrderDate", ListSortDirection.Descending));

            DGridOrders.ItemsSource = view;
        }
        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
          
            var currentOrders = DGridOrders.ItemsSource as List<Orders>;

            if (currentOrders == null || currentOrders.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV файл (Excel)|*.csv";
            saveFileDialog.FileName = $"Отчет_Заказы_{DateTime.Now.ToString("dd_MM_yyyy")}.csv";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
              
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, new UTF8Encoding(true)))
                    {
                       
                        sw.WriteLine("Номер заказа;Дата;Клиент;Сотрудник;Товар;Услуга;Статус;Сумма");

                      
                        foreach (var order in currentOrders)
                        {
                            string date = order.OrderDate.ToString("dd.MM.yyyy");
                            string client = order.Clients != null ? order.Clients.Surname : "Нет данных";
                            string employee = order.Employees != null ? order.Employees.Surname : "Нет данных";
                            string equipment = order.Equipment != null ? order.Equipment.Title : "Нет данных";
                            string service = order.Services != null ? order.Services.Title : "Нет данных";
                            string status = order.OrderStatus != null ? order.OrderStatus.StatusName : "Нет данных";
                            string price = order.TotalPrice.ToString("N2");
                            string row = $"{order.ID_Order};{date};{client};{employee};{equipment};{service};{status};{price}";
                            sw.WriteLine(row);
                        }
                    }

                    MessageBox.Show("Отчет успешно выгружен! Вы можете открыть его в Excel.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении файла: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddOrderWindow addWindow = new AddOrderWindow();
            addWindow.ShowDialog();
            UpdateOrders();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = DGridOrders.SelectedItem as Orders;
            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для редактирования!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EditOrderWindow editWindow = new EditOrderWindow(selectedOrder);
            editWindow.ShowDialog();
            UpdateOrders();
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            
            var selectedOrder = DGridOrders.SelectedItem as Orders;

            if (selectedOrder == null)
            {
                MessageBox.Show("Пожалуйста, выберите заказ из списка для печати!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
               
                DGridOrders.IsEnabled = false;

                try
                {
                    
                    printDialog.PrintVisual(DGridOrders, $"Заказ №{selectedOrder.ID_Order}");
                    MessageBox.Show("Заказ успешно отправлен на печать!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при отправке на печать: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    DGridOrders.IsEnabled = true;
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = DGridOrders.SelectedItem as Orders;
            if (selectedOrder != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить этот заказ?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        KitchenBaseEntities.GetContext().Orders.Remove(selectedOrder);
                        KitchenBaseEntities.GetContext().SaveChanges();
                        UpdateOrders();
                        MessageBox.Show("Заказ успешно удален!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении заказа:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}