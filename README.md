# Prague Parking V2.0

A C# console application using Spectre.Console to manage parking for cars and motorcycles. Configuration and parked vehicles are stored in JSON files, and prices are loaded from a text file.

## Requirements

- .NET 10 SDK
- Visual Studio with support for .NET 10 and `.slnx` solution files
- Internet access to restore NuGet packages on the first build

## How to Run

1. Clone the repository.
2. Open `Prague Parking V2.slnx`.
3. Set `PragueParking.V2.0.App` as the startup project.
4. Press **F5** or **Ctrl+F5**.

## Files

The initial files are in `V2.0/PragueParking.V2.0.App/Files/` and are copied to the build output.

During a normal Debug run, the application uses:
`V2.0/PragueParking.V2.0.App/bin/Debug/net10.0/Files/`

- `config.json`: garage capacity and vehicles per spot. Changes require a restart.
- `prices.txt`: hourly prices and free minutes. Use **Reload price list** after editing.
- `data.json`: sample parking data, updated automatically after parking, checkout and movement.

Edit the output copies when testing changes. Runtime saves do not update the original project files.

Development notes are in `LOGBOOK.md`.