// Coordinar operaciones de productos manteniendo la lógica de negocio centralizada
using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;
using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;

namespace botilleria_clean_architecture_api.Core.Application.Services;

public class ProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AuditService _auditService;

    public ProductService(IUnitOfWork unitOfWork, AuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
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
            var category = await _unitOfWork.Categories.GetByIdAsync(command.CategoryId.Value);
            if (category != null) product.CategoryId = category.Id;
        }
        if (command.BrandId.HasValue)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(command.BrandId.Value);
            if (brand != null) product.BrandId = brand.Id;
        }
        if (command.ProductTypeId.HasValue)
        {
            var productType = await _unitOfWork.ProductTypes.GetByIdAsync(command.ProductTypeId.Value);
            if (productType != null) product.ProductTypeId = productType.Id;
        }
        if (command.OriginId.HasValue)
        {
            var origin = await _unitOfWork.Origins.GetByIdAsync(command.OriginId.Value);
            if (origin != null) product.OriginId = origin.Id;
        }

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        // Registrar operación de creación para auditoría
        await _auditService.LogOperationAsync("CREATE", "Product", product.Id, null, command);

        return product;
    }

    public async Task<Product?> UpdateProductAsync(UpdateProductCommand command)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(command.Id);
        if (product == null) return null;

        // Capturar valores antiguos para auditoría
        var oldValues = new
        {
            product.Name,
            product.Description,
            product.Price,
            product.DiscountPrice,
            product.Volume,
            product.Unit,
            product.AlcoholContent,
            product.Stock,
            product.IsAvailable
        };

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

        await _unitOfWork.Products.UpdateAsync(product);

        // Registrar operación de actualización para auditoría
        await _auditService.LogOperationAsync("UPDATE", "Product", product.Id, oldValues, command);

        return product;
    }

    public async Task<bool> DeleteProductAsync(DeleteProductCommand command)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(command.Id);
        if (product == null) return false;

        // Capturar valores para auditoría antes de eliminar
        var oldValues = new
        {
            product.Name,
            product.Description,
            product.Price,
            product.Stock
        };

        await _unitOfWork.Products.DeleteAsync(product);

        // Registrar operación de eliminación para auditoría
        await _auditService.LogOperationAsync("DELETE", "Product", command.Id, oldValues, null);

        return true;
    }

    public async Task<Product?> GetProductAsync(GetProductQuery query)
    {
        return await _unitOfWork.Products.GetByIdAsync(query.Id);
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(GetProductsQuery query)
    {
        if (query.CategoryId.HasValue)
            return await _unitOfWork.Products.GetProductsByCategoryAsync(query.CategoryId.Value);
        if (query.BrandId.HasValue)
            return await _unitOfWork.Products.GetProductsByBrandAsync(query.BrandId.Value);
        if (query.IsAvailable.HasValue && query.IsAvailable.Value)
            return await _unitOfWork.Products.GetAvailableProductsAsync();

        return await _unitOfWork.Products.GetAllAsync();
    }

    /// <summary>
    /// Ejemplo de uso de Unit of Work para transacciones ACID.
    /// Crea un producto con todas sus entidades relacionadas en una sola transacción.
    /// Si algo falla, toda la operación se revierte.
    /// </summary>
    public async Task<Product> CreateProductWithTransactionAsync(CreateProductCommand command)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Crear o encontrar entidades relacionadas
            Category? category = null;
            if (command.CategoryId.HasValue)
            {
                category = await _unitOfWork.Categories.GetByIdAsync(command.CategoryId.Value);
            }

            Brand? brand = null;
            if (command.BrandId.HasValue)
            {
                brand = await _unitOfWork.Brands.GetByIdAsync(command.BrandId.Value);
            }

            ProductType? productType = null;
            if (command.ProductTypeId.HasValue)
            {
                productType = await _unitOfWork.ProductTypes.GetByIdAsync(command.ProductTypeId.Value);
            }

            Origin? origin = null;
            if (command.OriginId.HasValue)
            {
                origin = await _unitOfWork.Origins.GetByIdAsync(command.OriginId.Value);
            }

            // Crear el producto
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
                },
                Category = category,
                Brand = brand,
                ProductType = productType,
                Origin = origin
            };

            await _unitOfWork.Products.AddAsync(product);

            // Guardar todos los cambios en una sola transacción
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return product;
        }
        catch (Exception)
        {
            // Si algo falla, revertir toda la transacción
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}