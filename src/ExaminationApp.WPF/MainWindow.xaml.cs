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
        var missingOrdersForLastMonth = db.OrderHeaders.Select(o => o.OrderDate)
            .Where(date => date >= DateTime.Now.AddMonths(-1))
            .Count();

        Catalog.ItemsSource = products.Select(p => new
        {
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
        }).ToList();
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
        }
        else
            UserFio.Content = $"Пользователь: Гость";

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
}
