
using PragueParking.V2._0.Core.Config;
using PragueParking.V2._0.Core.Models;
using PragueParking.V2_0.Core.Models;
using PragueParking.V2_0.Data;
using Spectre.Console;
using System.Globalization;

namespace PragueParking.V2_0.App.UI;

public class ParkingApp
{
    private readonly GarageConfig _config;
    private PriceList _priceList;
    private readonly ParkingGarage _garage;

    private readonly string _configPath;
    private readonly string _pricesPath;
    private readonly string _dataPath;

    public ParkingApp()
    {
        string filesPath = Path.Combine(
            AppContext.BaseDirectory, "Files");

        _configPath = Path.Combine(filesPath, "config.json");
        _pricesPath = Path.Combine(filesPath, "prices.txt");
        _dataPath = Path.Combine(filesPath, "data.json");

        _config = MyFiles.ReadConfig(_configPath);
        _priceList = MyFiles.ReadPriceList(_pricesPath);

        _garage = new ParkingGarage(_config);

        var records = MyFiles.ReadParkedVehicles(_dataPath);
        _garage.LoadFromRecords(records, out var warnings);

        foreach (string warning in warnings)
        {
            AnsiConsole.MarkupLine(
                $"[yellow]Warning:[/] {Markup.Escape(warning)}");
        }
    }

