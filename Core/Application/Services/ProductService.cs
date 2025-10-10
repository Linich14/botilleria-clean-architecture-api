using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;
using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;

namespace botilleria_clean_architecture_api.Core.Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly IProductTypeRepository _productTypeRepository;
    private readonly IOriginRepository _originRepository;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IBrandRepository brandRepository,
        IProductTypeRepository productTypeRepository,
        IOriginRepository originRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
        _productTypeRepository = productTypeRepository;
        _originRepository = originRepository;
    }

    public async Task<Product> CreateProductAsync(CreateProductCommand command)
    {
        var product = new Product
        {
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            DiscountPrice = command.DiscountPrice,
            Volume = command.Volume,
            Unit = command.Unit,
            AlcoholContent = command.AlcoholContent,
            Stock = command.Stock,
            IsAvailable = command.IsAvailable,
            CreatedAt = DateTime.UtcNow,
            Characteristics = new Characteristics
            {
                Color = command.Color,
                Aroma = command.Aroma,
                Taste = command.Taste,
                ServingTemperature = command.ServingTemperature,
                FoodPairingJson = command.FoodPairing != null ? System.Text.Json.JsonSerializer.Serialize(command.FoodPairing) : null
            }
        };

        // Asignar relaciones si se proporcionan IDs
        if (command.CategoryId.HasValue)
        {
            var category = await _categoryRepository.GetByIdAsync(command.CategoryId.Value);
            if (category != null) product.CategoryId = category.Id;
        }
        if (command.BrandId.HasValue)
        {
            var brand = await _brandRepository.GetByIdAsync(command.BrandId.Value);
            if (brand != null) product.BrandId = brand.Id;
        }
        if (command.ProductTypeId.HasValue)
        {
            var productType = await _productTypeRepository.GetByIdAsync(command.ProductTypeId.Value);
            if (productType != null) product.ProductTypeId = productType.Id;
        }
        if (command.OriginId.HasValue)
        {
            var origin = await _originRepository.GetByIdAsync(command.OriginId.Value);
            if (origin != null) product.OriginId = origin.Id;
        }

        await _productRepository.AddAsync(product);
        return product;
    }

    public async Task<Product?> UpdateProductAsync(UpdateProductCommand command)
    {
        var product = await _productRepository.GetByIdAsync(command.Id);
        if (product == null) return null;

        product.Name = command.Name;
        product.Description = command.Description;
        product.Price = command.Price;
        product.DiscountPrice = command.DiscountPrice;
        product.Volume = command.Volume;
        product.Unit = command.Unit;
        product.AlcoholContent = command.AlcoholContent;
        product.Stock = command.Stock;
        product.IsAvailable = command.IsAvailable;
        product.UpdatedAt = DateTime.UtcNow;

        if (product.Characteristics != null)
        {
            product.Characteristics.Color = command.Color;
            product.Characteristics.Aroma = command.Aroma;
            product.Characteristics.Taste = command.Taste;
            product.Characteristics.ServingTemperature = command.ServingTemperature;
            product.Characteristics.FoodPairingJson = command.FoodPairing != null ? System.Text.Json.JsonSerializer.Serialize(command.FoodPairing) : null;
        }

        // Actualizar relaciones
        product.CategoryId = command.CategoryId;
        product.BrandId = command.BrandId;
        product.ProductTypeId = command.ProductTypeId;
        product.OriginId = command.OriginId;

        await _productRepository.UpdateAsync(product);
        return product;
    }

    public async Task<bool> DeleteProductAsync(DeleteProductCommand command)
    {
        var product = await _productRepository.GetByIdAsync(command.Id);
        if (product == null) return false;

        await _productRepository.DeleteAsync(product);
        return true;
    }

    public async Task<Product?> GetProductAsync(GetProductQuery query)
    {
        return await _productRepository.GetByIdAsync(query.Id);
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(GetProductsQuery query)
    {
        if (query.CategoryId.HasValue)
            return await _productRepository.GetProductsByCategoryAsync(query.CategoryId.Value);
        if (query.BrandId.HasValue)
            return await _productRepository.GetProductsByBrandAsync(query.BrandId.Value);
        if (query.IsAvailable.HasValue && query.IsAvailable.Value)
            return await _productRepository.GetAvailableProductsAsync();

        return await _productRepository.GetAllAsync();
    }
}