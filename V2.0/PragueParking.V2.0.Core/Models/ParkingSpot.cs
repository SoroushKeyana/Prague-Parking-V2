using System;
using System.Collections.Generic;
using System.Text;

namespace PragueParking.V2._0.Core.Models
{
    public class ParkingSpot
    {
        public int SpotNumber { get; set; }
        public List<Vehicle> Vehicles { get; set; } = new ();
        public bool IsEmpty => Vehicles.Count == 0;

        public bool CanPark(Vehicle v, int vehiclesPerSpot)
        {
            if (IsEmpty)
            {
                return true;
            }
            return Vehicles[0].TypeName == v.TypeName && Vehicles.Count < vehiclesPerSpot;
        }

        public void Park(Vehicle v)
        {
            Vehicles.Add(v);
        }

        public bool Remove(Vehicle v)
        {
            return Vehicles.Remove(v);
        }

        public bool IsFull(int vehiclesPerSpot)
        {
            if (IsEmpty)
            {
                return false;
            }

            return Vehicles.Count >= vehiclesPerSpot;
        }
    }
}
