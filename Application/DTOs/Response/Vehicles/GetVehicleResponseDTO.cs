using Application.DTOs.Request.Vehicles;

namespace Application.DTOs.Response.Vehicles
{
    public class GetVehicleResponseDTO : CreateVehicleRequestDTO
    {
        public int Id { get; set; }

        public  GetVehicleBrandResponseDTO? VehicleBrand { get; set; }
        public  GetVehicleOwnerResponseDTO? VehicleOwner { get; set; }
    }
}
