using ExaminationApp.Core;
using ExaminationApp.Core.Data;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExaminationApp.WPF;

public partial class AdministratorWindow : Window
{
    public AdministratorWindow()
    {
        InitializeComponent();
        LoadOrders();
    }

    private void LoadOrders()
    {
        using var db = new AppDbContext();
        OrderList.ItemsSource = db.OrderHeaders
            .Include(order => order.User)
            .Include(order => order.OrderItems)
                .ThenInclude(item => item.Product)
            .OrderByDescending(order => order.OrderDate)
            .Select(order => new OrderSummary
            {
                OrderNumber = order.OrderNumber,
                OrderTitle = $"Заказ №{order.OrderNumber}",
                OrderDate = order.OrderDate,
                CustomerFio = $"Клиент: {order.User.LastName} {order.User.FirstName} {order.User.Fathername}",
                TotalPrice = order.OrderItems.Sum(item => item.Product.Price * item.Quantity) ?? 0
            })
            .ToList();
    }

    private void OnOrderDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (OrderList.SelectedItem is not OrderSummary order)
            return;

        var isAdmin = UserSession.AuthorizedUser?.Role.Role1 == "Администратор";
        var window = new OrderDetailsWindow(order.OrderNumber, isAdmin);
        window.Owner = this;
        window.Closed += (_, _) => LoadOrders();
        window.ShowDialog();
    }

    private void OnOpenMainWindow(object sender, RoutedEventArgs e)
    {
        new MainWindow().Show();
        Close();
    }

    public sealed class OrderSummary
    {
        public int OrderNumber { get; set; }
        public string OrderTitle { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public string CustomerFio { get; set; } = "";
        public decimal TotalPrice { get; set; }
    }
}
