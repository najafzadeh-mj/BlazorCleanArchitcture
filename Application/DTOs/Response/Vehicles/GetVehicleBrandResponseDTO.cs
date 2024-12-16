using Application.DTOs.Request.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Response.Vehicles
{
    public class GetVehicleBrandResponseDTO:CreateVehicleBrandRequestDTO
    {
        public int Id { get; set; }
        public virtual ICollection<GetVehicleResponseDTO> Vehicles { get; set; } = null;
    }
}
