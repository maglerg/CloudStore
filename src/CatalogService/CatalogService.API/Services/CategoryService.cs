using CatalogService.API.Data;
using CatalogService.API.Domain.Entities;
using CatalogService.API.DTOs.Categories;
using CatalogService.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.API.Services;

public class CategoryService : ICategoryService
{
    private readonly CatalogDbContext _context;

    public CategoryService(CatalogDbContext context)
    {
        _context = context;
    }

    // Получение всех категорий
    public async Task<List<CategoryResponseDto>> GetAllAsync()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .ToListAsync();

        return categories.Select(MapToDto).ToList();
    }

    // Получение категории по ID
    public async Task<CategoryResponseDto?> GetByIdAsync(Guid id)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return null;

        return MapToDto(category);
    }

    // Создание категории
    public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Categories.Add(category);

        await _context.SaveChangesAsync();

        return MapToDto(category);
    }

    // Обновление категории
    public async Task<CategoryResponseDto?> UpdateAsync(
        Guid id,
        UpdateCategoryDto dto)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return null;

        category.Name = dto.Name;
        category.Description = dto.Description;

        await _context.SaveChangesAsync();

        return MapToDto(category);
    }

    // Удаление категории
    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return false;

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();

        return true;
    }

    // Преобразование сущности Category в CategoryResponseDto
    private static CategoryResponseDto MapToDto(Category category)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt
        };
    }
}