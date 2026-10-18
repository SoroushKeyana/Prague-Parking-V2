
using Spectre.Console;

namespace PragueParking.V2_0.App.UI;

public static class Header
{
    public static void Render()
    {
        AnsiConsole.Write(
            new FigletText("Prague Parking")
                .Centered()
                .Color(Color.Cyan));

        AnsiConsole.Write(
            new Rule("[yellow]Parking Management System[/]")
                .Centered());

        AnsiConsole.WriteLine();
    }
}
