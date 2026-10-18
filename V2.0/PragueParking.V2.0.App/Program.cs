using PragueParking.V2_0.App.UI;
using Spectre.Console;
using System;
using System.IO;


string configPath = Path.Combine(
    AppContext.BaseDirectory, "Files", "config.json");

string pricesPath = Path.Combine(
    AppContext.BaseDirectory, "Files", "prices.txt");

string dataPath = Path.Combine(
    AppContext.BaseDirectory, "Files", "data.json");


try
{
    new ParkingApp().Run();
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine(
        $"[red]Application error:[/] {Markup.Escape(ex.Message)}");
}
