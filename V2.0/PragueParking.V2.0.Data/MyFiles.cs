
using PragueParking.V2._0.Core.Config;
using PragueParking.V2._0.Core.Models;
using System.Globalization;
using System.Text.Json;

namespace PragueParking.V2_0.Data;

public static class MyFiles
{
    public static GarageConfig ReadConfig(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"Configuration file was not found: {path}", path);
        }

        string json = File.ReadAllText(path);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        GarageConfig config =
            JsonSerializer.Deserialize<GarageConfig>(json, options)
            ?? throw new InvalidDataException(
                "The configuration file is empty or invalid.");

        if (config.NumberOfSpots <= 0)
        {
            throw new InvalidDataException(
                "NumberOfSpots must be greater than zero.");
        }

        if (config.VehicleTypes == null ||
            config.VehicleTypes.Count == 0)
        {
            throw new InvalidDataException(
                "At least one vehicle type must be configured.");
        }

        var typeNames = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var type in config.VehicleTypes)
        {
            if (string.IsNullOrWhiteSpace(type.Name))
            {
                throw new InvalidDataException(
                    "Vehicle type names cannot be empty.");
            }

            if (type.VehiclesPerSpot <= 0)
            {
                throw new InvalidDataException(
                    $"VehiclesPerSpot for '{type.Name}' must be greater than zero.");
            }

            if (!typeNames.Add(type.Name.Trim()))
            {
                throw new InvalidDataException(
                    $"Duplicate vehicle type: '{type.Name}'.");
            }

            type.Name = type.Name.Trim();
        }

        return config;
    }

    public static PriceList ReadPriceList(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"Price list file was not found: {path}", path);
        }

        var priceList = new PriceList
        {
            FreeMinutes = 10,
            PricePerHour = new Dictionary<string, decimal>(
                StringComparer.OrdinalIgnoreCase)
        };

        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            string originalLine = lines[i];
            string text = originalLine.Split('#')[0].Trim();

            if (string.IsNullOrEmpty(text))
            {
                continue;
            }

            string[] parts = text.Split('=');

            if (parts.Length != 2)
            {
                throw new FormatException(
                    $"Invalid format on line {i + 1}: '{originalLine}'. " +
                    "Expected Key = Value.");
            }

            string key = parts[0].Trim();
            string value = parts[1].Trim();

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new FormatException(
                    $"Missing key on line {i + 1}: '{originalLine}'.");
            }

            if (key.Equals(
                "FreeMinutes", StringComparison.OrdinalIgnoreCase))
            {
                if (!int.TryParse(
                    value,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out int freeMinutes))
                {
                    throw new FormatException(
                        $"Invalid FreeMinutes value on line {i + 1}: '{originalLine}'.");
                }

                if (freeMinutes < 0)
                {
                    throw new FormatException(
                        $"FreeMinutes cannot be negative on line {i + 1}.");
                }

                priceList.FreeMinutes = freeMinutes;
            }
            else
            {
                if (!decimal.TryParse(
                    value,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal price))
                {
                    throw new FormatException(
                        $"Invalid price on line {i + 1}: '{originalLine}'.");
                }

                if (price < 0)
                {
                    throw new FormatException(
                        $"Price cannot be negative on line {i + 1}.");
                }

                if (!priceList.PricePerHour.TryAdd(key, price))
                {
                    throw new FormatException(
                        $"Duplicate price for '{key}' on line {i + 1}.");
                }
            }
        }

        return priceList;
    }

    public static List<ParkedVehicleRecord> ReadParkedVehicles(
        string path)
    {
        if (!File.Exists(path))
        {
            return new List<ParkedVehicleRecord>();
        }

        string json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<List<ParkedVehicleRecord>>(json)
            ?? new List<ParkedVehicleRecord>();
    }

    public static void SaveParkedVehicles(
        string path,
        IEnumerable<ParkedVehicleRecord> records)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(records, options);
        string tempPath = path + ".tmp";

        try
        {
            File.WriteAllText(tempPath, json);
            File.Move(tempPath, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }
}