    public void Run()
    {
        while (true)
        {
            Header.Render();

            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[cyan]What would you like to do?[/]")
                    .PageSize(10)
                    .AddChoices(
                        "Park vehicle",
                        "Check out vehicle",
                        "Move vehicle",
                        "Find vehicle",
                        "Show garage map",
                        "Reload price list",
                        "Exit"));

            if (choice == "Exit")
            {
                break;
            }

            AnsiConsole.Clear();
            Header.Render();

            Action action = choice switch
            {
                "Park vehicle" => ParkVehicle,
                "Check out vehicle" => CheckOutVehicle,
                "Move vehicle" => MoveVehicle,
                "Find vehicle" => FindVehicle,
                "Show garage map" => ShowGarageMap,
                "Reload price list" => ReloadPriceList,
                _ => () => AnsiConsole.MarkupLine(
                    "[red]Unknown menu option.[/]")
            };

            ExecuteSafely(action);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine(
                "[grey]Press any key to continue...[/]");

            Console.ReadKey(intercept: true);
            AnsiConsole.Clear();
        }
    }

    private static void ExecuteSafely(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine(
                $"[red]Error:[/] {Markup.Escape(ex.Message)}");
        }
    }

    private void SaveGarage()
    {
        MyFiles.SaveParkedVehicles(
            _dataPath, _garage.ToRecords());
    }

    private string AskForRegistration()
    {
        string registration = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter registration number:")
                .Validate(input =>
                {
                    string reg = input.Trim();

                    if (reg.Length == 0)
                    {
                        return ValidationResult.Error(
                            "[red]Registration cannot be empty.[/]");
                    }

                    if (reg.Length < 4 || reg.Length > 10)
                    {
                        return ValidationResult.Error(
                            "[red]Registration must be 4–10 characters.[/]");
                    }

                    if (!reg.All(c =>
                        char.IsLetterOrDigit(c) || c == '-'))
                    {
                        return ValidationResult.Error(
                            "[red]Use only letters, digits and hyphens.[/]");
                    }

                    if (_garage.RegistrationExists(reg))
                    {
                        return ValidationResult.Error(
                            "[red]That registration is already parked.[/]");
                    }

                    return ValidationResult.Success();
                }));

        return ParkingGarage.NormalizeReg(registration);
    }

    private string AskForExistingRegistration()
    {
        string registration = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter registration number:")
                .Validate(input =>
                    string.IsNullOrWhiteSpace(input)
                        ? ValidationResult.Error(
                            "[red]Registration cannot be empty.[/]")
                        : ValidationResult.Success()));

        return ParkingGarage.NormalizeReg(registration);
    }

    private void ParkVehicle()
    {
        string typeName = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select vehicle type:")
                .AddChoices(_config.VehicleTypes.Select(t => t.Name)));

        string registration = AskForRegistration();

        Vehicle vehicle = typeName.Trim().ToUpperInvariant() switch
        {
            "CAR" => new Car(),
            "MC" => new MC(),
            _ => throw new InvalidOperationException(
                $"Vehicle type '{typeName}' is not supported by the application.")
        };

        vehicle.RegistrationNumber = registration;
        vehicle.ArrivalTime = DateTime.Now;

        if (_garage.ParkVehicle(vehicle, out int spotNumber))
        {
            SaveGarage();

            AnsiConsole.MarkupLine(
                $"[green]Vehicle parked at spot {spotNumber}.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine(
                "[red]No room for this vehicle type.[/]");
        }
    }

    private void CheckOutVehicle()
    {
        string registration = AskForExistingRegistration();
        var result = _garage.FindVehicle(registration);

        if (result == null)
        {
            AnsiConsole.MarkupLine(
                "[red]Vehicle not found.[/]");
            return;
        }

        var (spot, vehicle) = result.Value;
        DateTime departure = DateTime.Now;
        TimeSpan duration = departure - vehicle.ArrivalTime;
        decimal price = _priceList.CalculatePrice(vehicle, departure);

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("Detail")
            .AddColumn("Value");

        table.AddRow("Spot", spot.SpotNumber.ToString());
        table.AddRow("Type", vehicle.TypeName);
        table.AddRow("Registration", vehicle.RegistrationNumber);
        table.AddRow("Arrival", vehicle.ArrivalTime.ToString("yyyy-MM-dd HH:mm:ss"));
        table.AddRow("Departure", departure.ToString("yyyy-MM-dd HH:mm:ss"));
        table.AddRow("Duration", FormatDuration(duration));
        table.AddRow("Price", $"{price.ToString("0.00", CultureInfo.InvariantCulture)} CZK");

        AnsiConsole.Write(new Panel(table).Header("Checkout details"));

        if (!AnsiConsole.Confirm("Confirm checkout?"))
        {
            AnsiConsole.MarkupLine("[yellow]Checkout cancelled.[/]");
            return;
        }

        var removed = _garage.RetrieveVehicle(registration);

        if (removed == null)
        {
            AnsiConsole.MarkupLine(
                "[red]Could not retrieve the vehicle.[/]");
            return;
        }

        SaveGarage();

        AnsiConsole.MarkupLine(
            $"[green]Vehicle checked out. Amount due: " +
            $"{price.ToString("0.00", CultureInfo.InvariantCulture)} CZK.[/]");
    }

    private void MoveVehicle()
    {
        string registration = AskForExistingRegistration();

        int targetSpot = AnsiConsole.Prompt(
            new TextPrompt<int>(
                $"Enter target spot (1–{_garage.Spots.Count}):")
                .Validate(number =>
                    number >= 1 && number <= _garage.Spots.Count
                        ? ValidationResult.Success()
                        : ValidationResult.Error(
                            $"[red]Enter a spot between 1 and {_garage.Spots.Count}.[/]")));

        if (_garage.MoveVehicle(
            registration, targetSpot, out string error))
        {
            SaveGarage();

            AnsiConsole.MarkupLine(
                $"[green]Vehicle moved to spot {targetSpot}.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine(
                $"[red]{Markup.Escape(error)}[/]");
        }
    }

    private void FindVehicle()
    {
        string registration = AskForExistingRegistration();
        var result = _garage.FindVehicle(registration);

        if (result == null)
        {
            AnsiConsole.MarkupLine(
                "[red]Vehicle not found.[/]");
            return;
        }

        var (spot, vehicle) = result.Value;
        TimeSpan duration = DateTime.Now - vehicle.ArrivalTime;

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("Detail")
            .AddColumn("Value");

        table.AddRow("Spot", spot.SpotNumber.ToString());
        table.AddRow("Type", vehicle.TypeName);
        table.AddRow("Registration", vehicle.RegistrationNumber);
        table.AddRow("Arrival", vehicle.ArrivalTime.ToString("yyyy-MM-dd HH:mm:ss"));
        table.AddRow("Parked duration", FormatDuration(duration));

        AnsiConsole.Write(table);
    }

    private void ShowGarageMap()
    {
        MapRenderer.Render(_garage, _config);
    }

    private void ReloadPriceList()
    {
        // Assign only after successful parsing, so errors preserve old prices.
        var updatedPrices = MyFiles.ReadPriceList(_pricesPath);
        _priceList = updatedPrices;

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("Vehicle type")
            .AddColumn("Price per started hour");

        foreach (var price in _priceList.PricePerHour)
        {
            table.AddRow(
                price.Key,
                $"{price.Value.ToString("0.00", CultureInfo.InvariantCulture)} CZK");
        }

        table.AddRow("Free minutes", _priceList.FreeMinutes.ToString());

        AnsiConsole.MarkupLine(
            "[green]Price list reloaded successfully.[/]");

        AnsiConsole.Write(table);
    }

    private static string FormatDuration(TimeSpan duration)
    {
        int hours = (int)duration.TotalHours;
        int minutes = duration.Minutes;

        return $"{hours}h {minutes:D2}min";
    }
}
