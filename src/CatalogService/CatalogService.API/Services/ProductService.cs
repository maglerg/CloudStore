
using CatalogService.API.Data;
using CatalogService.API.Domain.Entities;
using CatalogService.API.DTOs.Products;
using CatalogService.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.API.Services;

public class ProductService : IProductService
{
    private readonly CatalogDbContext _context;

    public ProductService(CatalogDbContext context)
    {
        _context = context;
    }

    // Получение всех товаров
    public async Task<List<ProductResponseDto>> GetAllAsync()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .ToListAsync();

        return products.Select(MapToDto).ToList();
    }

    // Получение товара по ID
    public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return null;

        return MapToDto(product);
    }

    // Создание товара
    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
    {
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == dto.CategoryId);

        if (!categoryExists)
            throw new KeyNotFoundException("Category not found.");

        var skuExists = await _context.Products
            .AnyAsync(p => p.SKU == dto.SKU);

        if (skuExists)
            throw new InvalidOperationException("Product with this SKU already exists.");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            SKU = dto.SKU,
            Price = dto.Price,
            ImageUrl = dto.ImageUrl,
            CategoryId = dto.CategoryId,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);

        await _context.SaveChangesAsync();

        await _context.Entry(product)
            .Reference(p => p.Category)
            .LoadAsync();

        return MapToDto(product);
    }

    // Обновление товара
    public async Task<ProductResponseDto?> UpdateAsync(
        Guid id,
        UpdateProductDto dto)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return null;

        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == dto.CategoryId);

        if (!categoryExists)
            throw new KeyNotFoundException("Category not found.");

        var skuExists = await _context.Products
            .AnyAsync(p => p.SKU == dto.SKU && p.Id != id);

        if (skuExists)
            throw new InvalidOperationException("Product with this SKU already exists.");

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.SKU = dto.SKU;
        product.Price = dto.Price;
        product.ImageUrl = dto.ImageUrl;
        product.CategoryId = dto.CategoryId;
        product.IsActive = dto.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _context.Entry(product)
            .Reference(p => p.Category)
            .LoadAsync();

        return MapToDto(product);
    }

    // Удаление товара
    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return false;

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();

        return true;
    }

    // Преобразование сущности Product в ProductResponseDto
    private static ProductResponseDto MapToDto(Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            SKU = product.SKU,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
