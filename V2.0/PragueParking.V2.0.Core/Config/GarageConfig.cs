using System;
using System.Collections.Generic;
using System.Text;

namespace PragueParking.V2._0.Core.Config
{
    public class GarageConfig
    {
        public int NumberOfSpots { get; set; } = 100;
        public List<VehicleTypeConfig> VehicleTypes { get; set; } = new();

        public int GetVehiclePerSpot(string typeName)
        {
            var vehicleTypeConfig = VehicleTypes.Find(v => v.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
            if (vehicleTypeConfig == null)
            {
                throw new ArgumentException($"Vehicle type '{typeName}' is not configured.");
            }
            return vehicleTypeConfig.VehiclePerSpot;
        }
    }
}
