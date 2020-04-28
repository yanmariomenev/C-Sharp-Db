using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            //using (ProductShopContext context = new ProductShopContext())
            //{
            //    context.Database.EnsureCreated();
            //};
            var context = new ProductShopContext();
            //var xmlUsersDir =
            //    File.ReadAllText
            //    (@"C:\Users\Yanmario\Desktop\Entity Framework Core\09.XML Processing\ProductShop\ProductShop\Datasets\users.xml");
            //var xmlProductsDir = File.ReadAllText
            //    (@"C:\Users\Yanmario\Desktop\Entity Framework Core\09.XML Processing\ProductShop\ProductShop\Datasets\products.xml");
            //var xmlCategoriesProducts = File.ReadAllText
            //    (@"C:\Users\Yanmario\Desktop\Entity Framework Core\09.XML Processing\ProductShop\ProductShop\Datasets\categories-products.xml");
            //var xmlCategories = File.ReadAllText
            //    (@"C:\Users\Yanmario\Desktop\Entity Framework Core\09.XML Processing\ProductShop\ProductShop\Datasets\categories.xml");

            var result = GetUsersWithProducts(context);
            Console.WriteLine(result);
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportUserDto[]), new XmlRootAttribute("Users"));

            var usersDto =(ImportUserDto[]) xmlSerializer.Deserialize(new StringReader(inputXml));
            var users = new List<User>();
            foreach (var userDto in usersDto)
            {
                var user = new User()
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Age = userDto.Age
                };
                users.Add(user);
            }
            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportProductDto[]), new XmlRootAttribute("Products"));

            var productDto = (ImportProductDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var products = new List<Product>();
            foreach (var dto in productDto)
            {
                var product = new Product()
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    SellerId = dto.SellerId,
                    BuyerId = dto.BuyerId
                };
                products.Add(product);
            }
            context.Products.AddRange(products);
            context.SaveChanges();
            return $"Successfully imported {products.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCategoryDto[]), new XmlRootAttribute("Categories"));

            var categoryDtos = (ImportCategoryDto[])xmlSerializer.Deserialize(new StringReader(inputXml));
            var categories = new List<Category>();
            foreach (var dto in categoryDtos)
            {
                if (!string.IsNullOrEmpty(dto.Name))
                {
                    var category = new Category()
                    {
                        Name = dto.Name
                    };
                    categories.Add(category);
                }
                
            }
            context.Categories.AddRange(categories);
            context.SaveChanges();
            return $"Successfully imported {categories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCategoryProductDto[]), new XmlRootAttribute("CategoryProducts"));

            var categoryAndProductDtos = (ImportCategoryProductDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var categoriesAndProducs = new List<CategoryProduct>();
            foreach (var dto in categoryAndProductDtos)
            {
                var findCategory = context.Categories.Find(dto.CategoryId);
                var findProduct = context.Products.Find(dto.ProductId);

                if (findCategory != null && findProduct != null)
                {
                    var cateogryAndProduct = new CategoryProduct()
                    {
                        CategoryId = dto.CategoryId,
                        ProductId = dto.ProductId
                    };
                    categoriesAndProducs.Add(cateogryAndProduct);
                }
            }
            context.CategoryProducts.AddRange(categoriesAndProducs);
            context.SaveChanges();
            return $"Successfully imported {categoriesAndProducs.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new ProductInRangeDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = $"{p.Buyer.FirstName} {p.Buyer.LastName}"
                })
                .Take(10)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ProductInRangeDto[]), new XmlRootAttribute("Products"));

            var sb = new StringBuilder();
            var nameSpaces = new XmlSerializerNamespaces(new [] {XmlQualifiedName.Empty}); 
            //new XmlSerializerNamespaces(new []{ new XmlQualifiedName("","")})
            xmlSerializer.Serialize(new StringWriter(sb), products, nameSpaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any(/*n => n.Buyer != null*/))
                .Select(u => new SoldProductsDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold.Select(p => new ProductDto
                    {
                        Name = p.Name,
                        Price = p.Price
                    }).ToArray()
                })
                .OrderBy(n => n.LastName)
                .ThenBy(n => n.FirstName)
                .Take(5)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(SoldProductsDto[]), new XmlRootAttribute("Users"));
            var sb = new StringBuilder();

            var nameSpaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            xmlSerializer.Serialize(new StringWriter(sb), users, nameSpaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new CategoryByProductCountDto
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count(),
                    AveragePrice = c.CategoryProducts.Average(x => x.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(x => x.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();
            var xmlSerializer = new XmlSerializer(typeof(CategoryByProductCountDto[]), new XmlRootAttribute("Categories"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), categories, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any())
                .Select(x => new ExportUserAndProductDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age,
                    UserProductSoldDto = new userProductSoldDto
                    {
                        Count = x.ProductsSold.Count(),
                        ProductDto = x.ProductsSold
                            .Select(p => new ProductDto
                            {
                                Name = p.Name,
                                Price = p.Price
                            })
                            .OrderByDescending(p => p.Price)
                            .ToArray()
                    }
                })
                .OrderByDescending(x => x.UserProductSoldDto.Count)
                .Take(10)
                .ToArray();

            var customExport = new ExportCustomUserDto
            {
                Count = context.Users
                    .Count(x => x.ProductsSold.Any()),
                ExportUserAndProductDto = users
            };

            var xmlSerializer = new XmlSerializer(typeof(ExportCustomUserDto), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), customExport, namespaces);

            return sb.ToString().TrimEnd();
        }
    }

}