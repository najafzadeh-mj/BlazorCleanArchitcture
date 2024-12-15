using Application.DTOs.Request.Account;
using NetcodeHub.Packages.Extensions.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Extensions
{
    public class LocalStorageService(ILocalStorageService localStorageService)
    {
        private async Task<string> GetBrowserLocalStorage()
        {
            var tokenModel = await localStorageService.GetEncryptedItemAsStringAsync(Constant.BrowserStorageKey);
            return tokenModel!;
        }
        public async Task<LocalStorageDTO> GetModelFromToken()
        {
            try
            {
                string token = await GetBrowserLocalStorage();
                if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
                    return new LocalStorageDTO();
                return DeserializeJsonString<LocalStorageDTO>(token);
            }
            catch
            {
                return new LocalStorageDTO();
            }
        }

        public async Task SetBrowserLocalStorage(LocalStorageDTO localStorageDTO)
        {
            try
            {
                string token = SerializeObj(localStorageDTO);
                await localStorageService.SaveAsEncryptedStringAsync(Constant.BrowserStorageKey, token);
            }
            catch { }
        }

        public async Task RemoveTokenFromBrowserLocalStorage() 
            => await localStorageService.DeleteItemAsync(Constant.BrowserStorageKey);

        private static string SerializeObj<T>(T modelObject)
        => JsonSerializer.Serialize(modelObject, JsonOption());

        private static T DeserializeJsonString<T>(string jsonString)
            => JsonSerializer.Deserialize<T>(jsonString, JsonOption())!;

        private static JsonSerializerOptions JsonOption()
        {
            return new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Skip
            };
        }

    }
}
