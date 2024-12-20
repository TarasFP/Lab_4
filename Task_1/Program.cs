using System;
using System.Collections.Generic;
using System.Linq;

namespace Task_1
{
    interface ISearchable
    {
        List<Product> SearchByCategory(string category);
        List<Product> SearchByPrice(double minPrice, double maxPrice);
        List<Product> SearchByRating(double minRating);
    }

    class Product
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public double Rating { get; set; }

        public Product(string name, double price, string description, string category, double rating)
        {
            Name = name;
            Price = price;
            Description = description;
            Category = category;
            Rating = rating;
        }

        public override string ToString()
        {
            return $"Назва: {Name}, Ціна: {Price}, Категорія: {Category}, Рейтинг: {Rating:F1}, Опис: {Description}";
        }
    }

    class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public List<Order> OrderHistory { get; private set; }

        public User(string login, string password)
        {
            Login = login;
            Password = password;
            OrderHistory = new List<Order>();
        }

        public void AddOrder(Order order)
        {
            OrderHistory.Add(order);
        }

        public override string ToString()
        {
            return $"Користувач: {Login}, Кількість замовлень: {OrderHistory.Count}";
        }
    }

    class Order
    {
        public List<(Product Product, int Quantity)> Items { get; private set; }
        public double TotalPrice { get; private set; }
        public string Status { get; set; }

        public Order()
        {
            Items = new List<(Product, int)>();
            Status = "Очікується";
        }

        public void AddItem(Product product, int quantity)
        {
            Items.Add((product, quantity));
            TotalPrice += product.Price * quantity;
        }

        public override string ToString()
        {
            var details = string.Join("\n", Items.Select(item => $"- {item.Product.Name} (x{item.Quantity})"));
            return $"Замовлення:\n{details}\nЗагальна вартість: {TotalPrice:F2}, Статус: {Status}";
        }
    }

    class Store : ISearchable
    {
        private List<Product> products;
        private List<User> users;
        private List<Order> orders;

        public Store()
        {
            products = new List<Product>();
            users = new List<User>();
            orders = new List<Order>();
        }

        public void AddProduct(Product product)
        {
            products.Add(product);
        }

        public void RegisterUser(User user)
        {
            users.Add(user);
        }

        public User AuthenticateUser(string login, string password)
        {
            return users.FirstOrDefault(u => u.Login == login && u.Password == password);
        }

        public void PlaceOrder(User user, Order order)
        {
            user.AddOrder(order);
            orders.Add(order);
        }

        public List<Product> SearchByCategory(string category)
        {
            return products.Where(p => p.Category.ToLower() == category.ToLower()).ToList();
        }

        public List<Product> SearchByPrice(double minPrice, double maxPrice)
        {
            return products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
        }

        public List<Product> SearchByRating(double minRating)
        {
            return products.Where(p => p.Rating >= minRating).ToList();
        }

        public List<Product> GetAllProducts()
        {
            return products;
        }
    }

    class Program
    {
        static void Main()
        {
            Store store = new Store();

            store.AddProduct(new Product("Ноутбук", 1500, "Високопродуктивний ноутбук", "Електроніка", 4.8));
            store.AddProduct(new Product("Смартфон", 800, "Сучасний смартфон", "Електроніка", 4.6));
            store.AddProduct(new Product("Книга", 20, "Бестселер року", "Книги", 4.9));

            User user = new User("user1", "password1");
            store.RegisterUser(user);

            Console.WriteLine("Вітаємо у системі магазину!");
            User authenticatedUser = AuthenticateUser(store);

            if (authenticatedUser == null)
            {
                Console.WriteLine("Помилка входу. Завершення програми.");
                return;
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Ласкаво просимо, {authenticatedUser.Login}!");
                Console.WriteLine("1. Переглянути товари");
                Console.WriteLine("2. Пошук товарів за категорією");
                Console.WriteLine("3. Пошук товарів за ціною");
                Console.WriteLine("4. Пошук товарів за рейтингом");
                Console.WriteLine("5. Замовити товари");
                Console.WriteLine("6. Переглянути історію замовлень");
                Console.WriteLine("7. Вихід");
                Console.Write("Оберіть пункт меню: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DisplayProducts(store);
                        break;
                    case "2":
                        SearchByCategory(store);
                        break;
                    case "3":
                        SearchByPrice(store);
                        break;
                    case "4":
                        SearchByRating(store);
                        break;
                    case "5":
                        PlaceOrder(store, authenticatedUser);
                        break;
                    case "6":
                        ViewOrderHistory(authenticatedUser);
                        break;
                    case "7":
                        Console.WriteLine("Дякуємо за використання системи магазину!");
                        return;
                    default:
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                        break;
                }
                Console.WriteLine("\nНатисніть будь-яку клавішу для повернення до меню...");
                Console.ReadKey();
            }
        }

        static User AuthenticateUser(Store store)
        {
            Console.Write("Введіть логін: ");
            string login = Console.ReadLine();
            Console.Write("Введіть пароль: ");
            string password = Console.ReadLine();
            return store.AuthenticateUser(login, password);
        }

        static void DisplayProducts(Store store)
        {
            Console.Clear();
            Console.WriteLine("Список товарів:");
            foreach (var product in store.GetAllProducts())
            {
                Console.WriteLine(product);
            }
        }

        static void SearchByCategory(Store store)
        {
            Console.Clear();
            Console.Write("Введіть категорію для пошуку: ");
            string category = Console.ReadLine();
            var results = store.SearchByCategory(category);

            if (results.Count > 0)
            {
                Console.WriteLine($"Товари у категорії '{category}':");
                foreach (var product in results)
                {
                    Console.WriteLine(product);
                }
            }
            else
            {
                Console.WriteLine($"У категорії '{category}' товарів не знайдено.");
            }
        }

        static void SearchByPrice(Store store)
        {
            Console.Clear();
            Console.Write("Введіть мінімальну ціну: ");
            double minPrice = double.Parse(Console.ReadLine());
            Console.Write("Введіть максимальну ціну: ");
            double maxPrice = double.Parse(Console.ReadLine());
            var results = store.SearchByPrice(minPrice, maxPrice);

            if (results.Count > 0)
            {
                Console.WriteLine($"Товари у ціновому діапазоні {minPrice}-{maxPrice}:");
                foreach (var product in results)
                {
                    Console.WriteLine(product);
                }
            }
            else
            {
                Console.WriteLine("Товарів у зазначеному діапазоні цін не знайдено.");
            }
        }

        static void SearchByRating(Store store)
        {
            Console.Clear();
            Console.Write("Введіть мінімальний рейтинг: ");
            double minRating = double.Parse(Console.ReadLine());
            var results = store.SearchByRating(minRating);

            if (results.Count > 0)
            {
                Console.WriteLine($"Товари з рейтингом {minRating} і вище:");
                foreach (var product in results)
                {
                    Console.WriteLine(product);
                }
            }
            else
            {
                Console.WriteLine("Товарів із зазначеним рейтингом не знайдено.");
            }
        }

        static void PlaceOrder(Store store, User user)
        {
            Console.Clear();
            Console.WriteLine("Оберіть товари для замовлення:");
            foreach (var product in store.GetAllProducts())
            {
                Console.WriteLine(product);
            }

            Order order = new Order();

            while (true)
            {
                Console.Write("\nВведіть назву товару (або 'завершити' для завершення): ");
                string productName = Console.ReadLine();
                if (productName.ToLower() == "завершити")
                    break;

                var product = store.GetAllProducts().FirstOrDefault(p => p.Name.ToLower() == productName.ToLower());
                if (product == null)
                {
                    Console.WriteLine("Товар не знайдено. Спробуйте ще раз.");
                    continue;
                }

                Console.Write($"Введіть кількість для '{product.Name}': ");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    order.AddItem(product, quantity);
                    Console.WriteLine($"Товар '{product.Name}' додано до замовлення.");
                }
                else
                {
                    Console.WriteLine("Невірна кількість. Спробуйте ще раз.");
                }
            }

            if (order.Items.Count > 0)
            {
                store.PlaceOrder(user, order);
                Console.WriteLine("Замовлення успішно оформлене!");
                Console.WriteLine(order);
            }
            else
            {
                Console.WriteLine("Замовлення не оформлене.");
            }
        }

        static void ViewOrderHistory(User user)
        {
            Console.Clear();
            Console.WriteLine("Історія замовлень:");
            if (user.OrderHistory.Count > 0)
            {
                foreach (var order in user.OrderHistory)
                {
                    Console.WriteLine(order);
                }
            }
            else
            {
                Console.WriteLine("У вас немає замовлень.");
            }
        }
    }

}
