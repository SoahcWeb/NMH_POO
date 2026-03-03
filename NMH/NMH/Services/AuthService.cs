using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public AuthService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public async Task<string?> Login(string username, string password)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/login", new { UserName = username, Password = password });
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var token = json.GetProperty("token").GetString();

        if (token != null)
            await _js.InvokeVoidAsync("localStorage.setItem", "jwtToken", token);

        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return token;
    }

    public async Task Logout()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", "jwtToken");
        _http.DefaultRequestHeaders.Authorization = null;
    }

    public async Task Initialize()
    {
        var token = await _js.InvokeAsync<string>("localStorage.getItem", "jwtToken");
        if (!string.IsNullOrEmpty(token))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}