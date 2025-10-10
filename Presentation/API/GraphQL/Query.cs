// Consultas flexibles para explorar datos de manera personalizada
using HotChocolate;
using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Application.Services;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;

namespace botilleria_clean_architecture_api.Presentation.API.GraphQL;

public class Query
{
    public async Task<IEnumerable<Product>> GetProducts([Service] ProductService productService)
    {
        var products = await productService.GetProductsAsync(new GetProductsQuery());
        return products;
    }

    public async Task<Product?> GetProduct(int id, [Service] ProductService productService)
    {
        return await productService.GetProductAsync(new GetProductQuery { Id = id });
    }

    public async Task<IEnumerable<Brand>> GetBrands([Service] BrandService brandService)
    {
        var brands = await brandService.GetBrandsAsync(new GetBrandsQuery());
        return brands;
    }

    public async Task<Brand?> GetBrand(int id, [Service] BrandService brandService)
    {
        return await brandService.GetBrandAsync(new GetBrandQuery { Id = id });
    }

    public async Task<IEnumerable<Category>> GetCategories([Service] CategoryService categoryService)
    {
        var categories = await categoryService.GetCategoriesAsync(new GetCategoriesQuery());
        return categories;
    }

    public async Task<Category?> GetCategory(int id, [Service] CategoryService categoryService)
    {
        return await categoryService.GetCategoryAsync(new GetCategoryQuery { Id = id });
    }

    public async Task<IEnumerable<ProductType>> GetProductTypes([Service] ProductTypeService productTypeService)
    {
        var productTypes = await productTypeService.GetProductTypesAsync(new GetProductTypesQuery());
        return productTypes;
    }

    public async Task<ProductType?> GetProductType(int id, [Service] ProductTypeService productTypeService)
    {
        return await productTypeService.GetProductTypeAsync(new GetProductTypeQuery { Id = id });
    }

    public async Task<IEnumerable<Country>> GetCountries([Service] CountryService countryService)
    {
        var countries = await countryService.GetCountriesAsync(new GetCountriesQuery());
        return countries;
    }

    public async Task<Country?> GetCountry(int id, [Service] CountryService countryService)
    {
        return await countryService.GetCountryAsync(new GetCountryQuery { Id = id });
    }

    public async Task<IEnumerable<Region>> GetRegions([Service] RegionService regionService)
    {
        var regions = await regionService.GetRegionsAsync(new GetRegionsQuery());
        return regions;
    }

    public async Task<Region?> GetRegion(int id, [Service] RegionService regionService)
    {
        return await regionService.GetRegionAsync(new GetRegionQuery { Id = id });
    }

    public async Task<IEnumerable<Origin>> GetOrigins([Service] OriginService originService)
    {
        var origins = await originService.GetOriginsAsync(new GetOriginsQuery());
        return origins;
    }

    public async Task<Origin?> GetOrigin(int id, [Service] OriginService originService)
    {
        return await originService.GetOriginAsync(new GetOriginQuery { Id = id });
    }
}
