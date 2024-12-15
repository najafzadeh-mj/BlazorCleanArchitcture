using Application.Services;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;

namespace Application.Extensions
{
    public class CustomHttpHandler(LocalStorageService localStorageService
        , NavigationManager navigationManager
        ,IAccountService accountService):DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken cancellationToken)
        {
            try
            {
                bool loginUrl = request.RequestUri!.AbsoluteUri.Contains(Constant.LoginRoute);
                bool RegisterUrl = request.RequestUri!.AbsoluteUri.Contains(Constant.RegisterRoute);
                bool refreshTokenUrl = request.RequestUri!.AbsoluteUri.Contains(Constant.RefreshTokenRoute);
                bool AdminCreateUrl = request.RequestUri!.AbsoluteUri.Contains(Constant.CreateAdminRoute);
                if (loginUrl || RegisterUrl || refreshTokenUrl || AdminCreateUrl)
                    return await base.SendAsync(request, cancellationToken);

                var result = await base.SendAsync(request, cancellationToken);
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var tokenModel = await localStorageService.GetModelFromToken();
                    if (tokenModel == null) return result;

                    //call for refresh toke
                    var newJwtToken = await GetRefreshToken(tokenModel.Refresh!);
                    if(string.IsNullOrEmpty(newJwtToken)) return result;

                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(Constant.HttpClientHeaderScheme, newJwtToken);
                    return await base.SendAsync(request, cancellationToken);
                }
                return result;
            }
            catch { return null!; }
        }

        private async Task<string> GetRefreshToken(string refreshToken)
        {
            try
            {
                var response = await accountService.RefreshTokenAsync(new DTOs.Request.Account.RefreshTokenDTO() { Token = refreshToken });
                if (response == null || response.Token == null)
                {
                    await ClearBrowserStorage();
                    NavigateToLogin();
                    return null!;
                }
                await localStorageService.RemoveTokenFromBrowserLocalStorage();
                await localStorageService.SetBrowserLocalStorage(
                    new DTOs.Request.Account.LocalStorageDTO { Refresh = response!.RefreshToken, Token = response!.Token });
                return response.Token;
            }
            catch { return null!; }
        }
        private void NavigateToLogin() => navigationManager.NavigateTo(navigationManager.BaseUri, true, true);

        private async Task ClearBrowserStorage()=> await localStorageService.RemoveTokenFromBrowserLocalStorage();
    }
}
