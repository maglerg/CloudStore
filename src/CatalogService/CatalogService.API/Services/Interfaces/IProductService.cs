
using CatalogService.API.DTOs.Products;

namespace CatalogService.API.Services.Interfaces;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetAllAsync();

    Task<ProductResponseDto?> GetByIdAsync(Guid id);

    Task<ProductResponseDto> CreateAsync(CreateProductDto dto);

    Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductDto dto);

    Task<bool> DeleteAsync(Guid id);
}
