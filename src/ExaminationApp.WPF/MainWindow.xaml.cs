using ExaminationApp.Core;
using ExaminationApp.Core.Data;
using ExaminationApp.Core.Models;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
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
    public void ApplyFilters()
    {
        using var db = new AppDbContext();
        var products = db.Products
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .Include(p => p.StockItems)
            .Include(p => p.Factory)
            .ToList();

        if (CategoriesSelect.SelectedItem != null)
        {
            var category = CategoriesSelect.SelectedValue.ToString();
            products = products.Where(p => p.Category.CategoryName == category || p.Subcategory.CategoryName == category).ToList();
        }
        if (SortingOptions.SelectedItem != null)
        {
            if (SortingOptions.SelectedValue == "По возрастанию цены")
                products = products.OrderBy(p => p.Price).ToList();
            else if (SortingOptions.SelectedValue == "По убыванию цены")
                products = products.OrderBy(p => -p.Price).ToList();
        }
        if (!string.IsNullOrWhiteSpace(SearchTextBox.Text.Trim()))
        {
            var searchText = SearchTextBox.Text.Trim().ToLower();
            products = products.Where(p => p.ProductName.ToLower().Contains(searchText) || p.Description.ToLower().Contains(searchText)).ToList();
        }
        var totalQuantity = products.SelectMany(p => p.StockItems).Sum(stock => stock.Quantity);
        var stockStatus = totalQuantity <= 5 ? "мало" : "много";

        Catalog.ItemsSource = products.Select(p =>
        {
            var missingOrdersForLastMonth = db.OrderHeaders.Include(o => o.OrderItems).Select(o => o)
                   .Where(o => o.OrderDate >= DateTime.Now.AddMonths(-1))
                   .Where(o => !o.OrderItems.Any(item => item.ProductId == p.ProductId))
                   .Count();
            return new
            {
                ProductId = p.ProductId,
                Image = string.IsNullOrWhiteSpace(p.Image)
                        ? null
                        : System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", p.Image),
                Title = p.ProductName,
                Details = p.Details,
                Price = missingOrdersForLastMonth > 0 ? $"{p.Price:F2}₽ (без скидки)" : $"{p.Price * 0.75m:F2}₽ (со скидкой 25%)",
                Quantity = $"Количество: {totalQuantity} шт. ({stockStatus})",
                Highlighted = totalQuantity <= 3,
                Category = $"Категория: {p.Category?.CategoryName ?? "Неизвестная категория"}",
                FactoryName = p.Factory?.FactoryName ?? "Неизвестный производитель",
            };
        });
    }
    public MainWindow()
    {
        InitializeComponent();
        using var db = new AppDbContext();

        var user = UserSession.AuthorizedUser;
        if (user != null)
        {
            UserFio.Content = $"Пользователь: {user?.LastName} {user?.FirstName} {user?.Fathername}";
            List<String> sortingVariants = ["По возрастанию цены", "По убыванию цены"];
            var categoriesNames = db.Categories.Select(c => c.CategoryName).ToList();
            CategoriesSelect.ItemsSource = categoriesNames;
            SortingOptions.ItemsSource = sortingVariants;
            SortingOptions.Visibility = Visibility.Visible;
            SortingLabel.Visibility = Visibility.Visible;
            CategoriesLabel.Visibility = Visibility.Visible;
            CategoriesSelect.Visibility = Visibility.Visible;
            SearchLabel.Visibility = Visibility.Visible;
            SearchTextBox.Visibility = Visibility.Visible;
            OrderInfoButton.Visibility = Visibility.Visible;
            CreateOrderButton.Visibility = Visibility.Visible;
            DropOrderButton.Visibility = Visibility.Visible;
            if (user.Role.Role1 == "Менеджер" || user.Role.Role1 == "Администратор")
            {
                AdminButton.Visibility = Visibility.Visible;
            }
        }
        else
            UserFio.Content = $"Пользователь: Гость";

        var products = db.Products.Include(c => c.Category).Include(s => s.StockItems).Include(f => f.Factory)
            .ToList().Select(product =>
            {
                var totalQuantity = product.StockItems.Sum(stock => stock.Quantity);
                var stockStatus = totalQuantity <= 5 ? "мало" : "много";
                var missingOrdersForLastMonth = db.OrderHeaders.Include(o => o.OrderItems).Select(o => o)
                    .Where(o => o.OrderDate >= DateTime.Now.AddMonths(-1))
                    .Where(o => !o.OrderItems.Any(item => item.ProductId == product.ProductId))
                    .Count();

                return new
                {
                    ProductId = product.ProductId,
                    Image = string.IsNullOrWhiteSpace(product.Image)
                        ? null
                        : System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", product.Image),
                    Title = product.ProductName,
                    Details = product.Details,
                    Price = missingOrdersForLastMonth > 0? $"{product.Price:F2}₽ (без скидки)": $"{product.Price * 0.75m:F2}₽ (со скидкой 25%)",
                    Quantity = $"Количество: {totalQuantity} шт. ({stockStatus})",
                    Highlighted = totalQuantity <= 3,
                    Category = $"Категория: {product.Category?.CategoryName ?? "Неизвестная категория"}",
                    FactoryName = product.Factory?.FactoryName ?? "Неизвестный производитель",
                };
            }).ToList();

        Catalog.ItemsSource = products;
    }

    private void OnLogout(object sender, RoutedEventArgs e)
    {
        LoginWindow window = new();
        window.Show();
        this.Close();
        UserSession.Logout();
    }

    private void OnChangeSort(object sender, SelectionChangedEventArgs e)
    {
        ApplyFilters();
    }

    private void OnChangeCategory(object sender, SelectionChangedEventArgs e)
    {
        ApplyFilters();
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyFilters();
    }

    private void OnProductDescriptionOpen(object sender, MouseButtonEventArgs e)
    {
        if (UserSession.AuthorizedUser == null)
            return;
        if (Catalog.SelectedItem is not { } selectedProduct)
            return;

        dynamic product = selectedProduct;
        ProductDescriptionWindow window = new(product.ProductId);
        window.Show();
    }

    private void OnOrderInfo(object sender, RoutedEventArgs e)
    {
        if (UserSession.AuthorizedUser == null)
            return;
        if (UserSession.Cart.Count == 0)
        {
            MessageBox.Show("Корзина пуста!");
            return;
        }
        List<String> orderLines = [];
        for (int i = 0; i < UserSession.Cart.Count; i++)
        {
            var product = UserSession.Cart[i];
            var quantity = UserSession.Quantities[i];
            orderLines.Add($"\nТовар: {product.ProductName}, Количество: {quantity} шт.,  Цена: {product.Price * quantity:F2}₽");
        }
        MessageBox.Show("Состав заказа", String.Join("", orderLines) + $"\n\nОбщая стоимость: {UserSession.Cart.Zip(UserSession.Quantities, (p, q) => p.Price * q).Sum():F2}₽");
    }

    private void OnDropOrder(object sender, RoutedEventArgs e)
    {
        if (UserSession.AuthorizedUser == null)
            return;
        UserSession.Cart.Clear();
        UserSession.Quantities.Clear();
        UserSession.CartSizeIds.Clear();
        MessageBox.Show("Заказ успешно отменен!");
    }

    private void OnCreateOrder(object sender, RoutedEventArgs e)
    {
        if (UserSession.AuthorizedUser == null)
            return;

        if (UserSession.Cart.Count == 0)
        {
            MessageBox.Show("Корзина пуста.");
            return;
        }

        using var db = new AppDbContext();
        var orderNumber = db.OrderHeaders.Any() ? db.OrderHeaders.Max(o => o.OrderNumber) + 1 : 1;
        var orderHeader = new OrderHeader
        {
            OrderNumber = orderNumber,
            UserId = UserSession.AuthorizedUser.UserId,
            OrderDate = DateTime.Now,
        };

        var nextOrderRowId = db.OrderItems.Any() ? db.OrderItems.Max(o => o.OrderRowId) + 1 : 1;
        var orderItems = new List<OrderItem>();
        for (var i = 0; i < UserSession.Cart.Count; i++)
        {
            var product = UserSession.Cart[i];
            var remaining = UserSession.Quantities[i];
            var sizeId = UserSession.CartSizeIds[i];
            var stockItems = db.StockItems
                .Where(si => si.ProductId == product.ProductId && si.SizeId == sizeId && si.Quantity > 0)
                .OrderBy(si => si.StockId)
                .ToList();

            if (stockItems.Sum(si => si.Quantity) < remaining)
            {
                MessageBox.Show($"Недостаточно товара «{product.ProductName}» выбранного размера. Обновите корзину.");
                return;
            }

            foreach (var stock in stockItems)
            {
                var quantityFromStock = Math.Min(remaining, stock.Quantity);
                if (quantityFromStock <= 0)
                    continue;

                orderItems.Add(new OrderItem
                {
                    OrderRowId = nextOrderRowId++,
                    ProductId = product.ProductId,
                    Quantity = quantityFromStock,
                    OrderNumber = orderNumber,
                    CategoryId = product.CategoryId,
                    SizeId = sizeId,
                    FactoryId = stock.FactoryId,
                });
                stock.Quantity -= quantityFromStock;
                remaining -= quantityFromStock;
                if (remaining == 0)
                    break;
            }
        }

        orderHeader.OrderItems = orderItems;
        db.OrderHeaders.Add(orderHeader);
        db.SaveChanges();

        UserSession.Cart.Clear();
        UserSession.Quantities.Clear();
        UserSession.CartSizeIds.Clear();
        MessageBox.Show("Заказ успешно сохранён!");
    }

    public void OnOpenManagementPanel(object sender, RoutedEventArgs e)
    {
        AdministratorWindow window = new();
        window.Show();
        Close();
    }
}
