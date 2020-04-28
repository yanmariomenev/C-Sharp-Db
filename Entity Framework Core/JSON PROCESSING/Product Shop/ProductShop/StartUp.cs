using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTO;
using ProductShop.Models;
using Remotion.Linq.Clauses;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();
            //context.Database.EnsureCreated();
            //var usersJson = File.ReadAllText
            //    (@"C:\Users\Yanmario\Desktop\Entity Framework Core\08.JSON PROCESSING\Product Shop\ProductShop\Datasets\users.json");
            //var productJson = File.ReadAllText
            //    (@"C:\Users\Yanmario\Desktop\Entity Framework Core\08.JSON PROCESSING\Product Shop\ProductShop\Datasets\products.json");
            //var categoryJson = File.ReadAllText(
            //    @"C:\Users\Yanmario\Desktop\Entity Framework Core\08.JSON PROCESSING\Product Shop\ProductShop\Datasets\categories.json");
            //var categoryAndProductsJson = File.ReadAllText(
            //    @"C:\Users\Yanmario\Desktop\Entity Framework Core\08.JSON PROCESSING\Product Shop\ProductShop\Datasets\categories-products.json");

            var result = GetCategoriesByProductsCount(context);
            Console.WriteLine(result);
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<User[]>(inputJson);
            context.Users.AddRange(users);
            var count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<Product[]>(inputJson);
            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<Category[]>(inputJson)
                .Where(c => c.Name != null);
            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryAndProduct = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);
            context.CategoryProducts.AddRange(categoryAndProduct);
            context.SaveChanges();

            return $"Successfully imported {categoryAndProduct.Length}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var exportProducs = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new ExportProductDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    Seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
                })
                .OrderBy(p => p.Price)
                .ToList();

            var json = JsonConvert.SerializeObject(exportProducs, Formatting.Indented);

            return json;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var soldProduct = context.Users
                .Where(u => u.ProductsSold.Any(x => x.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new UserDto()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProductDto = u.ProductsSold
                        .Where(p => p.Buyer != null)
                        .Select(s => new SoldProductDto()
                        {
                            Name = s.Name,
                            Price = s.Price,
                            BuyerFirstName = s.Buyer.FirstName,
                            BuyerLastName = s.Buyer.LastName
                        }).ToList()
                })
                .ToList();

            var json = JsonConvert.SerializeObject(soldProduct, Formatting.Indented);
            return json;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var category = context.Categories
                .OrderByDescending(c => c.CategoryProducts.Count())
                .Select(c => new CategoryDto()
                {
                    Category = c.Name,
                    ProductsCount = c.CategoryProducts.Count(),
                    AveragePrice = $"{c.CategoryProducts.Sum(p => p.Product.Price) / c.CategoryProducts.Count():F2}",
                    TotalRevenue = $"{c.CategoryProducts.Sum(x => x.Product.Price):F2}"
                })
                .ToList();
            var json = JsonConvert.SerializeObject(category, Formatting.Indented);
            return json;

        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var userAndProduct = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderByDescending(p => p.ProductsSold.Count(ps => ps.Buyer != null))
                .Select(u => new UserAndProductsDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new SoldProductsWithCountDto
                    {
                        Count = u.ProductsSold.Count(p => p.Buyer != null),
                        Products = u.ProductsSold
                            .Where(p => p.Buyer != null)
                            .Select(p => new SoldProductAndPriceDto
                            {
                                Name = p.Name,
                                Price = p.Price
                            })
                            .ToList()
                    }
                })
                .ToList();

            var result = new UserAndProductsResultDto
            {
                UsersCount = userAndProduct.Count(),
                Users = userAndProduct
            };

            var json = JsonConvert.SerializeObject(result,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            return json;
        }
    }
}