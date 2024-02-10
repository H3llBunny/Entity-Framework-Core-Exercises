using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.Models;
using System.Text.Json.Serialization;

namespace ProductShop
{
    internal class StartUp
    {
        static void Main(string[] args)
        {
            var db = new ProductShopContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            // Import Users
            var usersJson = File.ReadAllText(@"..\..\..\Datasets\users.json");
            Console.WriteLine(ImportUsers(db, usersJson));

            // Import Products
            var productsJson = File.ReadAllText(@"..\..\..\Datasets\products.json");
            Console.WriteLine(ImportProducts(db, productsJson));

            // Import Categories
            var categoriesJson = File.ReadAllText(@"..\..\..\Datasets\categories.json");
            Console.WriteLine(ImportCategories(db, categoriesJson));

            // Import CategoriesProducts
            var categoriesProductsJson = File.ReadAllText(@"..\..\..\Datasets\categories-products.json");
            Console.WriteLine(ImportCategoryProducts(db, categoriesProductsJson));

            // Export Products in Range
            File.WriteAllText(@"..\..\..\Datasets\exported-products.json", GetProductsInRange(db));

            // Export Successfully Sold Products
            File.WriteAllText(@"..\..\..\Datasets\exported-users-with-sales.json", GetSoldProducts(db));

            // Categories by Products Count
            File.WriteAllText(@"..\..\..\Datasets\exported-categories-by-product-count.json", GetCategoriesByProductsCount(db));

            //Export Users and Products
            File.WriteAllText(@"..\..\..\Datasets\users-with-products.json", GetUsersWithProducts(db));
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            try
            {
                var setting = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                };

                var usersToAdd = JsonConvert.DeserializeObject<List<User>>(inputJson, setting);

                foreach (var user in usersToAdd)
                {
                    context.Users.Add(user);
                }

                context.SaveChanges();

                return $"Successfully imported {usersToAdd.Count}";
            }
            catch (Exception ex)
            {
                return $"Error importing users: {ex.Message}";
            }
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            try
            {
                var productsToAdd = JsonConvert.DeserializeObject<List<Product>>(inputJson);

                foreach (var product in productsToAdd)
                {
                    context.Products.Add(product);
                }

                context.SaveChanges();

                return $"Successfully imported {productsToAdd.Count}";
            }
            catch (Exception ex)
            {
                return $"Error importing products: {ex.Message}";
            }
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            try
            {
                var categoriesToAdd = JsonConvert.DeserializeObject<List<Category>>(inputJson);

                foreach (var category in categoriesToAdd)
                {
                    context.Categories.Add(category);
                }

                context.SaveChanges();

                return $"Successfully imported {categoriesToAdd.Count}";
            }
            catch (Exception ex)
            {
                return $"Error importing categories: {ex.Message}";
            }
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            try
            {
                var categoriesProductsToAdd = JsonConvert.DeserializeObject<List<CategoryProduct>>(inputJson);

                foreach (var categoryProduct in categoriesProductsToAdd)
                {
                    context.CategoryProducts.Add(categoryProduct);
                }

                context.SaveChanges();

                return $"Successfully imported {categoriesProductsToAdd.Count}";
            }
            catch (Exception ex)
            {
                return $"Error importing categoriesProducts: {ex.Message}";
            }
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var productsInRange = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(x => new
                {
                    Name = x.Name,
                    Price = x.Price,
                    Seller = $"{x.Seller.FirstName} {x.Seller.LastName}"
                }).ToList();

            var options = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var productsJson = JsonConvert.SerializeObject(productsInRange, options);

            return productsJson;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var usersWithSales = context.Users
                .Where(u => u.ProductsSold.Any())
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(x => new
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold.Select(p => new
                    {
                        Name = p.Name,
                        Price = p.Price,
                        BuyerFirstName = p.Buyer.FirstName,
                        BuyerLastName = p.Buyer.LastName
                    }).ToList()
                }).ToList();

            var options = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var usersWithSalesJson = JsonConvert.SerializeObject(usersWithSales, options);

            return usersWithSalesJson;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .OrderByDescending(p => p.CategoryProducts.Count())
                .Select(x => new
                {
                    Category = x.Name,
                    ProductsCount = x.CategoryProducts.Count(),
                    AveragePrice = x.CategoryProducts.Any() ? $"{x.CategoryProducts.Average(p => p.Product.Price):F2}" : "0.00",
                    TotalRevenue = x.CategoryProducts.Any() ? $"{x.CategoryProducts.Sum(p => p.Product.Price):F2}" : "0.00"
                }).ToList();
            var options = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var categoriesJson = JsonConvert.SerializeObject(categories, options);

            return categoriesJson;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersWithProducts = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .OrderByDescending(u => u.ProductsSold.Count(p => p.BuyerId != null))
                .Select(u => new
                {
                    FisrtName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new
                    {
                        Count = u.ProductsSold.Count(p => p.BuyerId != null),
                        Products = u.ProductsSold
                        .Where(p => p.BuyerId != null)
                        .Select(p => new
                        {
                            Name = p.Name,
                            Price = p.Price
                        }).ToList()
                    }
                }).ToList();

            var exportData = new
            {
                UserCount = usersWithProducts.Count(),
                Users = usersWithProducts
            };

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            var usersJson = JsonConvert.SerializeObject(exportData, settings);

            return usersJson;
        }
    }
}
