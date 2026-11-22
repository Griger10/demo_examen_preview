using ExaminationApp.Core.Data;
using ExaminationApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ExaminationApp.Core
{
    // Используется для хранения локальной сессии текущего пользователя и его корзины
    public static class UserSession
    {
        public static User? AuthorizedUser { get; private set; }
        public static bool isAuthorized => AuthorizedUser != null;
        public static List<Product> Cart = [];
        public static List<int> Quantities = [];
        public static List<int> CartSizeIds = [];

        public static void Login(string username)
        {
            using var db = new AppDbContext();
            var user = db.Users.Include(u => u.Role).FirstOrDefault(u => u.Login == username);
            if (user == null)
            {
                throw new Exception("User not found");

            }
            else
            {
                AuthorizedUser = user;
            }
        }

        public static void Logout()
        {
            AuthorizedUser = null;
            Cart = [];
            Quantities = [];
            CartSizeIds = [];
        }
    }
}
