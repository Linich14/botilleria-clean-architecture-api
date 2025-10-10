using botilleria_clean_architecture_api.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using botilleria_clean_architecture_api.Infrastructure.Persistence.Repositories;

namespace botilleria_clean_architecture_api.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    private IProductRepository? _products;
    private IBrandRepository? _brands;
    private ICategoryRepository? _categories;
    private IProductTypeRepository? _productTypes;
    private ICountryRepository? _countries;
    private IRegionRepository? _regions;
    private IOriginRepository? _origins;

    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public IBrandRepository Brands => _brands ??= new BrandRepository(_context);
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
    public IProductTypeRepository ProductTypes => _productTypes ??= new ProductTypeRepository(_context);
    public ICountryRepository Countries => _countries ??= new CountryRepository(_context);
    public IRegionRepository Regions => _regions ??= new RegionRepository(_context);
    public IOriginRepository Origins => _origins ??= new OriginRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _context.Dispose();
        _transaction?.Dispose();
    }
}