using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Vehicles
{
    public class VehicleBaseDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }
        public int VehicleOwnerId { get; set; }
        public int VehicleBrandId { get; set; }
    }
}
