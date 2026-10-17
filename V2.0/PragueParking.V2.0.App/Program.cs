using System;
using System.IO;
using Spectre.Console;


string configPath = Path.Combine(
    AppContext.BaseDirectory, "Files", "config.json");

string pricesPath = Path.Combine(
    AppContext.BaseDirectory, "Files", "prices.txt");

string dataPath = Path.Combine(
    AppContext.BaseDirectory, "Files", "data.json");


Console.WriteLine("Hello, World! ");
AnsiConsole.MarkupLine("[red] Hello[/], [yellow bold slowblink]World[/]!");