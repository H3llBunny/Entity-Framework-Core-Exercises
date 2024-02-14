using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;

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

            // Export Users and Products
            File.WriteAllText(@"..\..\..\Datasets\users-with-products.json", GetUsersWithProducts(db));
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            try
            {
                var usersDto = JsonConvert.DeserializeObject<List<UsersImportDto>>(inputJson);

                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProductShopProfile>();
                });

                var mapper = mapperConfig.CreateMapper();

                var usersToAdd = mapper.Map<List<User>>(usersDto);

                context.Users.AddRange(usersToAdd);

                context.SaveChanges();

                return $"Successfully imported {context.Users.Count()}";
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
                var productsDto = JsonConvert.DeserializeObject<List<ProductsImportDto>>(inputJson);

                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProductShopProfile>();
                });

                var mapper = mapperConfig.CreateMapper();

                var productsToAdd = mapper.Map<List<Product>>(productsDto);

                context.Products.AddRange(productsToAdd);

                context.SaveChanges();

                return $"Successfully imported {context.Products.Count()}";
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
                var categoriesDto = JsonConvert.DeserializeObject<List<CategoriesImportDto>>(inputJson);

                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProductShopProfile>();
                });

                var mapper = mapperConfig.CreateMapper();

                var categoriesToAdd = mapper.Map<List<Category>>(categoriesDto);

                foreach (var category in categoriesToAdd)
                {
                    if (category.Name != null)
                    {
                        context.Categories.Add(category);
                    }
                }

                context.SaveChanges();

                return $"Successfully imported {context.Categories.Count()}";
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
                var categoriesProductsDto = JsonConvert.DeserializeObject<List<CategoryProduct>>(inputJson);

                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProductShopProfile>();
                });

                var mapper = mapperConfig.CreateMapper();

                var categoriesProductsToAdd = mapper.Map<List<CategoryProduct>>(categoriesProductsDto);

                foreach (var categoryProduct in categoriesProductsToAdd)
                {
                    if (context.Categories.Any(c => c.Id == categoryProduct.CategoryId
                    && context.Products.Any(p => p.Id == categoryProduct.ProductId)))
                    {
                        context.CategoryProducts.Add(categoryProduct);

                    }
                }

                context.SaveChanges();

                return $"Successfully imported {context.CategoryProducts.Count()}";
            }
            catch (Exception ex)
            {
                return $"Error importing categoriesProducts: {ex.Message}";
            }
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var productsInRange = context.Products
                .Include(s => s.Seller)
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var productsInRangeDto = mapper.Map<List<ProductsInRangeExportDto>>(productsInRange);

            var options = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var productsJson = JsonConvert.SerializeObject(productsInRangeDto, options);

            return productsJson;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var usersWithSales = context.Users
                .Include(p => p.ProductsSold)
                .Where(p => p.ProductsSold.Any())
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var usersWithProductsDto = mapper.Map<List<SoldProductsExportDto.SellerWithProductsDto>>(usersWithSales);

            var options = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            var usersWithSalesJson = JsonConvert.SerializeObject(usersWithProductsDto, options);

            return usersWithSalesJson;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Include(cp => cp.CategoryProducts)
                .ThenInclude(p => p.Product)
                .OrderByDescending(p => p.CategoryProducts.Count())
                .ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var categoriesDto = mapper.Map<List<CategoryProductExportDto>>(categories);

            var options = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var categoriesJson = JsonConvert.SerializeObject(categoriesDto, options);

            return categoriesJson;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersWithProducts = context.Users
                .Include(ps => ps.ProductsSold)
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .OrderByDescending(ps => ps.ProductsSold.Count())
                .ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var usersWithProductsDto = mapper.Map<List<UsersAndProductsExportDto.UsersDto>>(usersWithProducts);

            var exportData = new UsersAndProductsExportDto
            {
                UsersCount = usersWithProductsDto.Count(),
                Users = usersWithProductsDto
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
