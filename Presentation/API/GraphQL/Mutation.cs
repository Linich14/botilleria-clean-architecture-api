using HotChocolate;
using HotChocolate.Authorization;
using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Application.Services;
using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;

namespace botilleria_clean_architecture_api.Presentation.API.GraphQL;

public class Mutation
{
    [Authorize]
    public async Task<Product> CreateProduct(CreateProductCommand input, [Service] ProductService productService)
    {
        return await productService.CreateProductAsync(input);
    }

    [Authorize]
    public async Task<Product?> UpdateProduct(int id, UpdateProductCommand input, [Service] ProductService productService)
    {
        input.Id = id;
        return await productService.UpdateProductAsync(input);
    }

    [Authorize]
    public async Task<bool> DeleteProduct(int id, [Service] ProductService productService)
    {
        return await productService.DeleteProductAsync(new DeleteProductCommand { Id = id });
    }

    [Authorize]
    public async Task<Brand> CreateBrand(CreateBrandCommand input, [Service] BrandService brandService)
    {
        return await brandService.CreateBrandAsync(input);
    }

    [Authorize]
    public async Task<Brand?> UpdateBrand(int id, UpdateBrandCommand input, [Service] BrandService brandService)
    {
        input.Id = id;
        return await brandService.UpdateBrandAsync(input);
    }

    [Authorize]
    public async Task<bool> DeleteBrand(int id, [Service] BrandService brandService)
    {
        return await brandService.DeleteBrandAsync(new DeleteBrandCommand { Id = id });
    }

    [Authorize]
    public async Task<Category> CreateCategory(CreateCategoryCommand input, [Service] CategoryService categoryService)
    {
        return await categoryService.CreateCategoryAsync(input);
    }

    [Authorize]
    public async Task<Category?> UpdateCategory(int id, UpdateCategoryCommand input, [Service] CategoryService categoryService)
    {
        input.Id = id;
        return await categoryService.UpdateCategoryAsync(input);
    }

    [Authorize]
    public async Task<bool> DeleteCategory(int id, [Service] CategoryService categoryService)
    {
        return await categoryService.DeleteCategoryAsync(new DeleteCategoryCommand { Id = id });
    }

    [Authorize]
    public async Task<ProductType> CreateProductType(CreateProductTypeCommand input, [Service] ProductTypeService productTypeService)
    {
        return await productTypeService.CreateProductTypeAsync(input);
    }

    [Authorize]
    public async Task<ProductType?> UpdateProductType(int id, UpdateProductTypeCommand input, [Service] ProductTypeService productTypeService)
    {
        input.Id = id;
        return await productTypeService.UpdateProductTypeAsync(input);
    }

    [Authorize]
    public async Task<bool> DeleteProductType(int id, [Service] ProductTypeService productTypeService)
    {
        return await productTypeService.DeleteProductTypeAsync(new DeleteProductTypeCommand { Id = id });
    }

    [Authorize]
    public async Task<Country> CreateCountry(CreateCountryCommand input, [Service] CountryService countryService)
    {
        return await countryService.CreateCountryAsync(input);
    }

    [Authorize]
    public async Task<Country?> UpdateCountry(int id, UpdateCountryCommand input, [Service] CountryService countryService)
    {
        input.Id = id;
        return await countryService.UpdateCountryAsync(input);
    }

    [Authorize]
    public async Task<bool> DeleteCountry(int id, [Service] CountryService countryService)
    {
        return await countryService.DeleteCountryAsync(new DeleteCountryCommand { Id = id });
    }

    [Authorize]
    public async Task<Region> CreateRegion(CreateRegionCommand input, [Service] RegionService regionService)
    {
        return await regionService.CreateRegionAsync(input);
    }

    [Authorize]
    public async Task<Region?> UpdateRegion(int id, UpdateRegionCommand input, [Service] RegionService regionService)
    {
        input.Id = id;
        return await regionService.UpdateRegionAsync(input);
    }

    [Authorize]
    public async Task<bool> DeleteRegion(int id, [Service] RegionService regionService)
    {
        return await regionService.DeleteRegionAsync(new DeleteRegionCommand { Id = id });
    }

    [Authorize]
    public async Task<Origin> CreateOrigin(CreateOriginCommand input, [Service] OriginService originService)
    {
        return await originService.CreateOriginAsync(input);
    }

    [Authorize]
    public async Task<Origin?> UpdateOrigin(int id, UpdateOriginCommand input, [Service] OriginService originService)
    {
        input.Id = id;
        return await originService.UpdateOriginAsync(input);
    }

    [Authorize]
    public async Task<bool> DeleteOrigin(int id, [Service] OriginService originService)
    {
        return await originService.DeleteOriginAsync(new DeleteOriginCommand { Id = id });
    }
}
