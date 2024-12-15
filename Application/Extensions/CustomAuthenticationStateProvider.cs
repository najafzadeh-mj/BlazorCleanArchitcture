using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.DTOs.Request.Account;
using Application.DTOs.Response.Account;
using Microsoft.AspNetCore.Components.Authorization;

namespace Application.Extensions
{
    public class CustomAuthenticationStateProvider(LocalStorageService localStorageService):AuthenticationStateProvider
    {
        private readonly ClaimsPrincipal anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var tokenModel = await localStorageService.GetModelFromToken();
            if (string.IsNullOrEmpty(tokenModel.Token))
                return await Task.FromResult(new AuthenticationState(anonymous));

            var getuserclaims=DecryptToken(tokenModel.Token);
            if(getuserclaims != null) 
                return await Task.FromResult(new AuthenticationState(anonymous));

            var claimsPrincipal=SetClaimPrincipal(getuserclaims);
            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        public async Task UpdateAuthenticationState(LocalStorageDTO localStorageDTO)
        {
            var claimPrincipal = new ClaimsPrincipal();
            if(localStorageDTO.Token != null || localStorageDTO.Refresh != null)
            {
                await localStorageService.SetBrowserLocalStorage(localStorageDTO);
                var getUserClaims= DecryptToken(localStorageDTO.Token);
                claimPrincipal= SetClaimPrincipal(getUserClaims);
            }
            else
            {
                await localStorageService.RemoveTokenFromBrowserLocalStorage();
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimPrincipal)));
        }

        public static ClaimsPrincipal SetClaimPrincipal(UserClaimsDTO claim)
        {
            if (claim.Email is null)
                return new ClaimsPrincipal();
            return new ClaimsPrincipal(new ClaimsIdentity(

                [
                new (ClaimTypes.Name, claim.UserName!),
                new (ClaimTypes.Email, claim.Email!),
                new (ClaimTypes.Role, claim.Role!),
                new ("FullName", claim.FullName!),
                ], Constant.AuthenticationType));
        }

        private static UserClaimsDTO DecryptToken(string jwtToken)
        {
            try
            {
                if (string.IsNullOrEmpty(jwtToken))
                    return new UserClaimsDTO();

                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtToken);

                var name = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Name)!.Value;
                var email = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Email)!.Value;
                var role = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Role)!.Value;
                var fullName = token.Claims.FirstOrDefault(_ => _.Type == "FullName")!.Value;
                return new UserClaimsDTO(fullName, name, email, role);
            }
            catch
            {
                return null!;
            }
        }
    }
}
