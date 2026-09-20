using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Service.Product.Domain.Entities;
using Ecommerce.Service.Product.DataLayer;

namespace Ecommerce.Service.Product.DataLayer.Repository
{
    public interface IProductRepository
    {
        Task<Ecommerce.Service.Product.Domain.Entities.Product> GetProductByIdAsync(int productId);
        Task<int> InsertProductAsync(Ecommerce.Service.Product.Domain.Entities.Product product);
        Task<bool> DeleteProductAsync(int productId);
        Task<bool> UpdateProductAsync(Ecommerce.Service.Product.Domain.Entities.Product product);
        Task<string> SetProductDetailsAsync(Ecommerce.Service.Product.Domain.Entities.Product product);
        Task<List<Ecommerce.Service.Product.Domain.Entities.ProductCategory>> GetCategoriesAsync();
        Task<List<Ecommerce.Service.Product.Domain.Entities.SubProductCategory>> GetSubCategoriesAsync(int categoryId);
        Task<List<Ecommerce.Service.Product.Domain.Entities.SubProductCategory>> GetAllSubCategoriesAsync();
        Task<List<Ecommerce.Service.Product.Domain.Entities.Gender>> GetGendersAsync();
        Task<List<Ecommerce.Service.Product.Domain.Entities.Product>> GetProductsAsync(int? categoryId, int? subCategoryId, int? genderId);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _context;

        public ProductRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<Ecommerce.Service.Product.Domain.Entities.Product> GetProductByIdAsync(int productId)
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open) await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "GetProductDetails";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@ProductId", productId));

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var product = new Ecommerce.Service.Product.Domain.Entities.Product
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    SubCategoryId = reader.GetInt32(reader.GetOrdinal("SubCategoryId")),
                    ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                    Description = !reader.IsDBNull(reader.GetOrdinal("Description")) ? reader.GetString(reader.GetOrdinal("Description")) : string.Empty,
                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                    GenderId = reader.GetInt32(reader.GetOrdinal("GenderId")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                };

                int catIdOrdinal = -1;
                try { catIdOrdinal = reader.GetOrdinal("CategoryId"); } catch { }
                int categoryId = (catIdOrdinal >= 0 && !reader.IsDBNull(catIdOrdinal)) ? reader.GetInt32(catIdOrdinal) : 0;

                product.SubCategory = new SubProductCategory
                {
                    Id = product.SubCategoryId,
                    CategoryId = categoryId
                };

                return product;
            }

            return null!;
        }

        public async Task<int> InsertProductAsync(Ecommerce.Service.Product.Domain.Entities.Product product)
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open) await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "InsertProductDetails";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@SubCategoryId", product.SubCategoryId));
            command.Parameters.Add(new SqlParameter("@ProductName", product.ProductName));
            command.Parameters.Add(new SqlParameter("@Description", (object?)product.Description ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@Price", product.Price));
            command.Parameters.Add(new SqlParameter("@GenderId", product.GenderId));
            command.Parameters.Add(new SqlParameter("@IsActive", product.IsActive));

            var result = await command.ExecuteScalarAsync();
            int newId = result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
            product.Id = newId;
            return newId;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open) await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "DeleteProductDetails";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@ProductId", productId));

            var result = await command.ExecuteScalarAsync();
            return result != null && result != DBNull.Value && Convert.ToInt32(result) > 0;
        }

        public async Task<bool> UpdateProductAsync(Ecommerce.Service.Product.Domain.Entities.Product product)
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open) await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "UpdateProductDetails";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@ProductId", product.Id));
            command.Parameters.Add(new SqlParameter("@SubCategoryId", product.SubCategoryId));
            command.Parameters.Add(new SqlParameter("@ProductName", product.ProductName));
            command.Parameters.Add(new SqlParameter("@Description", (object?)product.Description ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@Price", product.Price));
            command.Parameters.Add(new SqlParameter("@GenderId", product.GenderId));
            command.Parameters.Add(new SqlParameter("@IsActive", product.IsActive));

            var result = await command.ExecuteScalarAsync();
            return result != null && result != DBNull.Value && Convert.ToInt32(result) > 0;
        }

        public async Task<string> SetProductDetailsAsync(Ecommerce.Service.Product.Domain.Entities.Product product)
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open) await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SetProductDetails";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@ProductId", product.Id > 0 ? product.Id : (object)DBNull.Value));
            command.Parameters.Add(new SqlParameter("@SubCategoryId", product.SubCategoryId));
            command.Parameters.Add(new SqlParameter("@ProductName", product.ProductName));
            command.Parameters.Add(new SqlParameter("@Description", (object?)product.Description ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@Price", product.Price));
            command.Parameters.Add(new SqlParameter("@GenderId", product.GenderId));
            command.Parameters.Add(new SqlParameter("@IsActive", product.IsActive));

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                int msgOrdinal = reader.GetOrdinal("Message");
                return !reader.IsDBNull(msgOrdinal) ? reader.GetString(msgOrdinal) : "Success";
            }

            return "No response from stored procedure";
        }


        public async Task<List<ProductCategory>> GetCategoriesAsync()
        {
            return await _context.ProductCategories.ToListAsync();
        }

        public async Task<List<SubProductCategory>> GetSubCategoriesAsync(int categoryId)
        {
            return await _context.SubProductCategories
                .Where(s => s.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<List<SubProductCategory>> GetAllSubCategoriesAsync()
        {
            return await _context.SubProductCategories
                .ToListAsync();
        }

        public async Task<List<Gender>> GetGendersAsync()
        {
            return await _context.Genders.ToListAsync();
        }

        public async Task<List<Ecommerce.Service.Product.Domain.Entities.Product>> GetProductsAsync(int? categoryId, int? subCategoryId, int? genderId)
        {
            var query = _context.Products.Include(p => p.SubCategory).AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.SubCategory.CategoryId == categoryId.Value);
            }

            if (subCategoryId.HasValue)
            {
                query = query.Where(p => p.SubCategoryId == subCategoryId.Value);
            }

            if (genderId.HasValue)
            {
                query = query.Where(p => p.GenderId == genderId.Value);
            }

            return await query.ToListAsync();
        }
    }
}
