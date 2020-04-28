using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();
            //context.Database.EnsureCreated();
            //var supplierJson = File.ReadAllText
            //    (@"C:\Users\Yanmario\Desktop\Entity Framework Core\08.JSON PROCESSING\Car Dealer\CarDealer\Datasets\suppliers.json");
            //var carsJson = File.ReadAllText
            //    (@"C:\Users\Yanmario\Desktop\Entity Framework Core\08.JSON PROCESSING\Car Dealer\CarDealer\Datasets\cars.json");
            //var customersJson = File.ReadAllText
            //    (@"C:\Users\Yanmario\Desktop\Entity Framework Core\08.JSON PROCESSING\Car Dealer\CarDealer\Datasets\customers.json");
            //var partsJson = File.ReadAllText
            //    (@"C:\Users\Yanmario\Desktop\Entity Framework Core\08.JSON PROCESSING\Car Dealer\CarDealer\Datasets\parts.json");
            //var salesJson = File.ReadAllText
            //    (@"C:\Users\Yanmario\Desktop\Entity Framework Core\08.JSON PROCESSING\Car Dealer\CarDealer\Datasets\sales.json");

            var result = GetSalesWithAppliedDiscount(context);
            Console.WriteLine(result);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);
            context.AddRange(suppliers);
            context.SaveChanges();
            return $"Successfully imported {suppliers.Length}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var parts = JsonConvert.DeserializeObject<Part[]>(inputJson)
                .Where(p => context.Suppliers.Any(s => s.Id == p.SupplierId))
                .ToList();
            context.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {parts.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carsDto = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);

            var carsCollection = new List<Car>();
            var carParts = new List<PartCar>();

            foreach (var carDto in carsDto)
            {
                var car = new Car()
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TravelledDistance
                };
                foreach (var part in carDto.PartsId.Distinct())
                { 
                    var carPart = new PartCar()
                    {
                        PartId = part,
                        Car = car
                    };

                    carParts.Add(carPart);
                }
                carsCollection.Add(car);
            }
            context.Cars.AddRange(carsCollection);
            context.PartCars.AddRange(carParts);
            context.SaveChanges();
           return $"Successfully imported {carsDto.Length}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);
            context.AddRange(customers);
            context.SaveChanges();
            return $"Successfully imported {customers.Length}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);
            context.AddRange(sales);
            context.SaveChanges();
            return $"Successfully imported {sales.Length}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToArray();

            var json = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return json;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var carsFromToyota = context.Cars
                .Where(m => m.Make == "Toyota")
                .OrderBy(m => m.Model)
                .ThenByDescending(t => t.TravelledDistance)
                .Select(c => new
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToArray();

            var json = JsonConvert.SerializeObject(carsFromToyota, Formatting.Indented);
            return json;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var supplier = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count()
                })
                .ToArray();

            var json = JsonConvert.SerializeObject(supplier, Formatting.Indented);
            return json;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TravelledDistance = c.TravelledDistance
                    },
                    parts = c.PartCars
                        .Select(p => new
                        {
                            Name = p.Part.Name,
                            Price = $"{p.Part.Price:F2}"
                        }).ToArray()
                    
                })
                .ToArray();

            var json = JsonConvert.SerializeObject(cars, Formatting.Indented);
            return json;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customer = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count(),
                    spentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(p => p.Part.Price))
                })
                .OrderByDescending(m => m.spentMoney)
                .ThenByDescending(b => b.boughtCars)
                .ToArray();

            var json = JsonConvert.SerializeObject(customer, Formatting.Indented);
            return json;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(s => new
                {
                    car = new
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },
                    customerName = s.Customer.Name,
                    Discount = $"{s.Discount:F2}",
                    price = $"{s.Car.PartCars.Sum(c => c.Part.Price):F2}",
                    priceWithDiscount = $"{s.Car.PartCars.Sum(c => c.Part.Price) - s.Car.PartCars.Sum(c => c.Part.Price) * s.Discount / 100:F2}"
                })
                .Take(10)
                .ToArray();

            var json = JsonConvert.SerializeObject(sales, Formatting.Indented);
            return json;
        }
    }
}