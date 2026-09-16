using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using Ecommerce.UI.ViewModel;

namespace Ecommerce.UI.ServiceLayer
{
    public class ProductServiceClient

    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7156/api/Product";

        public ProductServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProductDto>> GetProductsAsync(int? catId, int? subCatId, int? genId)
        {
            var query = string.Empty;
            if (catId.HasValue) query += $"&categoryId={catId}";
            if (subCatId.HasValue) query += $"&subCategoryId={subCatId}";
            if (genId.HasValue) query += $"&genderId={genId}";

            var url = $"{BaseUrl}/products?{query.TrimStart('&')}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return await response.Content.ReadFromJsonAsync<List<ProductDto>>(options) ?? new List<ProductDto>();
            }
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

        public async Task<ProductDto> GetProductByIdAsync(int id)
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
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, product);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API Error {response.StatusCode}: {error}");
            }
            return true;
        }

        public async Task<bool> UpdateProductAsync(ProductDto product)
        {
            var response = await _httpClient.PutAsJsonAsync(BaseUrl, product);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API Error {response.StatusCode}: {error}");
            }
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API Error {response.StatusCode}: {error}");
            }
            return true;
        }
    }
}
