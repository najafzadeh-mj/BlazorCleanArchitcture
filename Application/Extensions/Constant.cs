using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Extensions
{
    public static class Constant
    {
        public const string BrowserStorageKey = "x-key";
        public const string HttpClientName = "WebUIClient";
        public const string HttpClientHeaderScheme = "Bearer";
       
        public const string RegisterRoute = "api/account/identity/create";
        public const string LoginRoute = "api/account/identity/login";
        public const string RefreshTokenRoute = "api/account/identity/refresh-token";
        public const string GetRolesRoute = "api/account/identity/role/list";
        public const string CreateRolesRoute = "api/account/identity/role/create";
        public const string CreateAdminRoute = "setting";
        public const string AuthenticationType = "JwtAuth";
        public const string GetUserWithRolesRoute = "api/account/identity/users-with-roles";
        public const string ChangeUserRoleRoute = "api/account/identity/change-role";

        public const string AddVehicleRoute = "api/Vehicle/add-vehicle";
        public const string AddVehicleBrandRoute = "api/Vehicle/add-vehicle-brand";
        public const string AddVehicleOwnerRoute = "api/Vehicle/add-vehicle-owner";

        public const string GetVehicleRoute = "api/Vehicle/get-vehicle";
        public const string GetVehicleBrandRoute = "api/Vehicle/get-vehicle-brand";
        public const string GetVehicleOwnerRoute = "api/Vehicle/get-vehicle-owner";

        public const string GetVehiclesRoute = "api/Vehicle/get-vehicles";
        public const string GetVehicleBrandsRoute = "api/Vehicle/get-vehicle-brands";
        public const string GetVehicleOwnersRoute = "api/Vehicle/get-vehicle-owners";

        public const string DeleteVehicleRoute = "api/Vehicle/delete-vehicle";
        public const string DeleteVehicleBrandRoute = "api/Vehicle/delete-vehicle-brand";
        public const string DeleteVehicleOwnerRoute = "api/Vehicle/delete-vehicle-owner";

        public const string UpdateVehicleRoute = "api/Vehicle/update-vehicle";
        public const string UpdateVehicleBrandRoute = "api/Vehicle/update-vehicle-brand";
        public const string UpdateVehicleOwnerRoute = "api/Vehicle/update-vehicle-owner";

        public static class Role
        {
            public const string Admin = "Admin";
            public const string User = "User";
        }
    }
}
