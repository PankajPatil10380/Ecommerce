using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Service.Product.DataLayer.Repository;
using Ecommerce.Service.Product.Domain.Entities;
using Ecommerce.Service.Product.BusinessLayer.Dtos;
using Ecommerce.Common.Services;

namespace Ecommerce.Service.Product.BusinessLayer
{
    public interface IProductManager
    {
        Task<ProductDto> GetProductAsync(int id);
        Task<int> CreateProductAsync(ProductDto productDto);
        Task<bool> UpdateProductAsync(ProductDto productDto);
        Task<bool> DeleteProductAsync(int id);
        Task<string> UpsertProductAsync(ProductDto productDto);
        Task<int> GetLogsCountAsync();
        Task<List<ProductCategory>> GetCategoriesAsync();
        Task<List<SubProductCategory>> GetSubCategoriesAsync(int categoryId);
        Task<List<SubProductCategory>> GetAllSubCategoriesAsync();
        Task<List<Gender>> GetGendersAsync();
        Task<List<ProductDto>> GetProductsAsync(int? categoryId, int? subCategoryId, int? genderId);
    }

    public class ProductManager : IProductManager
    {
        private readonly IProductRepository _productRepository;
        private readonly ILoggingService _loggingService;

        public ProductManager(IProductRepository productRepository, ILoggingService loggingService)
        {
            _productRepository = productRepository;
            _loggingService = loggingService;
        }

        public async Task<ProductDto> GetProductAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    await _loggingService.LogAsync("Error", "GetProduct", $"Invalid Product ID: {id}", "Product");
                    throw new ArgumentException("Invalid Product ID");
                }

                var product = await _productRepository.GetProductByIdAsync(id);
                if (product == null)
                {
                    await _loggingService.LogAsync("Info", "GetProduct", $"Product not found for ID: {id}", "Product", id);
                    return null;
                }

                return new ProductDto
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.SubCategory?.CategoryId ?? 0,
                    SubCategoryId = product.SubCategoryId,
                    GenderId = product.GenderId,
                    IsActive = product.IsActive
                };
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                await _loggingService.LogAsync("Error", "GetProduct", $"Unexpected error: {ex.Message}", "Product", id);
                throw;
            }
        }

        public async Task<int> CreateProductAsync(ProductDto productDto)
        {
            try
            {
                var product = new Ecommerce.Service.Product.Domain.Entities.Product
                {
                    ProductName = productDto.ProductName,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    SubCategoryId = productDto.SubCategoryId,
                    GenderId = productDto.GenderId,
                    IsActive = productDto.IsActive
                };

                if (string.IsNullOrWhiteSpace(product.ProductName))
                {
                    await _loggingService.LogAsync("Error", "CreateProduct", "Product name is required", "Product");
                    throw new ArgumentException("Product name is required");
                }

                if (product.Price < 0)
                {
                    await _loggingService.LogAsync("Error", "CreateProduct", $"Price cannot be negative: {product.Price}", "Product");
                    throw new ArgumentException("Price cannot be negative");
                }

                int result = await _productRepository.InsertProductAsync(product);
                await _loggingService.LogAsync("Info", "CreateProduct", $"Product created successfully with ID: {result}", "Product", result);
                return result;
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                await _loggingService.LogAsync("Error", "CreateProduct", $"Unexpected error: {ex.Message}", "Product");
                throw;
            }
        }

        public async Task<bool> UpdateProductAsync(ProductDto productDto)
        {
            try
            {
                var product = new Ecommerce.Service.Product.Domain.Entities.Product
                {
                    Id = productDto.Id,
                    ProductName = productDto.ProductName,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    SubCategoryId = productDto.SubCategoryId,
                    GenderId = productDto.GenderId,
                    IsActive = productDto.IsActive
                };

                if (product.Id <= 0)
                {
                    await _loggingService.LogAsync("Error", "UpdateProduct", $"Existing Product ID is required for update. Provided: {product.Id}", "Product");
                    throw new ArgumentException("Existing Product ID is required for update");
                }

                bool result = await _productRepository.UpdateProductAsync(product);
                if (result)
                {
                    await _loggingService.LogAsync("Info", "UpdateProduct", $"Product updated successfully with ID: {product.Id}", "Product", product.Id);
                }
                else
                {
                    await _loggingService.LogAsync("Warning", "UpdateProduct", $"Product update failed for ID: {product.Id}", "Product", product.Id);
                }
                return result;
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                await _loggingService.LogAsync("Error", "UpdateProduct", $"Unexpected error: {ex.Message}", "Product", productDto.Id);
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    await _loggingService.LogAsync("Error", "DeleteProduct", $"Invalid Product ID: {id}", "Product");
                    throw new ArgumentException("Invalid Product ID");
                }

                bool result = await _productRepository.DeleteProductAsync(id);
                if (result)
                {
                    await _loggingService.LogAsync("Info", "DeleteProduct", $"Product deleted successfully with ID: {id}", "Product", id);
                }
                else
                {
                    await _loggingService.LogAsync("Warning", "DeleteProduct", $"Product deletion failed for ID: {id}", "Product", id);
                }
                return result;
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                await _loggingService.LogAsync("Error", "DeleteProduct", $"Unexpected error: {ex.Message}", "Product", id);
                throw;
            }
        }

        public async Task<string> UpsertProductAsync(ProductDto productDto)
        {
            try
            {
                var product = new Ecommerce.Service.Product.Domain.Entities.Product
                {
                    Id = productDto.Id,
                    ProductName = productDto.ProductName,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    SubCategoryId = productDto.SubCategoryId,
                    GenderId = productDto.GenderId,
                    IsActive = productDto.IsActive
                };

                string result = await _productRepository.SetProductDetailsAsync(product);
                await _loggingService.LogAsync("Info", "UpsertProduct", $"Upsert completed: {result}", "Product", productDto.Id);
                return result;
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Error", "UpsertProduct", $"Unexpected error: {ex.Message}", "Product", productDto.Id);
                throw;
            }
        }

        public async Task<int> GetLogsCountAsync()
        {
            return await _loggingService.GetLogsCountAsync();
        }

        public async Task<List<ProductCategory>> GetCategoriesAsync()
        {
            return await _productRepository.GetCategoriesAsync();
        }

        public async Task<List<SubProductCategory>> GetSubCategoriesAsync(int categoryId)
        {
            return await _productRepository.GetSubCategoriesAsync(categoryId);
        }

        public async Task<List<SubProductCategory>> GetAllSubCategoriesAsync()
        {
            return await _productRepository.GetAllSubCategoriesAsync();
        }

        public async Task<List<Gender>> GetGendersAsync()
        {
            return await _productRepository.GetGendersAsync();
        }

        public async Task<List<ProductDto>> GetProductsAsync(int? categoryId, int? subCategoryId, int? genderId)
        {
            var products = await _productRepository.GetProductsAsync(categoryId, subCategoryId, genderId);

            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                ProductName = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.SubCategory?.CategoryId ?? 0,
                SubCategoryId = p.SubCategoryId,
                GenderId = p.GenderId,
                IsActive = p.IsActive
            }).ToList();
        }
    }
}
