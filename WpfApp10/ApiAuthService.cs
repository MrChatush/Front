using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp10
{
    public class ApiAuthService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7000/api/auth";

        public ApiAuthService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public async Task<string> RegisterAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("register",
                new { Username = username, Password = password });

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("login",
                new { Username = username, Password = password });

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<LoginResult>();
        }
    }
    public class LoginResult
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }

        public LoginResult(string token, int userId, string username)
        {
            Token = token;
            UserId = userId;
            Username = username;
        }
    }

}
