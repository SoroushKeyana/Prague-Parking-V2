using System;
using System.Collections.Generic;
using System.Text;

namespace PragueParking.V2._0.Core.Models
{
    public class PriceList
    {
        public Dictionary<string, decimal> PricePerHour { get; set; } = new();

        public int FreeMinutes { get; set; }

        public decimal CalculatePrice(Vehicle v, DateTime departure)
        {
            throw new NotImplementedException();
        }
    }
}
