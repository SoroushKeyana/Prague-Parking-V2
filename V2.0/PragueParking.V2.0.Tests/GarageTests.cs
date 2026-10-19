
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PragueParking.V2._0.Core.Config;
using PragueParking.V2._0.Core.Models;
using PragueParking.V2_0.Core.Models;

namespace PragueParking.V2._0.Tests;

[TestClass]
public class GarageTests
{
    private static GarageConfig CreateConfig(int numberOfSpots = 5)
    {
        return new GarageConfig
        {
            NumberOfSpots = numberOfSpots,
            VehicleTypes = new List<VehicleTypeConfig>
            {
                new() { Name = "Car", VehiclesPerSpot = 1 },
                new() { Name = "MC", VehiclesPerSpot = 2 }
            }
        };
    }

    [TestMethod]
    public void Cannot_Park_Third_MC_In_One_Spot()
    {
        // Arrange
        var garage = new ParkingGarage(CreateConfig(1));

        var mc1 = new MC { RegistrationNumber = "MC001" };
        var mc2 = new MC { RegistrationNumber = "MC002" };
        var mc3 = new MC { RegistrationNumber = "MC003" };

        // Act
        bool firstParked = garage.ParkVehicle(mc1, out _);
        bool secondParked = garage.ParkVehicle(mc2, out _);
        bool thirdParked = garage.ParkVehicle(mc3, out _);

        // Assert
        Assert.IsTrue(firstParked);
        Assert.IsTrue(secondParked);
        Assert.IsFalse(thirdParked);
        Assert.AreEqual(2, garage.Spots[0].Vehicles.Count);
    }

    [TestMethod]
    public void Duplicate_Registration_Is_Rejected()
    {
        // Arrange
        var garage = new ParkingGarage(CreateConfig());
        var firstCar = new Car { RegistrationNumber = "ABC123" };
        var secondCar = new Car { RegistrationNumber = "ABC123" };

        // Act
        bool firstParked = garage.ParkVehicle(firstCar, out _);
        bool secondParked = garage.ParkVehicle(secondCar, out _);

        // Assert
        Assert.IsTrue(firstParked);
        Assert.IsFalse(secondParked);
    }

    [TestMethod]
    public void Retrieve_Removes_Vehicle()
    {
        // Arrange
        var garage = new ParkingGarage(CreateConfig(1));
        var car = new Car { RegistrationNumber = "ABC123" };
        garage.ParkVehicle(car, out int spotNumber);

        // Act
        var retrieved = garage.RetrieveVehicle("ABC123");

        // Assert
        Assert.IsNotNull(retrieved);
        Assert.AreEqual(spotNumber, 1);
        Assert.IsTrue(garage.Spots[0].IsEmpty);
        Assert.IsNull(garage.FindVehicle("ABC123"));
    }

    [TestMethod]
    public void Move_To_Full_Spot_Fails_And_Vehicle_Stays()
    {
        // Arrange
        var garage = new ParkingGarage(CreateConfig(3));

        var car1 = new Car { RegistrationNumber = "CAR001" };
        var car2 = new Car { RegistrationNumber = "CAR002" };
        var mc1 = new MC { RegistrationNumber = "MC001" };
        var mc2 = new MC { RegistrationNumber = "MC002" };

        garage.ParkVehicle(car1, out int originalSpot);
        garage.ParkVehicle(car2, out int fullCarSpot);
        garage.ParkVehicle(mc1, out int mcSpot1);
        garage.ParkVehicle(mc2, out int mcSpot2);

        // Act
        bool moved = garage.MoveVehicle(
            "CAR001", fullCarSpot, out string error);

        var currentLocation = garage.FindVehicle("CAR001");

        // Assert
        Assert.IsFalse(moved);
        Assert.IsFalse(string.IsNullOrWhiteSpace(error));
        Assert.IsNotNull(currentLocation);
        Assert.AreEqual(originalSpot, currentLocation.Value.Spot.SpotNumber);
    }
}
