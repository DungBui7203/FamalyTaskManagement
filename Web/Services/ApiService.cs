using System.Text;
using System.Text.Json;
using Web.Models;
using Web.Models.Task;

namespace Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]!);
        }

        public async Task<AuthResponse?> LoginAsync(LoginViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AuthResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return null;
        }

        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<TaskViewModel>?> GetTasksAsync(string? status = null, long? assigneeId = null)
        {
            var url = "/api/tasks";
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(status))
                queryParams.Add($"status={status}");
            if (assigneeId.HasValue)
                queryParams.Add($"assigneeId={assigneeId}");

            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<TaskViewModel>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return null;
        }

        public async Task<TaskViewModel?> CreateTaskAsync(CreateTaskViewModel task)
        {
            var json = JsonSerializer.Serialize(task);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/tasks", content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TaskViewModel>(responseJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            return null;
        }

        public async Task<List<UserViewModel>?> GetFamilyMembersAsync()
        {
            var response = await _httpClient.GetAsync("/api/users");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<UserViewModel>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return null;
        }

        public async Task<bool> UpdateTaskStatusAsync(long taskId, string status)
        {
            var dto = new { Status = status };
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"/api/tasks/{taskId}/status", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<TaskViewModel>?> GetMyTasksAsync(string? status = null)
        {
            var url = "/api/tasks/my-tasks";
            if (!string.IsNullOrEmpty(status))
                url += $"?status={status}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<TaskViewModel>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            return null;
        }

        public async Task<bool> VerifyTaskAsync(long taskId)
        {
            var response = await _httpClient.PostAsync($"/api/tasks/{taskId}/verify", null);
            return response.IsSuccessStatusCode;
        }

    }
}
