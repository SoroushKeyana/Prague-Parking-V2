
using PragueParking.V2._0.Core.Config;
using PragueParking.V2_0.Core.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PragueParking.V2_0.App.UI;

public static class MapRenderer
{
    public static void Render(ParkingGarage garage, GarageConfig config)
    {
        var table = new Table()
        .Border(TableBorder.Rounded);

        for (int i = 0; i < 10; i++)
        {
            table.AddColumn(new TableColumn("Spot").Centered());
        }

        for (int start = 0; start < garage.Spots.Count; start += 10)
        {
            var cells = new List<IRenderable>();

            for (int offset = 0; offset < 10; offset++)
            {
                int index = start + offset;

                if (index >= garage.Spots.Count)
                {
                    cells.Add(new Markup(" "));
                    continue;
                }

                var spot = garage.Spots[index];
                string background;

                if (spot.IsEmpty)
                {
                    background = "green";
                }
                else
                {
                    string typeName = spot.Vehicles[0].TypeName;

                    var typeConfig = config.VehicleTypes.FirstOrDefault(
                        t => t.Name.Equals(
                            typeName,
                            StringComparison.OrdinalIgnoreCase));

                    int capacity = typeConfig?.VehiclesPerSpot ?? 1;

                    background = spot.Vehicles.Count >= capacity
                        ? "red"
                        : "yellow";
                }

                cells.Add(new Markup(
                    $"[black on {background}] {spot.SpotNumber,3} [/ ]"
                        .Replace("[/ ]", "[/]")));
            }

            table.AddRow(cells.ToArray());
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine(
            "[green]  GREEN  [/]: Empty    " +
            "[yellow]  YELLOW  [/]: Partially filled    " +
            "[red]  RED  [/]: Full");

        AnsiConsole.WriteLine();

        AnsiConsole.Write(
            new FigletText(
                $"Free: {garage.FreeSpotCount} / {garage.Spots.Count}")
                .Color(Color.Green)
                .Centered());

        AnsiConsole.MarkupLine(
            $"[yellow]Partially filled:[/] {garage.PartialSpotCount}    " +
            $"[red]Full:[/] {garage.FullSpotCount}");
    }
}
