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


        Catalog.ItemsSource = db.Products.ToList().Select(product => new {
            Image = string.IsNullOrWhiteSpace(product.Image) ? null : System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", product.Image),
            Title = product.ProductName,
            Description = product.Description,
            Price = $"{product.Price:F2}₽",
            Quantity = db.StockItems.Where(stock => stock.ProductId == product.ProductId).Sum(stock => stock.Quantity),
            Highlighted = product.StockItems.Sum(el => el.Quantity) > 3,
            Category = $"Категория: {product.Category}",
        }).ToList();
    }
}