using Library.API.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Library.API.Extensions
{
    public static class HttpClientExtension
    {
        public static async Task<(bool result, string token)> TryGetBearerTokenAsync(this HttpClient httpClient, LoginUser loginUser)
        {
            var userCredentialInfo = new StringContent(content: JsonConvert.SerializeObject(loginUser), encoding: Encoding.UTF8, mediaType: "application/json");
            var response = await httpClient.PostAsync("api/auth/token", userCredentialInfo);
            var tokenResult = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(tokenResult))
            {
                return (false, null);
            }
            else
            {
                var token = JsonConvert.DeserializeObject<TokenResult>(tokenResult);
                return (true, token.Token);
            }
        }
    }
}
