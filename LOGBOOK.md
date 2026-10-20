# Logbook — Prague Parking V2

## 2026-10-14

* Read through the assignment specification.

## 2026-10-15

* Created the Solution and the projects and the solution structure.
* Made a plan on how to do the tastks, modulated the project.
* Installed `Spectre.Console`.
* Verified project references and NuGet package dependencies for all projects in the solution.
* Created necessary folders and files for the project.

## 2026-10-16

* Created `Vehicle` class and implemented the properties, made it abstract becuase it will be inherited by other classes. For this specific version it will be inherited by `Car` and `Motorcycle` classes.
* Created `Car` class and implemented the properties, inherited from `Vehicle` class.
* Created `Motorcycle` class and implemented the properties, inherited from `Vehicle` class.
* Created `VehicleTypeConfig` and `GarageConfig` classes with configurable garage capacity and vehicle types. 
* Added `GetVehiclesPerSpot()` to retrieve the configured capacity for each vehicle type.
* A spot holds only one vehicle type at a time, and up to `VehiclesPerSpot` vehicles of that type. So 1 car, or up to 2 MCs, never a mix.

## 2026-10-17

* Created `ParkingSpot` with vehicle capacity validation, `Park` and `remove` methods, and `Empty` status checks. 
* Created `PriceList` with hourly rates per vehicle type, free parking minutes, and a placeholder for price calculation.
* Created `ParkedVehicleRecord` to hold vehicle information and arival time.
* Created `ParkedVehicleRecord` for JSON data persistence and implemented `ParkingGarage` with vehicle parking, retrieval, duplicate registration checks, vehicle movement, record conversion and loading, and garage occupancy counting.
* Created `config.json` file with default configuration for vehicle types and garage capacity.
* Created `prices.txt` file with default pricing for vehicle types.
* Created `data.json` file, left it empty for now.
* Created file paths for the configuration, price list, and parking data files using AppContext.BaseDirectory and Path.Combine to ensure the application can locate files independently of the project location.

## 2026-10-18

* Implemented `MyFiles` to read and validate garage configuration and parking prices, load saved vehicle records, and save parking data using a temporary file before replacing the existing file. Tested reading all three files from the App project.
* Added 16 test parking records with cars and motorcycles in different spots and arrival times covering the planned pricing scenarios. Tested `LoadFromRecords()` to verify that vehicles are restored to the correct spots.
* Implemented `CalculatePrice()` in `PriceList` using configurable hourly rates and free parking minutes. Parking under the freetime limit costs nothing, while billable time is rounded up to started hours. Used the supplied departure time to keep the calculation testable.
* Implemented the `Spectre.Console` user interface with a header and interactive main menu.
* Added vehicle parking with configurable vehicle types, registration number validation, and duplicate checks.
* Implemented vehicle checkout with parking duration calculation, price display, and checkout confirmation.
* Added vehicle search and movement between parking spots, including validation and error handling.
* Created a garage map with color coded occupancy status and a summary of free, partially filled, and full spots.
* Implemented price list reloading with error handling, preserving the previous prices if loading fails.
* Added automatic saving of parking data after successful parking, checkout, and vehicle movement operations.
* Moved application initialization to Program.cs.
* Set up paths for configuration, prices, and parking data files.
* Loaded the garage configuration and price list.
* Created the parking garage and restored saved vehicles.
* Displayed warnings for parking records that could not be restored.
* Updated `ParkingApp` to receive dependencies through its constructor.
* Added startup error handling using `Spectre.Console`.
* Removed unnecessary using statements to resolve compilation errors.
* Verified that the application builds and runs successfully.

## 2026-10-19

* Created PriceTests.cs using MSTest and Arrange / Act / Assert. 
* Tested free parking under 10 minutes and charging at exactly 10 minutes.
* Tested car parking for 90 minutes and motorcycle parking for 61 minutes.
* Created `GarageTests.cs` to test parking capacity and duplicate registrations.
* Tested vehicle retrieval and moving a vehicle to a full spot.
* Ran all eight tests in Test Explorer and all passed.
* Created a checklist for the final application testing.
* Tested everything manually in the application, including parking, checkout, moving vehicles, and reloading prices. Everything worked as expected, and the application was stable.
* Cloned the repository to a new folder and verified that the application runs successfully with the default configuration and pricing files.

## 2026-10-20


* 