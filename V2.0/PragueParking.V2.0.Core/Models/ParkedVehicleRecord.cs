using System;
using System.Collections.Generic;
using System.Text;

namespace PragueParking.V2._0.Core.Models
{
    public class ParkedVehicleRecord
    {
        public string Type { get; set; } = "";
        public string RegistrationNumber { get; set; } = "";
        public DateTime ArrivalTime { get; set; }
        public int SpotNumber { get; set; }
    }
}
