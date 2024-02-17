using AutoMapper;
using CarDealerXml.Data;
using CarDealerXml.DTOS.Export;
using CarDealerXml.DTOS.Import;
using CarDealerXml.Models;
using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CarDealerXml
{
    internal class StartUp
    {
        static void Main(string[] args)
        {
            var db = new CarDealerContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            // Import Suppliers
            var suppliers = File.ReadAllText(@"..\..\..\Datasets\suppliers.xml");
            Console.WriteLine(ImportSuppliers(db, suppliers));

            // Import Parts
            var parts = File.ReadAllText(@"..\..\..\Datasets\parts.xml");
            Console.WriteLine(ImportParts(db, parts));

            // Import Cars
            var cars = File.ReadAllText(@"..\..\..\Datasets\cars.xml");
            Console.WriteLine(ImportCars(db, cars));

            // Import Customers
            var customers = File.ReadAllText(@"..\..\..\Datasets\customers.xml");
            Console.WriteLine(ImportCustomers(db, customers));

            // Import Sales
            var sales = File.ReadAllText(@"..\..\..\Datasets\sales.xml");
            Console.WriteLine(ImportSales(db, sales));

            // Cars With Distance
            var carsWithDistance = GetCarsWithDistance(db);
            File.WriteAllText(@"..\..\..\Datasets\cars-with-distance.xml", carsWithDistance);

            // Cars from make BMW
            var bmwCars = GetCarsFromMakeBmw(db);
            File.WriteAllText(@"..\..\..\Datasets\bmw-cars.xml", bmwCars);

            // Local Suppliers
            var localSuppliers = GetLocalSuppliers(db);
            File.WriteAllText(@"..\..\..\Datasets\local-suppliers.xml", localSuppliers);

            // Cars with Their List of Parts
            var carsWithParts = GetCarsWithTheirListOfParts(db);
            File.WriteAllText(@"..\..\..\Datasets\cars-and-parts.xml", carsWithParts);

            // Total Sales by Customer
            var customersTotalSales = GetTotalSalesByCustomer(db);
            File.WriteAllText(@"..\..\..\Datasets\customers-total-sales.xml", customersTotalSales);

            // Sales with Applied Discount
            var salesWithDiscount = GetSalesWithAppliedDiscount(db);
            File.WriteAllText(@"..\..\..\Datasets\sales-discounts.xml", salesWithDiscount);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<SuppliersImportDto>), new XmlRootAttribute("Suppliers"));

                var seuupliersDto = (List<SuppliersImportDto>)serializer.Deserialize(new StringReader(inputXml));

                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<CarDealerProfile>();
                });

                var mapper = mapperConfig.CreateMapper();

                var suppliersToAdd = mapper.Map<List<Supplier>>(seuupliersDto);

                context.Suppliers.AddRange(suppliersToAdd);
                context.SaveChanges();

                return $"Successfully imported {context.Suppliers.Count()} suppliers.";
            }
            catch (Exception ex)
            {
                return $"Error importing suppliers: {ex.Message}";
            }
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<PartsImportDto>), new XmlRootAttribute("Parts"));

                var partsDto = (List<PartsImportDto>)serializer.Deserialize(new StringReader(inputXml));

                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<CarDealerProfile>();
                });

                var mapper = mapperConfig.CreateMapper();

                var partsToAdd = mapper.Map<List<Part>>(partsDto);

                foreach (var part in partsToAdd)
                {
                    if (context.Suppliers.Any(s => s.Id == part.SupplierId))
                    {
                        context.Parts.Add(part);
                    }
                }

                context.SaveChanges();

                return $"Successfully imported {context.Parts.Count()} parts.";
            }
            catch (Exception ex)
            {
                return $"Error importing parts: {ex.Message}";
            }
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<CarsImportDto>), new XmlRootAttribute("Cars"));

                var carsDto = (List<CarsImportDto>)serializer.Deserialize(new StringReader(inputXml));

                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<CarDealerProfile>();
                });

                var mapper = mapperConfig.CreateMapper();

                foreach (var carDto in carsDto)
                {
                    // Remove duplicate part IDs from carDto.Parts
                    carDto.Parts = carDto.Parts
                        .GroupBy(p => p.Id)
                        .Select(g => g.First()) // Keep only the first occurrence of each part ID
                        .ToList();

                    var car = mapper.Map<Car>(carDto);

                    context.Cars.Add(car);
                    context.SaveChanges();
                }

                return $"Successfully imported {context.Cars.Count()} cars.";

            }
            catch (Exception ex)
            {
                return $"Error importing cars: {ex.Message}";
            }
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<CustomersImportDto>), new XmlRootAttribute("Customers"));

                var customersDto = (List<CustomersImportDto>)serializer.Deserialize(new StringReader(inputXml));

                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<CarDealerProfile>();
                });

                var mapper = mapperConfig.CreateMapper();

                var customerdsToAdd = mapper.Map<List<Customer>>(customersDto);

                context.Customers.AddRange(customerdsToAdd);
                context.SaveChanges();

                return $"Successfully imported {context.Customers.Count()} customers.";
            }
            catch (Exception ex)
            {
                return $"Error importing customers: {ex.Message}";
            }
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<SalesImportDto>), new XmlRootAttribute("Sales"));

                var salesDto = (List<SalesImportDto>)serializer.Deserialize(new StringReader(inputXml));


                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<CarDealerProfile>();
                });

                var mapper = mapperConfig.CreateMapper();

                var salesToAdd = mapper.Map<List<Sale>>(salesDto);

                foreach (var sale in salesToAdd)
                {
                    if (context.Cars.Any(c => c.Id == sale.CarId)
                        && context.Customers.Any(c => c.Id == sale.CustomerId))
                    {
                        context.Sales.Add(sale);
                    }
                }

                context.SaveChanges();

                return $"Successfully imported {context.Sales.Count()} sales.";
            }
            catch (Exception ex)
            {
                return $"Error importing sales: {ex.Message}";
            }
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.TravelledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var carsDto = mapper.Map<List<CarsWithDistanceExportDto>>(cars);

            var serializer = new XmlSerializer(typeof(List<CarsWithDistanceExportDto>), new XmlRootAttribute("cars"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", ""); // Add an empty namespace mapping

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, carsDto, namespaces);
                return writer.ToString();
            }
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var bmwCars = context.Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var bmwCarsDto = mapper.Map<List<BmwCarsExportDto>>(bmwCars);

            var serializer = new XmlSerializer(typeof(List<BmwCarsExportDto>), new XmlRootAttribute("cars"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", ""); // Add an empty namespace mapping

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, bmwCarsDto, namespaces);
                return writer.ToString();
            }
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Include(p => p.Parts)
                .Where(s => s.IsImporter == false)
                .ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var suppliersDto = mapper.Map<List<LocalSuppliersExportDto>>(suppliers);

            var serializer = new XmlSerializer(typeof(List<LocalSuppliersExportDto>), new XmlRootAttribute("suppliers"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", ""); // Add an empty namespace mapping

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, suppliersDto, namespaces);
                return writer.ToString();
            }
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Include(pc => pc.PartCars)
                .ThenInclude(p => p.Part)
                .Select(c => new CarsAndPartsExportDto.CarDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars.Select(pc => new CarsAndPartsExportDto.PartDto
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price
                    }).OrderByDescending(pc => pc.Price).ToList()
                }).ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var carsWithPartsDto = mapper.Map<List<CarsAndPartsExportDto.CarDto>>(cars);

            var serializer = new XmlSerializer(typeof(List<CarsAndPartsExportDto.CarDto>), new XmlRootAttribute("cars"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", ""); // Add an empty namespace mapping

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, carsWithPartsDto, namespaces);
                return writer.ToString();
            }
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(s => s.Sales.Any())
                .Include(s => s.Sales)
                .ThenInclude(pc => pc.Car.PartCars)
                .ThenInclude(p => p.Part)
                .OrderByDescending(s => s.Sales.SelectMany(s => s.Car.PartCars).Sum(p => p.Part.Price))
                .ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var customersDto = mapper.Map<List<CustomersAndSalesExportDto>>(customers);

            var serializer = new XmlSerializer(typeof(List<CustomersAndSalesExportDto>), new XmlRootAttribute("customers"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", ""); // Add an empty namespace mapping

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, customersDto, namespaces);
                return writer.ToString();
            }
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
            .Include(s => s.Car)
                .ThenInclude(c => c.PartCars)
                    .ThenInclude(pc => pc.Part)
            .Include(s => s.Customer)
            .ToList();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            var mapper = mapperConfig.CreateMapper();

            var salesDto = mapper.Map<List<SaleDto>>(sales);

            var salesWithDiscountsDto = new SalesWithDiscountsExportDto
            {
                Sales = salesDto
            };

            var serializer = new XmlSerializer(typeof(SalesWithDiscountsExportDto));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", ""); // Add an empty namespace mapping

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, salesWithDiscountsDto, namespaces);
                return writer.ToString();
            }
        }
    }
}
