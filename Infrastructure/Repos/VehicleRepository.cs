using Application.Contracts;
using Application.DTOs.Request.Vehicles;
using Application.DTOs.Response;
using Application.DTOs.Response.Vehicles;
using Domain.Entity.VehicleEntity;
using Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos
{
    public class VehicleRepository(AppDbContext context) : IVehicle
    {

        private async Task<Vehicle?> FindVehicleByName(string name)
            => await context.Vehicles.FirstOrDefaultAsync(m => m.Name!.ToLower() == name.ToLower());
        private async Task<VehicleBrand?> FindVehicleBrandByName(string name)
            => await context.VehicleBrands.FirstOrDefaultAsync(m => m.Name!.ToLower() == name.ToLower());
        private async Task<VehicleOwner?> FindVehicleOwnerByName(string name)
            => await context.VehicleOwners.FirstOrDefaultAsync(m => m.Name!.ToLower() == name.ToLower());

        private async Task<Vehicle?> FindVehicleById(int id)
            => await context.Vehicles
            .Include(m=> m.VehicleBrand)
            .Include(m=> m.VehicleOwner)
            .FirstOrDefaultAsync(m => m.Id == id);
        private async Task<VehicleBrand?> FindVehicleBrandById(int id)
            => await context.VehicleBrands
            .FirstOrDefaultAsync(m => m.Id == id);
        private async Task<VehicleOwner?> FindVehicleOwnerById(int id)
            => await context.VehicleOwners
            .FirstOrDefaultAsync(m => m.Id == id);

        private async Task SaveChangeAsync() => await context.SaveChangesAsync();

        private static GeneralResponse NullResponse(string message) => new(false, message);
        private static GeneralResponse AlreadyExistResponse(string message) => new(false, message);
        private static GeneralResponse OperationSuccessResponse(string message) => new(true, message);


        public async Task<GeneralResponse> AddVehicle(CreateVehicleRequestDTO model)
        {
            if (await FindVehicleByName(model.Name) is not null) 
                return AlreadyExistResponse("Vehicle already exist!");
            context.Vehicles.Add(model.Adapt(new Vehicle()));
            await SaveChangeAsync();
            return OperationSuccessResponse("Vehicle data saved!");
        }

        public async Task<GeneralResponse> AddVehicleBrand(CreateVehicleBrandRequestDTO model)
        {
            if (await FindVehicleBrandByName(model.Name) is not null) 
                return AlreadyExistResponse("Vehicle already exist!");
            context.VehicleBrands.Add(model.Adapt(new VehicleBrand()));
            await SaveChangeAsync();
            return OperationSuccessResponse("Vehicle data saved!");
        }

        public async Task<GeneralResponse> AddVehicleOwner(CreateVehicleOwnerRequestDTO model)
        {
            if (await FindVehicleOwnerByName(model.Name) is not null)
                return AlreadyExistResponse("Vehicle already exist!");
            context.VehicleOwners.Add(model.Adapt(new VehicleOwner()));
            await SaveChangeAsync();
            return OperationSuccessResponse("Vehicle data saved!");
        }

        public async Task<GeneralResponse> DeleteVehicle(int id)
        {
            if (await FindVehicleById(id) is null)
                return AlreadyExistResponse("Vehicle not found!");
            context.Vehicles.Remove(await FindVehicleById(id));
            await SaveChangeAsync();
            return OperationSuccessResponse("Vehicle deleted!");
        }

        public async Task<GeneralResponse> DeleteVehicleBrand(int id)
        {
            if (await FindVehicleBrandById(id) is null)
                return AlreadyExistResponse("Vehicle not found!");
            context.VehicleBrands.Remove(await FindVehicleBrandById(id));
            await SaveChangeAsync();
            return OperationSuccessResponse("Vehicle Brand deleted!");
        }

        public async Task<GeneralResponse> DeleteVehicleOwner(int id)
        {
            if (await FindVehicleOwnerById(id) is null)
                return AlreadyExistResponse("Vehicle not found!");
            context.VehicleOwners.Remove(await FindVehicleOwnerById(id));
            await SaveChangeAsync();
            return OperationSuccessResponse("Vehicle Owner deleted!");
        }

        public async Task<GetVehicleResponseDTO> GetVehicle(int id)
        => (await FindVehicleById(id)).Adapt(new GetVehicleResponseDTO());

        public async Task<GetVehicleBrandResponseDTO> GetVehicleBrand(int id)
        => (await FindVehicleBrandById(id)).Adapt(new GetVehicleBrandResponseDTO());

        public async Task<GetVehicleOwnerResponseDTO> GetVehicleOwner(int id)
        => (await FindVehicleOwnerById(id)).Adapt(new GetVehicleOwnerResponseDTO());


        public async Task<IEnumerable<GetVehicleBrandResponseDTO>> GetVehicleBrands()
        => (await context.VehicleBrands.ToListAsync()).Adapt(new List<GetVehicleBrandResponseDTO>());

        public async Task<IEnumerable<GetVehicleOwnerResponseDTO>> GetVehicleOwners()
         => (await context.VehicleOwners.ToListAsync()).Adapt(new List<GetVehicleOwnerResponseDTO>());


        public async Task<IEnumerable<GetVehicleResponseDTO>> GetVehicles()
        {
            var data = (await context.Vehicles
                .Include(m => m.VehicleBrand)
                .Include(m => m.VehicleOwner)
                .ToListAsync());

            return data.Select(m => new GetVehicleResponseDTO
            {
                Id = m.Id,
                Name = m.Name!,
                Description = m.Description!,
                VehicleBrandId = m.VehicleBrandId,
                VehicleOwnerId = m.VehicleOwnerId,
                Price = m.Price,
                VehicleBrand = new GetVehicleBrandResponseDTO
                {
                    Id = m.VehicleBrand!.Id,
                    Location = m.VehicleBrand!.Location!,
                    Name = m.VehicleBrand!.Name!
                },
                VehicleOwner = new GetVehicleOwnerResponseDTO
                {
                    Id = m.VehicleOwner.Id,
                    Address = m.VehicleOwner.Address,
                    Name = m.VehicleOwner.Name
                }
            }).ToList();
        }

        public async Task<GeneralResponse> UpdateVehicle(UpdateVehicleRequestDTO model)
        {
            if (await FindVehicleById(model.Id) is null)
                return AlreadyExistResponse("Vehicle not found!");
            context.Entry(await FindVehicleById(model.Id)).State = EntityState.Detached;
            context.Vehicles.Update(model.Adapt(new Vehicle()));
            await SaveChangeAsync();
            return OperationSuccessResponse("Vehicle Owner updated!");
        }

        public async  Task<GeneralResponse> UpdateVehicleBrand(UpdateVehicleBrandRequestDTO model)
        {
            if (await FindVehicleBrandById(model.Id) is null)
                return AlreadyExistResponse("Vehicle not found!");
            context.Entry(await FindVehicleBrandById(model.Id)).State = EntityState.Detached;
            context.VehicleBrands.Update(model.Adapt(new VehicleBrand()));
            await SaveChangeAsync();
            return OperationSuccessResponse("Vehicle Brand Owner updated!");
        }

        public async Task<GeneralResponse> UpdateVehicleOwner(UpdateVehicleOwnerRequestDTO model)
        {
            if (await FindVehicleOwnerById(model.Id) is null)
                return AlreadyExistResponse("Vehicle not found!");
            context.Entry(await FindVehicleOwnerById(model.Id)).State = EntityState.Detached;
            context.VehicleOwners.Update(model.Adapt(new VehicleOwner()));
            await SaveChangeAsync();
            return OperationSuccessResponse("Vehicle Brand Owner updated!");
        }
    }
}
