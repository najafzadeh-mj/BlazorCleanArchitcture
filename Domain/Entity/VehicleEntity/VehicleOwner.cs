﻿using System.Text.Json.Serialization;

namespace Domain.Entity.VehicleEntity
{
    public class VehicleOwner: BaseClass
    {
        public string? Name { get; set; }
        public string? Address { get; set; }

        [JsonIgnore]
        public ICollection<Vehicle>? Vehicles { get; set; }
    }
}
