using ExaminationApp.Core.Data;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace ExaminationApp.WPF;

public partial class OrderDetailsWindow : Window
{
    private readonly int _orderNumber;
    public bool CanEdit { get; }

    public OrderDetailsWindow(int orderNumber, bool canEdit)
    {
        InitializeComponent();
        _orderNumber = orderNumber;
        CanEdit = canEdit;
        DataContext = this;
        if (!CanEdit)
        {
            OrderDatePicker.IsEnabled = false;
            SaveDateButton.Visibility = Visibility.Collapsed;
        }
        LoadOrder();
    }

    private void LoadOrder()
    {
        using var db = new AppDbContext();
        var order = db.OrderHeaders
            .Include(header => header.OrderItems)
                .ThenInclude(item => item.Product)
            .Include(header => header.OrderItems)
                .ThenInclude(item => item.Factory)
            .Include(header => header.OrderItems)
                .ThenInclude(item => item.Size)
            .FirstOrDefault(header => header.OrderNumber == _orderNumber);

        if (order == null)
        {
            MessageBox.Show("Заказ не найден.");
            Close();
            return;
        }

        OrderTitle.Text = $"Заказ №{order.OrderNumber}";
        OrderDatePicker.SelectedDate = order.OrderDate;
        var items = order.OrderItems.Select(item => new OrderLine
        {
            OrderRowId = item.OrderRowId,
            ProductInfo = $"{item.Product.ProductName} — {item.Factory.FactoryName}, размер {item.Size.Size1}",
            Quantity = item.Quantity,
            UnitPrice = item.Product.Price ?? 0,
            TotalPrice = (item.Product.Price ?? 0) * item.Quantity
        }).ToList();
        ItemsList.ItemsSource = items;
        OrderTotal.Text = $"Итого: {items.Sum(item => item.TotalPrice):F2} ₽";
    }

    private void OnSaveDate(object sender, RoutedEventArgs e)
    {
        if (OrderDatePicker.SelectedDate is not DateTime date)
        {
            MessageBox.Show("Выберите дату заказа.");
            return;
        }

        using var db = new AppDbContext();
        var order = db.OrderHeaders.FirstOrDefault(header => header.OrderNumber == _orderNumber);
        if (order == null)
            return;

        order.OrderDate = date;
        db.SaveChanges();
        MessageBox.Show("Дата заказа сохранена.");
    }

    private void OnDeleteItem(object sender, RoutedEventArgs e)
    {
        if (!CanEdit || sender is not Button { Tag: not null } button ||
            !int.TryParse(button.Tag.ToString(), out var orderRowId))
            return;

        using var db = new AppDbContext();
        var item = db.OrderItems.FirstOrDefault(row => row.OrderRowId == orderRowId && row.OrderNumber == _orderNumber);
        if (item == null)
            return;

        var stock = db.StockItems.FirstOrDefault(row => row.ProductId == item.ProductId &&
            row.FactoryId == item.FactoryId && row.SizeId == item.SizeId);
        if (stock != null)
            stock.Quantity += item.Quantity;

        db.OrderItems.Remove(item);
        db.SaveChanges();
        LoadOrder();
    }

    private void OnClose(object sender, RoutedEventArgs e) => Close();

    public sealed class OrderLine
    {
        public int OrderRowId { get; set; }
        public string ProductInfo { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
