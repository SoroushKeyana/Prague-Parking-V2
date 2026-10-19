
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PragueParking.V2._0.Core.Models;

namespace PragueParking.V2._0.Tests;

[TestClass]
public class PriceTests
{
    private static PriceList CreatePriceList()
    {
        return new PriceList
        {
            PricePerHour = new Dictionary<string, decimal>
            {
                { "Car", 20m },
                { "MC", 10m }
            },
            FreeMinutes = 10
        };
    }

    [TestMethod]
    public void Price_Is_Zero_Under_10_Minutes()
    {
        // Arrange
        var priceList = CreatePriceList();
        var car = new Car
        {
            ArrivalTime = new DateTime(2026, 1, 1, 10, 0, 0)
        };
        var departure = car.ArrivalTime.AddMinutes(9);

        // Act
        decimal price = priceList.CalculatePrice(car, departure);

        // Assert
        Assert.AreEqual(0m, price);
    }

    [TestMethod]
    public void Price_Is_Charged_At_Exactly_10_Minutes()
    {
        // Arrange
        var priceList = CreatePriceList();
        var car = new Car
        {
            ArrivalTime = new DateTime(2026, 1, 1, 10, 0, 0)
        };
        var departure = car.ArrivalTime.AddMinutes(10);

        // Act
        decimal price = priceList.CalculatePrice(car, departure);

        // Assert
        Assert.AreEqual(20m, price);
    }

    [TestMethod]
    public void Car_Parked_90_Minutes_Costs_40()
    {
        // Arrange
        var priceList = CreatePriceList();
        var car = new Car
        {
            ArrivalTime = new DateTime(2026, 1, 1, 10, 0, 0)
        };
        var departure = car.ArrivalTime.AddMinutes(90);

        // Act
        decimal price = priceList.CalculatePrice(car, departure);

        // Assert
        Assert.AreEqual(40m, price);
    }

    [TestMethod]
    public void MC_Parked_61_Minutes_Costs_20()
    {
        // Arrange
        var priceList = CreatePriceList();
        var motorcycle = new MC
        {
            ArrivalTime = new DateTime(2026, 1, 1, 10, 0, 0)
        };
        var departure = motorcycle.ArrivalTime.AddMinutes(61);

        // Act
        decimal price = priceList.CalculatePrice(motorcycle, departure);

        // Assert
        Assert.AreEqual(20m, price);
    }
}
