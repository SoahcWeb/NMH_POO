using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Microsoft.JSInterop;

namespace NMH.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _js;
        private readonly HttpClient _http;

        public CustomAuthStateProvider(IJSRuntime js, HttpClient http)
        {
            _js = js;
            _http = http;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "jwtToken");
            ClaimsIdentity identity;

            if (!string.IsNullOrEmpty(token))
            {
                // Ajouter le token à l'HttpClient pour les appels API
                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Lire le nom d'utilisateur depuis le token (payload)
                var jwtPayload = ParseJwt(token);
                var username = jwtPayload.GetValueOrDefault("unique_name") ?? "Utilisateur";

                identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
                }, "jwt");
            }
            else
            {
                identity = new ClaimsIdentity(); // anonyme
            }

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public void NotifyUserAuthentication(string username)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }, "jwt");

            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
        }

        private Dictionary<string, string> ParseJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            var bytes = Convert.FromBase64String(payload);
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
        }
    }
}