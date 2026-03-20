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

            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var jwtPayload = ParseJwt(token);
            var username = jwtPayload.GetValueOrDefault("unique_name") ?? "Utilisateur";

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }, "jwt");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task NotifyUserAuthentication(string token)
        {
            await _js.InvokeVoidAsync("localStorage.setItem", "jwtToken", token);

            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var jwtPayload = ParseJwt(token);
            var username = jwtPayload.GetValueOrDefault("unique_name") ?? "Utilisateur";

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }, "jwt");

            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(user))
            );
        }

        public async Task NotifyUserLogout()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", "jwtToken");

            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(anonymous))
            );
        }

        private Dictionary<string, string> ParseJwt(string jwt)
{
    var payload = jwt.Split('.')[1];

    payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');

    var bytes = Convert.FromBase64String(payload);
    var json = System.Text.Encoding.UTF8.GetString(bytes);

    var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json)
               ?? new Dictionary<string, object>();

    return dict.ToDictionary(
        kvp => kvp.Key,
        kvp => kvp.Value?.ToString() ?? ""
    );
}
    }
}