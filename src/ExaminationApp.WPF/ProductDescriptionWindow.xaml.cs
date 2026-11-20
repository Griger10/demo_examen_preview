using ExaminationApp.Core.Data;
using Microsoft.EntityFrameworkCore;
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

namespace ExaminationApp.WPF
{
    /// <summary>
    /// Логика взаимодействия для ProductDescription.xaml
    /// </summary>
    public partial class ProductDescriptionWindow : Window
    {
        public ProductDescriptionWindow(int ProductId)
        {
            InitializeComponent();

            using var db = new AppDbContext();
            var p = db.Products.Include(p => p.Category).Include(p => p.StockItems).FirstOrDefault(p => p.ProductId == ProductId);
            var missingOrdersForLastMonth = db.OrderHeaders.Include(o => o.OrderItems).Select(o => o)
                   .Where(o => o.OrderDate >= DateTime.Now.AddMonths(-1))
                   .Where(o => !o.OrderItems.Any(item => item.ProductId == p.ProductId))
                   .Count();
            var availableSizes = db.StockItems.Select(s => s)
                .Where(si => si.ProductId == p.ProductId && si.Quantity > 0)
                .Select(si => si.Size.Size1)
                .Distinct().OrderBy(s => s)
                .ToList();
            ProductImage.Source = new BitmapImage(
                new Uri(
                    string.IsNullOrWhiteSpace(p.Image)? null: System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", p?.Image ?? "default.png")
                    )
                );
            ProductTitle.Text = p?.ProductName ?? "Неизвестный продукт";
            ProductDescription.Text = p?.Description ?? "Отсутствует описание";
            ProductDetails.Text = p?.Details ?? "Отсутствует состав";
            ProductPrice.Text = missingOrdersForLastMonth > 0 ? $"{p.Price:F2}₽ (без скидки)" : $"{p.Price * 0.75m:F2}₽ (со скидкой 25%)";
            ProductCategory.Text = $"Категория: {p?.Category.CategoryName}";
            AvailableSizes.Text = $"Доступные размеры: {String.Join("; ", availableSizes)}";
        }

        private void OnOpenMainWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
