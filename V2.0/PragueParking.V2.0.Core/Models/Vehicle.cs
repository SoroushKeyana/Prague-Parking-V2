using System;
using System.Collections.Generic;
using System.Text;

namespace PragueParking.V2._0.Core.Models
{
    public abstract class Vehicle
    {
        public string RegistrationNumber { get; set; } = "";
        public DateTime ArrivalTime { get; set; }
        public abstract string TypeName { get; }
    }
}
