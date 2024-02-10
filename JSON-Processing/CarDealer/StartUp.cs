using CarDealer.Data;
using CarDealer.Models;
using Castle.Core.Resource;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Security.Claims;
using System.Text.Json;

namespace CarDealer
{
    internal class StartUp
    {
        static void Main(string[] args)
        {
            var db = new CarDealerContext();
            //db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            //Import Suppliers
            //var supplierJson = File.ReadAllText(@"..\..\..\Datasets\suppliers.json");
            //Console.WriteLine(ImportSuppliers(db, supplierJson));

            //Import Parts
            //var partsJson = File.ReadAllText(@"..\..\..\Datasets\parts.json");
            //Console.WriteLine(ImportParts(db, partsJson));

            //Import Cars
            //var carsJson = File.ReadAllText(@"..\..\..\Datasets\cars.json");
            //Console.WriteLine(ImportCars(db, carsJson));

            //Import Customers
            //var customersJson = File.ReadAllText(@"..\..\..\Datasets\customers.json");
            //Console.WriteLine(ImportCustomers(db, customersJson));

            //Import Sales
            //var salesJson = File.ReadAllText(@"..\..\..\Datasets\sales.json");
            //Console.WriteLine(ImportSales(db, salesJson));

            // Import PartCars (there was none in the skeleton)

            //var random = new Random();

            //var partIds = db.Parts.Select(p => p.Id).Distinct().ToList();
            //var carIds = db.Cars.Select(c => c.Id).Distinct().ToList();

            //for (int i = 0; i < 200; i++)
            //{
            //    var randomPartId = partIds[random.Next(partIds.Count())];
            //    var randomCarId = carIds[random.Next(carIds.Count())];

            //    var newPartCar = new PartCar
            //    {
            //        PartId = randomPartId,
            //        CarId = randomCarId
            //    };

            //    db.PartCars.Add(newPartCar);
            //    db.SaveChanges();
            //}

            // Export Ordered Customers
            //File.WriteAllText(@"..\..\..\Datasets\exported-customers.json", GetOrderedCustomers(db));

            // Cars from Make Toyota
            //File.WriteAllText(@"..\..\..\Datasets\exported-toyota-cars.json", GetCarsFromMakeToyota(db));

            // Export Local Suppliers
            //File.WriteAllText(@"..\..\..\Datasets\exported-local-suppliers.json", GetLocalSuppliers(db));

            // Export Cars with Their List of Parts
            //File.WriteAllText(@"..\..\..\Datasets\exported-cars-with-partlist.json", GetCarsWithTheirListOfParts(db));

            // Export Total Sales by Customer
            //File.WriteAllText(@"..\..\..\Datasets\exported-total-sales-by-customer.json", GetTotalSalesByCustomer(db));

            // Export Sales with Applied Discount
            File.WriteAllText(@"..\..\..\Datasets\exported-sales-with-applied-discount.json", GetSalesWithAppliedDiscount(db));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            try
            {
                var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);

                foreach (var supplier in suppliers)
                {
                    context.Suppliers.Add(supplier);
                }

                context.SaveChanges();

                return $"Successfully imported suppliers: {context.Suppliers.Count()}.";
            }
            catch (Exception ex)
            {
                return $"Error importing suppliers: {ex.Message}.";
            }
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            try
            {
                var parts = JsonConvert.DeserializeObject<List<Part>>(inputJson);

                foreach (var part in parts)
                {
                    if (context.Suppliers.Any(s => s.Id == part.SupplierId))
                    {
                        context.Parts.Add(part);
                    }
                }

                context.SaveChanges();

                return $"Successfully imported parts: {context.Parts.Count()}.";
            }
            catch (Exception ex)
            {
                return $"Error importing parts: {ex.Message}.";
            }
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            try
            {
                var cars = JsonConvert.DeserializeObject<List<Car>>(inputJson);

                foreach (var car in cars)
                {
                    context.Cars.Add(car);
                }

                context.SaveChanges();

                return $"Successfully imported cars: {context.Cars.Count()}.";
            }
            catch (Exception ex)
            {
                return $"Error importing cars: {ex.Message}.";
            }
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            try
            {
                var customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson);

                foreach (var customer in customers)
                {
                    context.Customers.Add(customer);
                }
                context.SaveChanges();

                return $"Successfully imported customers: {context.Customers.Count()}.";
            }
            catch (Exception ex)
            {
                return $"Error importing customers: {ex.Message}.";
            }
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            try
            {
                var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);

                foreach (var sale in sales)
                {
                    context.Sales.Add(sale);
                }

                context.SaveChanges();

                return $"Successfully imported sales: {context.Sales.Count()}.";
            }
            catch (Exception ex)
            {
                return $"Error importing sales: {ex.Message}.";
            }
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(x => new
                {
                    Name = x.Name,
                    BirthDate = x.BirthDate,
                    IsYoungDriver = x.IsYoungDriver
                }).ToList();

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            var customersJson = JsonConvert.SerializeObject(customers, settings);

            return customersJson;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var toyotaCars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(m => m.Model)
                .ThenByDescending(t => t.TravelledDistance)
                .Select(x => new
                {
                    Id = x.Id,
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                }).ToList();

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            var carsJson = JsonConvert.SerializeObject(toyotaCars, settings);

            return carsJson;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var localSuppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count()
                }).ToList();

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            var suppliersJson = JsonConvert.SerializeObject(localSuppliers, settings);

            return suppliersJson;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(x => new
                {
                    car = new
                    {
                        Make = x.Make,
                        Model = x.Model,
                        TravelledDistance = x.TravelledDistance
                    },
                    parts = x.PartCars.Select(p => new
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price.ToString("F2")
                    }).ToList()
                }).ToList();

            var carsJson = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return carsJson;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count(),
                    TotalSpentMoney = c.Sales
                        .SelectMany(s => s.Car.PartCars)
                        .Sum(pc => pc.Part.Price)
                })
                .OrderByDescending(c => c.TotalSpentMoney)
                .ThenByDescending(c => c.BoughtCars)
                .ToList();

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            };

            var customersJson = JsonConvert.SerializeObject(customers, settings);

            return customersJson;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var cars = context.Sales
                .Select(x => new
                {
                    car = new
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },
                    customerName = x.Customer.Name,
                    Discount = x.Discount,
                    price = x.Car.PartCars.Sum(pc => pc.Part.Price),
                    priceWithDiscount = (x.Car.PartCars.Sum(pc => pc.Part.Price) * (1 - (x.Discount / 100))).ToString("F2")
                }); ;

            var carsJson = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return carsJson;
        }
    }
}
