using ExaminationApp.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace ExaminationApp.WPF
{
    /// <summary>
    /// Логика взаимодействия для AdministratorWindow.xaml
    /// </summary>
    public partial class AdministratorWindow : Window
    {
        public AdministratorWindow()
        {
            InitializeComponent();
            using var db = new AppDbContext();
            var orders = db.OrderHeaders.Include(oh => oh.OrderItems).Include(oh => oh.User)
                .OrderBy(oh => oh.OrderNumber).ToList().Select(oh =>
            {
                return new
                {
                    OrderTitle = $"Заказ №{oh.OrderNumber}",
                    OrderDate = oh.OrderDate,
                    CustomerFio = $"Клиент: {oh.User.LastName} {oh.User.FirstName} {oh.User.Fathername}",
                    TotalPrice = $"{db.OrderItems.Include(oi => oi.Product)
                    .Where(oi => oi.OrderNumber == oh.OrderNumber).Sum(oi => oi.Product.Price * oi.Quantity):F2} р."
                };
            });
            OrderList.ItemsSource = orders;
        }
        public void OnOpenMainWindow(object sender, RoutedEventArgs e)
        {
            MainWindow window = new();
            window.Show();
            Close();
        }
    }
}
