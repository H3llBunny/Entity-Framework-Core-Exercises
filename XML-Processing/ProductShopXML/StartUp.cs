using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductShopXML.Data;
using ProductShopXML.DTO.Export;
using ProductShopXML.DTO.ExportDtos;
using ProductShopXML.DTO.Import;
using ProductShopXML.Models;
using System.Diagnostics;
using System.Xml.Serialization;

namespace ProductShopXML
{
    internal class StartUp
    {
        static void Main(string[] args)
        {
            var db = new ProductShopContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            // Import Users
            var usersXml = File.ReadAllText(@"..\..\..\Datasets\users.xml");
            Console.WriteLine(ImportUsers(db, usersXml));

            // Import Products
            var productsXml = File.ReadAllText(@"..\..\..\Datasets\products.xml");
            Console.WriteLine(ImportProducts(db, productsXml));

            // Import Categories
            var categoriesXml = File.ReadAllText(@"..\..\..\Datasets\categories.xml");
            Console.WriteLine(ImportCategories(db, categoriesXml));

            // Import Categories and Products
            var categoriesProductsXml = File.ReadAllText(@"..\..\..\Datasets\categories-products.xml");
            Console.WriteLine(ImportCategoryProducts(db, categoriesProductsXml));

            // Export Products In Range
            var productsInRange = GetProductsInRange(db);
            File.WriteAllText(@"..\..\..\Datasets\products-in-range.xml", productsInRange);

            // Export Sold Products
            var soldProducts = GetSoldProducts(db);
            File.WriteAllText(@"..\..\..\Datasets\sold-products.xml", soldProducts);

            // Export Categories By Products Count
            var categories = GetCategoriesByProductsCount(db);
            File.WriteAllText(@"..\..\..\Datasets\categories-product-count.xml", categories);

            // Export Users and Products
            var usersAndProducts = GetUsersWithProducts(db);
            File.WriteAllText(@"..\..\..\Datasets\users-and-products.xml", usersAndProducts);
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<UsersImportDto>), new XmlRootAttribute("Users"));

                var usersDto = (List<UsersImportDto>)serializer.Deserialize(new StringReader(inputXml));

                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProductShopProfile>();
                });

                var mapper = mapperConfig.CreateMapper();

                var usersToAdd = mapper.Map<List<User>>(usersDto);

                context.Users.AddRange(usersToAdd);

                context.SaveChanges();

                return $"Successfully imported {context.Users.Count()} users.";
            }
            catch (Exception ex)
            {
                return $"Error importing users: {ex.Message}.";
            }
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<ProductsImportDto>), new XmlRootAttribute("Products"));

                var productsDto = (List<ProductsImportDto>)serializer.Deserialize(new StringReader(inputXml));

                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProductShopProfile>();
                });

                var mapper = mapperConfig.CreateMapper();

                var productsToAdd = mapper.Map<List<Product>>(productsDto);

                context.Products.AddRange(productsToAdd);

                context.SaveChanges();

                return $"Successfully imported {context.Products.Count()} products";
            }
            catch (Exception ex)
            {
                return $"Error importing products: {ex.Message}.";
            }
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<CategoriesImportDto>), new XmlRootAttribute("Categories"));

                var categoriesDto = (List<CategoriesImportDto>)serializer.Deserialize(new StringReader(inputXml));

                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProductShopProfile>();
                });

                var mapper = mapperConfig.CreateMapper();

                var categoriesToAdd = mapper.Map<List<Category>>(categoriesDto);

                context.Categories.AddRange(categoriesToAdd);

                context.SaveChanges();

                return $"Successfully imported {context.Products.Count()} categories";
            }
            catch (Exception ex)
            {
                return $"Error importing categories: {ex.Message}.";
            }
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<CategoryProductsImportDto>), new XmlRootAttribute("CategoryProducts"));

                var categoryProductsDto = (List<CategoryProductsImportDto>)serializer.Deserialize(new StringReader(inputXml));

                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProductShopProfile>();
                });

                var mapper = mapperConfig.CreateMapper();

                var ctegoryProductsToAdd = mapper.Map<List<CategoryProduct>>(categoryProductsDto);

                context.CategoryProducts.AddRange(ctegoryProductsToAdd);

                context.SaveChanges();

                return $"Successfully imported {context.Products.Count()} categoryProducts";
            }
            catch (Exception ex)
            {
                return $"Error importing categoryProducts: {ex.Message}.";
            }
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var productDtos = mapper.Map<List<ProductExportDto>>(products);

            var serializer = new XmlSerializer(typeof(List<ProductExportDto>), new XmlRootAttribute("Products"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", ""); // Add an empty namespace mapping

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, productDtos, namespaces);
                return writer.ToString();
            }
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var soldProducts = context.Users
                .Where(p => p.ProductsSold.Any())
                .Include(ps => ps.ProductsSold)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .Take(5)
                .ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var soldProductsDto = mapper.Map<List<SoldProductsExportDto.UserDtop>>(soldProducts);

            var serializer = new XmlSerializer(typeof(List<SoldProductsExportDto.UserDtop>), new XmlRootAttribute("Users"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", ""); // Add an empty namespace mapping

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, soldProductsDto, namespaces);
                return writer.ToString();
            }
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categoriest = context.Categories
                .Include(cp => cp.CategoryProducts)
                .ThenInclude(p => p.Product)
                .OrderByDescending(cp => cp.CategoryProducts.Select(p => p.Product).Count())
                .ThenByDescending(cp => cp.CategoryProducts.Sum(p => p.Product.Price))
                .ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var categoriesDto = mapper.Map<List<CategorisByProductCountExportDto>>(categoriest);

            var serializer = new XmlSerializer(typeof(List<CategorisByProductCountExportDto>), new XmlRootAttribute("Categories"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", ""); // Add an empty namespace mapping

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, categoriesDto, namespaces);
                return writer.ToString();
            }
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersWithProducts = context.Users
                .Where(ps => ps.ProductsSold.Any())
                .OrderByDescending(ps => ps.ProductsSold.Count())
                .Take(10)
                .Select(u => new UsersAndProductsExportDto.UsersDto()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new UsersAndProductsExportDto.ProductsDto()
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold
                            .OrderByDescending(p => p.Price)
                            .Select(p => new UsersAndProductsExportDto.ProductDto()
                            {
                                Name = p.Name,
                                Price = p.Price
                            }).ToList()
                    }
                }).ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var userdAndProductsDto = mapper.Map<List<UsersAndProductsExportDto.UsersDto>>(usersWithProducts);

            var serializer = new XmlSerializer(typeof(List<UsersAndProductsExportDto.UsersDto>), new XmlRootAttribute("Users"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", ""); // Add an empty namespace mapping

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, userdAndProductsDto, namespaces);
                return writer.ToString();
            }
        }
    }
}
