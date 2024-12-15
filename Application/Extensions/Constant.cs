﻿using System;
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

        public static class Role
        {
            public const string Admin = "Admin";
            public const string User = "User";
        }
    }
}
