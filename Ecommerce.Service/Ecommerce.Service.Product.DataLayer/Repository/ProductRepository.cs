using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            return await _context.Products
                .Include(p => p.SubCategory)
                .Include(p => p.Gender)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<int> InsertProductAsync(Ecommerce.Service.Product.Domain.Entities.Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            // Remove associated orders first to avoid FK constraint violations
            var orders = await _context.Orders
                .Where(o => o.ProductId == productId)
                .ToListAsync();

            if (orders.Any())
            {
                var orderIds = orders.Select(o => o.Id).ToList();

                // Remove all transactions linked to these orders
                var transactions = await _context.Transactions
                    .Where(t => orderIds.Contains(t.OrderId))
                    .ToListAsync();

                if (transactions.Any())
                {
                    _context.Transactions.RemoveRange(transactions);
                }

                // Remove the orders
                _context.Orders.RemoveRange(orders);
            }

            // Finally, remove the product
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProductAsync(Ecommerce.Service.Product.Domain.Entities.Product product)
        {
            _context.Products.Update(product);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<string> SetProductDetailsAsync(Ecommerce.Service.Product.Domain.Entities.Product product)
        {
            if (product.Id == 0)
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                return $"Product created successfully with ID: {product.Id}";
            }
            else
            {
                var existingProduct = await _context.Products.FindAsync(product.Id);
                if (existingProduct == null) return "Product not found";

                existingProduct.ProductName = product.ProductName;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.SubCategoryId = product.SubCategoryId;
                existingProduct.GenderId = product.GenderId;
                existingProduct.IsActive = product.IsActive;

                await _context.SaveChangesAsync();
                return "Product updated successfully";
            }
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
