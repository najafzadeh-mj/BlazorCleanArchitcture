﻿using Application.Contracts;
using Application.DTOs.Request.Vehicles;
using Application.DTOs.Response;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController(IVehicle vehicle) : ControllerBase
    {
       [HttpPost("add-vehicle")]
       public async Task<ActionResult<GeneralResponse>> Create(CreateVehicleRequestDTO model)
            => Ok(await vehicle.AddVehicle(model));

        [HttpPost("add-vehicle-brand")]
        public async Task<ActionResult<GeneralResponse>> CreateBrand(CreateVehicleBrandRequestDTO model)
            => Ok(await vehicle.AddVehicleBrand(model));

        [HttpPost("add-vehicle-owner")]
        public async Task<ActionResult<GeneralResponse>> CreateOwner(CreateVehicleOwnerRequestDTO model)
            => Ok(await vehicle.AddVehicleOwner(model));

        #region get single
        [HttpGet("get-vehicle/{id}")]
        public async Task<ActionResult<GeneralResponse>> Get(int id)
            => Ok(await vehicle.GetVehicle(id));

        [HttpGet("get-vehicle-brand/{id}")]
        public async Task<ActionResult<GeneralResponse>> GetBrand(int id)
            => Ok(await vehicle.GetVehicleBrand(id));

        [HttpGet("get-vehicle-owner/{id}")]
        public async Task<ActionResult<GeneralResponse>> GetOwner(int id)
            => Ok(await vehicle.GetVehicleOwner(id));
        #endregion

        #region get list
        [HttpGet("get-vehicles")]
        public async Task<ActionResult<GeneralResponse>> Gets()
            => Ok(await vehicle.GetVehicles());

        [HttpGet("get-vehicle-brands")]
        public async Task<ActionResult<GeneralResponse>> GetBrands()
            => Ok(await vehicle.GetVehicleBrands());

        [HttpGet("get-vehicle-owners")]
        public async Task<ActionResult<GeneralResponse>> GetOwners()
            => Ok(await vehicle.GetVehicleOwners());
        #endregion

        #region update
        [HttpPut("update-vehicle")]
        public async Task<ActionResult<GeneralResponse>> Update(UpdateVehicleRequestDTO model)
            => Ok(await vehicle.UpdateVehicle(model));

        [HttpPut("update-vehicle-brand")]
        public async Task<ActionResult<GeneralResponse>> UpdateBrands(UpdateVehicleBrandRequestDTO model)
            => Ok(await vehicle.UpdateVehicleBrand(model));

        [HttpPut("update-vehicle-owner")]
        public async Task<ActionResult<GeneralResponse>> UpdateOwners(UpdateVehicleOwnerRequestDTO model)
            => Ok(await vehicle.UpdateVehicleOwner(model));
        #endregion

        #region delete
        [HttpDelete("delete-vehicle/{id}")]
        public async Task<ActionResult<GeneralResponse>> Delete(int id)
            => Ok(await vehicle.DeleteVehicle(id));

        [HttpDelete("delete-vehicle-brand/{id}")]
        public async Task<ActionResult<GeneralResponse>> DeleteBrand(int id)
            => Ok(await vehicle.DeleteVehicleBrand(id));

        [HttpDelete("delete-vehicle-owner/{id}")]
        public async Task<ActionResult<GeneralResponse>> DeleteOwner(int id)
            => Ok(await vehicle.DeleteVehicleOwner(id));
        #endregion
    }
}
