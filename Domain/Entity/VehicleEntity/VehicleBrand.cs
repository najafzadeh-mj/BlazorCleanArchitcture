using System.Text.Json.Serialization;

namespace Domain.Entity.VehicleEntity
{
    public class VehicleBrand: BaseClass
    {
        public string? Name { get; set; }
        public string? Location { get; set; }
        
        [JsonIgnore]
        public ICollection<Vehicle>? Vehicles { get; set; }
    }
}
