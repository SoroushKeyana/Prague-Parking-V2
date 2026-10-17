
using PragueParking.V2._0.Core.Config;
using PragueParking.V2._0.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PragueParking.V2_0.Core.Models;

public class ParkingGarage
{
    private readonly GarageConfig _config;

    public List<ParkingSpot> Spots { get; } = new();

    public int FreeSpotCount =>
        Spots.Count(spot => spot.IsEmpty);

    public int PartialSpotCount =>
        Spots.Count(spot =>
            !spot.IsEmpty &&
            !spot.IsFull(
                _config.GetVehiclePerSpot(spot.Vehicles[0].TypeName)));

    public int FullSpotCount =>
        Spots.Count(spot =>
            !spot.IsEmpty &&
            spot.IsFull(
                _config.GetVehiclePerSpot(spot.Vehicles[0].TypeName)));

    public ParkingGarage(GarageConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));

        if (config.NumberOfSpots < 0)
        {
            throw new ArgumentException(
                "Number of spots cannot be negative.", nameof(config));
        }

        for (int i = 1; i <= config.NumberOfSpots; i++)
        {
            Spots.Add(new ParkingSpot { SpotNumber = i });
        }
    }

    public static string NormalizeReg(string reg)
    {
        return reg.Trim().ToUpperInvariant();
    }

    public bool RegistrationExists(string reg)
    {
        string normalizedReg = NormalizeReg(reg);

        return Spots
            .SelectMany(spot => spot.Vehicles)
            .Any(vehicle =>
                NormalizeReg(vehicle.RegistrationNumber) == normalizedReg);
    }

    public (ParkingSpot Spot, Vehicle Vehicle)? FindVehicle(string reg)
    {
        string normalizedReg = NormalizeReg(reg);

        foreach (var spot in Spots)
        {
            var vehicle = spot.Vehicles.FirstOrDefault(v =>
                NormalizeReg(v.RegistrationNumber) == normalizedReg);

            if (vehicle != null)
            {
                return (spot, vehicle);
            }
        }

        return null;
    }

    public bool ParkVehicle(Vehicle v, out int spotNumber)
    {
        spotNumber = 0;

        if (RegistrationExists(v.RegistrationNumber))
        {
            return false;
        }

        v.RegistrationNumber = NormalizeReg(v.RegistrationNumber);

        int capacity;

        try
        {
            capacity = _config.GetVehiclePerSpot(v.TypeName);
        }
        catch (ArgumentException)
        {
            return false;
        }

        if (capacity <= 0)
        {
            return false;
        }

        // First pass: use a partially filled spot of the same type.
        foreach (var spot in Spots)
        {
            if (!spot.IsEmpty && spot.CanPark(v, capacity))
            {
                spot.Park(v);
                spotNumber = spot.SpotNumber;
                return true;
            }
        }

        // Second pass: use the first empty spot.
        foreach (var spot in Spots)
        {
            if (spot.IsEmpty && spot.CanPark(v, capacity))
            {
                spot.Park(v);
                spotNumber = spot.SpotNumber;
                return true;
            }
        }

        return false;
    }

    public Vehicle? RetrieveVehicle(string reg)
    {
        var result = FindVehicle(reg);

        if (result == null)
        {
            return null;
        }

        var (spot, vehicle) = result.Value;

        if (!spot.Remove(vehicle))
        {
            return null;
        }

        return vehicle;
    }

    public bool MoveVehicle(
        string reg,
        int targetSpotNumber,
        out string error)
    {
        error = "";

        var result = FindVehicle(reg);

        if (result == null)
        {
            error = $"Vehicle '{reg}' was not found.";
            return false;
        }

        var (currentSpot, vehicle) = result.Value;

        if (targetSpotNumber < 1 || targetSpotNumber > Spots.Count)
        {
            error = $"Spot number {targetSpotNumber} is out of range.";
            return false;
        }

        var targetSpot = Spots[targetSpotNumber - 1];

        if (targetSpot.SpotNumber == currentSpot.SpotNumber)
        {
            error = "The vehicle is already in that spot.";
            return false;
        }

        int capacity;

        try
        {
            capacity = _config.GetVehiclePerSpot(vehicle.TypeName);
        }
        catch (ArgumentException)
        {
            error = $"Vehicle type '{vehicle.TypeName}' is not configured.";
            return false;
        }

        if (capacity <= 0 || !targetSpot.CanPark(vehicle, capacity))
        {
            error = $"Vehicle cannot be parked in spot {targetSpotNumber}.";
            return false;
        }

        // Validate everything before removing the vehicle.
        if (!currentSpot.Remove(vehicle))
        {
            error = "Could not remove the vehicle from its current spot.";
            return false;
        }

        targetSpot.Park(vehicle);
        return true;
    }

    public List<ParkedVehicleRecord> ToRecords()
    {
        var records = new List<ParkedVehicleRecord>();

        foreach (var spot in Spots)
        {
            foreach (var vehicle in spot.Vehicles)
            {
                records.Add(new ParkedVehicleRecord
                {
                    Type = vehicle.TypeName,
                    RegistrationNumber =
                        NormalizeReg(vehicle.RegistrationNumber),
                    ArrivalTime = vehicle.ArrivalTime,
                    SpotNumber = spot.SpotNumber
                });
            }
        }

        return records;
    }

    public void LoadFromRecords(
        IEnumerable<ParkedVehicleRecord> records,
        out List<string> warnings)
    {
        warnings = new List<string>();

        // Loading replaces the garage's current contents.
        foreach (var spot in Spots)
        {
            spot.Vehicles.Clear();
        }

        foreach (var record in records)
        {
            if (record.SpotNumber < 1 ||
                record.SpotNumber > Spots.Count)
            {
                warnings.Add(
                    $"Skipped '{record.RegistrationNumber}': " +
                    $"spot {record.SpotNumber} is out of range.");
                continue;
            }

            Vehicle? vehicle = record.Type.Trim().ToUpperInvariant() switch
            {
                "CAR" => new Car(),
                "MC" => new MC(),
                _ => null
            };

            if (vehicle == null)
            {
                warnings.Add(
                    $"Skipped '{record.RegistrationNumber}': " +
                    $"unknown vehicle type '{record.Type}'.");
                continue;
            }

            vehicle.RegistrationNumber =
                NormalizeReg(record.RegistrationNumber);
            vehicle.ArrivalTime = record.ArrivalTime;

            if (RegistrationExists(vehicle.RegistrationNumber))
            {
                warnings.Add(
                    $"Skipped '{record.RegistrationNumber}': " +
                    "duplicate registration number.");
                continue;
            }

            int capacity;

            try
            {
                capacity = _config.GetVehiclePerSpot(vehicle.TypeName);
            }
            catch (ArgumentException)
            {
                warnings.Add(
                    $"Skipped '{record.RegistrationNumber}': " +
                    $"vehicle type '{vehicle.TypeName}' is not configured.");
                continue;
            }

            if (capacity <= 0)
            {
                warnings.Add(
                    $"Skipped '{record.RegistrationNumber}': " +
                    $"vehicle type '{vehicle.TypeName}' has invalid capacity.");
                continue;
            }

            var spot = Spots[record.SpotNumber - 1];

            if (!spot.CanPark(vehicle, capacity))
            {
                warnings.Add(
                    $"Skipped '{record.RegistrationNumber}': " +
                    $"spot {record.SpotNumber} is full or contains " +
                    "a different vehicle type.");
                continue;
            }

            spot.Park(vehicle);
        }
    }
}
