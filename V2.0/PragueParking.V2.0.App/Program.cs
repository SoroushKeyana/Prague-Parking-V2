using PragueParking.V2_0.Core.Models;
using PragueParking.V2_0.Data;
using PragueParking_V2_0.App.UI;
using Spectre.Console;

try
{
    // 1. Build file paths
    string filesPath = Path.Combine(
        AppContext.BaseDirectory, "Files");

    string configPath = Path.Combine(filesPath, "config.json");
    string pricesPath = Path.Combine(filesPath, "prices.txt");
    string dataPath = Path.Combine(filesPath, "data.json");

    // 2. Read configuration
    var config = MyFiles.ReadConfig(configPath);

    // 3. Read price list
    var priceList = MyFiles.ReadPriceList(pricesPath);

    // 4. Create the garage
    var garage = new ParkingGarage(config);

    // 5. Load parked vehicles
    var records = MyFiles.ReadParkedVehicles(dataPath);
    garage.LoadFromRecords(records, out var warnings);

    foreach (string warning in warnings)
    {
        AnsiConsole.MarkupLine(
            $"[yellow]Warning:[/] {Markup.Escape(warning)}");
    }

    // 6. Start the application
    var app = new ParkingApp(
        config,
        priceList,
        garage,
        pricesPath,
        dataPath);

    app.Run();
}
catch (Exception ex)
{
    AnsiConsole.Write(
        new Panel(
            $"[red]{Markup.Escape(ex.Message)}[/]")
        {
            Header = new PanelHeader("Startup Error"),
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Red),
            Padding = new Padding(1, 1)
        });
}