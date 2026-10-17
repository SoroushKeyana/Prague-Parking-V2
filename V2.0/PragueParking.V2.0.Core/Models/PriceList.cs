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
            if (!PricePerHour.TryGetValue(v.TypeName, out var rate))
            {
                throw new InvalidOperationException(
                    $"No price defined for {v.TypeName}.");
            }

            var parked = departure - v.ArrivalTime;

            if (parked.TotalMinutes < FreeMinutes)
            {
                return 0;
            }

            var startedHours = (decimal)Math.Ceiling(parked.TotalHours);

            return startedHours * rate;
        }

    }
}
