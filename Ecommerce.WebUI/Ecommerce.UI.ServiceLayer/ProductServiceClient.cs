using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Ecommerce.UI.ViewModel;

namespace Ecommerce.UI.ServiceLayer
{
    public class ProductServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductServiceClient> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string BaseUrl = "https://localhost:7156/api/Product";
        private const string AuthUrl = "https://localhost:7156/api/Auth";

        public ProductServiceClient(
            HttpClient httpClient, 
            ILogger<ProductServiceClient> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AttachBearerToken(HttpRequestMessage request)
        {
            var token = _httpContextAccessor?.HttpContext?.User?.FindFirst("JWT_TOKEN")?.Value
                ?? _httpContextAccessor?.HttpContext?.Session?.GetString("JWToken");

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // ================= AUTHENTICATION METHODS =================

        public async Task<AuthResultDto> LoginAsync(LoginViewModel loginModel)
        {
            try
            {
                _logger.LogInformation("Attempting API login for user {Username}", loginModel?.Username);
                var response = await _httpClient.PostAsJsonAsync($"{AuthUrl}/login", loginModel);
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var result = JsonSerializer.Deserialize<AuthResultDto>(json, options);
                return result ?? new AuthResultDto { IsSuccess = false, Message = "Unable to parse login response." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling login API");
                return new AuthResultDto { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterViewModel registerModel)
        {
            try
            {
                _logger.LogInformation("Attempting API registration for user {Username}", registerModel?.Username);
                var response = await _httpClient.PostAsJsonAsync($"{AuthUrl}/register", registerModel);
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var result = JsonSerializer.Deserialize<AuthResultDto>(json, options);
                return result ?? new AuthResultDto { IsSuccess = false, Message = "Unable to parse registration response." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling registration API");
                return new AuthResultDto { IsSuccess = false, Message = ex.Message };
            }
        }

        // ================= PRODUCT & METADATA METHODS =================

        public async Task<List<ProductDto>> GetProductsAsync(int? catId, int? subCatId, int? genId)
        {
            var query = string.Empty;
            if (catId.HasValue) query += $"&categoryId={catId}";
            if (subCatId.HasValue) query += $"&subCategoryId={subCatId}";
            if (genId.HasValue) query += $"&genderId={genId}";

            var url = $"{BaseUrl}/products?{query.TrimStart('&')}";
            _logger.LogInformation("Fetching products from API: {Url}", url);
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return await response.Content.ReadFromJsonAsync<List<ProductDto>>(options) ?? new List<ProductDto>();
            }
            _logger.LogWarning("Failed to fetch products. Status: {StatusCode}", response.StatusCode);
            return new List<ProductDto>();
        }

        public async Task<List<ProductCategory>> GetCategoriesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ProductCategory>>($"{BaseUrl}/categories") ?? new List<ProductCategory>();
        }

        public async Task<List<SubProductCategory>> GetSubCategoriesAsync(int categoryId)
        {
            return await _httpClient.GetFromJsonAsync<List<SubProductCategory>>($"{BaseUrl}/categories/{categoryId}/subcategories") ?? new List<SubProductCategory>();
        }

        public async Task<List<SubProductCategory>> GetAllSubCategoriesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<SubProductCategory>>($"{BaseUrl}/subcategories") ?? new List<SubProductCategory>();
        }

        public async Task<List<Gender>> GetGendersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Gender>>($"{BaseUrl}/genders") ?? new List<Gender>();
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<ProductDto>(jsonString, options);
            }
            return null;
        }

        public async Task<bool> CreateProductAsync(ProductDto product)
        {
            _logger.LogInformation("Creating product via API: {ProductName}", product.ProductName);
            using var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl)
            {
                Content = JsonContent.Create(product)
            };
            AttachBearerToken(request);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("API Error creating product: {StatusCode} - {Error}", response.StatusCode, error);
                throw new HttpRequestException($"API Error {response.StatusCode}: {error}");
            }
            return true;
        }

        public async Task<bool> UpdateProductAsync(ProductDto product)
        {
            _logger.LogInformation("Updating product via API: ID {ProductId}", product.Id);
            using var request = new HttpRequestMessage(HttpMethod.Put, BaseUrl)
            {
                Content = JsonContent.Create(product)
            };
            AttachBearerToken(request);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("API Error updating product: {StatusCode} - {Error}", response.StatusCode, error);
                throw new HttpRequestException($"API Error {response.StatusCode}: {error}");
            }
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            _logger.LogInformation("Deleting product via API: ID {ProductId}", id);
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"{BaseUrl}/{id}");
            AttachBearerToken(request);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("API Error deleting product: {StatusCode} - {Error}", response.StatusCode, error);
                throw new HttpRequestException($"API Error {response.StatusCode}: {error}");
            }
            return true;
        }

        public async Task<List<LogDto>> GetLogsAsync(int count = 50)
        {
            _logger.LogInformation("Fetching audit logs from API, count: {Count}", count);
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/logs?count={count}");
            AttachBearerToken(request);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return await response.Content.ReadFromJsonAsync<List<LogDto>>(options) ?? new List<LogDto>();
            }
            _logger.LogWarning("Failed to fetch logs. Status: {StatusCode}", response.StatusCode);
            return new List<LogDto>();
        }
    }
}
