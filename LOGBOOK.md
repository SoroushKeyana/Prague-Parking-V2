# Logbook — Prague Parking V2

## 2026-10-15

* Read through the assignment specification.

## 2026-10-16

* Created the Solution and the projects and the solution structure.
* Made a plan on how to do the tastks, modulated the project.
* Installed `Spectre.Console`.
* Verified project references and NuGet package dependencies for all projects in the solution.
* Created necessary folders and files for the project.
* Created `Vehicle` class and implemented the properties, made it abstract becuase it will be inherited by other classes. For this specific version it will be inherited by `Car` and `Motorcycle` classes.
* Created `Car` class and implemented the properties, inherited from `Vehicle` class.
* Created `Motorcycle` class and implemented the properties, inherited from `Vehicle` class.
* Created `VehicleTypeConfig` and `GarageConfig` classes with configurable garage capacity and vehicle types. 
* Added `GetVehiclesPerSpot()` to retrieve the configured capacity for each vehicle type.