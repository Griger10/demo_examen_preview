using ExaminationApp.Core.Data;
using ExaminationApp.Core.Models;
using Humanizer;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;

namespace ExaminationApp.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        using var db = new AppDbContext();

        var products = db.Products.Include(c => c.Category).Include(s => s.StockItems).Include(f => f.Factory)
            .ToList().Select(product =>
            {
                var totalQuantity = product.StockItems.Sum(stock => stock.Quantity);
                var stockStatus = totalQuantity <= 5 ? "мало" : "много";
                var missingOrdersForLastMonth = db.OrderHeaders.Select(o => o.OrderDate)
                    .Where(date => date >= DateTime.Now.AddMonths(-1))
                    .Count();

                return new
                {
                    Image = string.IsNullOrWhiteSpace(product.Image)
                        ? null
                        : System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", product.Image),
                    Title = product.ProductName,
                    Details = product.Details,
                    Price = missingOrdersForLastMonth > 0? $"{product.Price:F2}₽": $"{product.Price * 0.75m:F2}₽ (со скидкой 25%)",
                    Quantity = $"Количество: {totalQuantity} шт. ({stockStatus})",
                    Highlighted = totalQuantity <= 3,
                    Category = $"Категория: {product.Category?.CategoryName ?? "Неизвестная категория"}",
                    FactoryName = product.Factory?.FactoryName ?? "Неизвестный производитель",
                };
            }).ToList();

        Catalog.ItemsSource = products;
    }
}
