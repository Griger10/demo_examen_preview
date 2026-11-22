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
using ExaminationApp.Core;

namespace ExaminationApp.WPF
{
    /// <summary>
    /// Логика взаимодействия для ProductOrderWindow.xaml
    /// </summary>
    public partial class ProductOrderWindow : Window
    {
        public int ProductId { get; private set; }

        public ProductOrderWindow(int productId)
        {
            InitializeComponent();
            ProductId = productId;
            using var db = new AppDbContext();
            var p = db.Products.Include(p => p.StockItems).FirstOrDefault(p => p.ProductId == productId);
            var availableSizes = db.StockItems.Select(s => s)
                .Where(si => si.ProductId == p.ProductId && si.Quantity > 0)
                .Select(si => si.Size.Size1)
                .Distinct().OrderBy(s => s)
                .ToList();
            SizesSelect.ItemsSource = availableSizes;
        }

        public void OnAddToCart(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();
            var p = db.Products.FirstOrDefault(p => p.ProductId == ProductId);
            var quantity = Quantity.Text.Trim();
            if (!int.TryParse(quantity, out int parsedQuantity) || parsedQuantity <= 0) {
                MessageBox.Show("Количество должно быть положительным целым числом!");
                return;
            }
            var size = (decimal)(SizesSelect?.SelectedValue ?? 0.0m);
            if (size == 0.0m)
            {
                MessageBox.Show("Пожалуйста, выберите размер!");
                return;
            }
            var availableQuantity = db.StockItems
                .Where(si => si.ProductId == ProductId && si.Size.Size1 == size)
                .Sum(si => (int?)si.Quantity) ?? 0;
            if (parsedQuantity > availableQuantity)
            {
                MessageBox.Show($"Нельзя заказать больше, чем есть в наличии!\n\nДоступно: {availableQuantity}");
                return;
            }
            var sizeId = db.StockItems
                .Where(si => si.ProductId == ProductId && si.Size.Size1 == size)
                .Select(si => si.SizeId)
                .First();
            UserSession.Cart.Add(p);
            UserSession.Quantities.Add(parsedQuantity);
            UserSession.CartSizeIds.Add(sizeId);
            MessageBox.Show("Товар успешно добавлен в корзину!");
            this.Close();
        }

        public void OnReturnToDescription(object sender, RoutedEventArgs e)
        {
            ProductDescriptionWindow window = new(ProductId);
            window.Show();
            this.Close();
        }
    }
}
