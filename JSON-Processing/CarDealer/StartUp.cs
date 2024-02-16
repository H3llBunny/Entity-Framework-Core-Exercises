using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO.Export;
using CarDealer.DTO.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CarDealer
{
    internal class StartUp
    {
        static void Main(string[] args)
        {
            var db = new CarDealerContext();
            //db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            // Import Suppliers
            var supplierJson = File.ReadAllText(@"..\..\..\Datasets\suppliers.json");
            Console.WriteLine(ImportSuppliers(db, supplierJson));

            // Import Parts
            var partsJson = File.ReadAllText(@"..\..\..\Datasets\parts.json");
            Console.WriteLine(ImportParts(db, partsJson));

            // Import Cars
            var carsJson = File.ReadAllText(@"..\..\..\Datasets\cars.json");
            Console.WriteLine(ImportCars(db, carsJson));

            // Import Customers
            var customersJson = File.ReadAllText(@"..\..\..\Datasets\customers.json");
            Console.WriteLine(ImportCustomers(db, customersJson));

            // Import Sales
            var salesJson = File.ReadAllText(@"..\..\..\Datasets\sales.json");
            Console.WriteLine(ImportSales(db, salesJson));

            // Export Ordered Customers
            File.WriteAllText(@"..\..\..\Datasets\exported-customers.json", GetOrderedCustomers(db));

            // Cars from Make Toyota
            File.WriteAllText(@"..\..\..\Datasets\exported-toyota-cars.json", GetCarsFromMakeToyota(db));

            // Export Local Suppliers
            File.WriteAllText(@"..\..\..\Datasets\exported-local-suppliers.json", GetLocalSuppliers(db));

            // Export Cars with Their List of Parts
            File.WriteAllText(@"..\..\..\Datasets\exported-cars-with-partlist.json", GetCarsWithTheirListOfParts(db));

            // Export Total Sales by Customer
            File.WriteAllText(@"..\..\..\Datasets\exported-total-sales-by-customer.json", GetTotalSalesByCustomer(db));

            // Export Sales with Applied Discount
            File.WriteAllText(@"..\..\..\Datasets\exported-sales-with-applied-discount.json", GetSalesWithAppliedDiscount(db));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            try
            {
                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<CarDealerProfile>();
                });

                var mapper = mapperConfig.CreateMapper();

                var supplierDtos = JsonConvert.DeserializeObject<List<SuppliersImportDto>>(inputJson);

                var suppliers = mapper.Map<List<Supplier>>(supplierDtos);

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
                var mappingConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<CarDealerProfile>();
                });

                var mapper = mappingConfig.CreateMapper();

                var partsDto = JsonConvert.DeserializeObject<List<PartsImportDto>>(inputJson);

                var parts = mapper.Map<List<Part>>(partsDto);

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
                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<CarDealerProfile>();
                });

                var mapper = mapperConfig.CreateMapper();

                var carsDto = JsonConvert.DeserializeObject<List<CarsImportDto>>(inputJson);

                foreach (var carDto in carsDto)
                {
                    carDto.Parts = carDto.Parts
                        .GroupBy(p => p)
                        .Select(g => g.First())
                        .ToList();

                    var car = mapper.Map<Car>(carDto);
                    context.Cars.Add(car);
                    context.SaveChanges();
                }

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
                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<CarDealerProfile>();
                });

                var mapper = mapperConfig.CreateMapper();

                var customersDto = JsonConvert.DeserializeObject<List<CustomersImportDto>>(inputJson);

                var customers = mapper.Map<List<Customer>>(customersDto);

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
                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<CarDealerProfile>();
                });

                var mapper = mapperConfig.CreateMapper();

                var salesDto = JsonConvert.DeserializeObject<List<SalesImportDto>>(inputJson);

                var sales = mapper.Map<List<Sale>>(salesDto);

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
                .ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var customersDto = mapper.Map<List<CustomersExportDto>>(customers);

            var settings = new JsonSerializerSettings
            {
                DateFormatString = "MM/dd/yyyy",
                Formatting = Formatting.Indented
            };

            var customersJson = JsonConvert.SerializeObject(customersDto, settings);

            return customersJson;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var toyotaCars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(m => m.Model)
                .ThenByDescending(t => t.TravelledDistance)
                .ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var carsToyotaDto = mapper.Map<List<CarsToyotaExportDto>>(toyotaCars);

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            var carsJson = JsonConvert.SerializeObject(carsToyotaDto, settings);

            return carsJson;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var localSuppliers = context.Suppliers
                .Include(p => p.Parts)
                .Where(s => s.IsImporter == false)
                .ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var localSuppliersDto = mapper.Map<List<LocalSuppliersExportDto>>(localSuppliers);

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            var suppliersJson = JsonConvert.SerializeObject(localSuppliersDto, settings);

            return suppliersJson;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Include(pc => pc.PartCars)
                .ThenInclude(p => p.Part)
                .ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var carsDto = mapper.Map<List<CarsWithPartsExportDto.CarExportDto>>(cars);

            var carsJson = JsonConvert.SerializeObject(carsDto, Formatting.Indented);

            return carsJson;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Include(p => p.Sales)
                .ThenInclude(c => c.Car)
                .ThenInclude(pc => pc.PartCars)
                .ThenInclude(p => p.Part)
                .Where(c => c.Sales.Any())
                .OrderByDescending(s => s.Sales
                        .SelectMany(pc => pc.Car.PartCars)
                        .Sum(p => p.Part.Price))
                .ThenByDescending(s => s.Sales.Count())
                .ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var customersDto = mapper.Map<List<TotalSalesCustomersExportDto>>(customers);

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            var customersJson = JsonConvert.SerializeObject(customersDto, settings);

            return customersJson;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var customers = context.Customers
                .Include(s => s.Sales)
                .ThenInclude(c => c.Car)
                .ThenInclude(pc => pc.PartCars)
                .ThenInclude(p => p.Part)
                .Take(10)
                .ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var carAndCustomerDto = mapper.Map<List<SalesWithDiscountExportDto.SalesWithDiscountDto>>(customers);

            var carAndCustomersJson = JsonConvert.SerializeObject(carAndCustomerDto, Formatting.Indented);

            return carAndCustomersJson;
        }
    }
}
