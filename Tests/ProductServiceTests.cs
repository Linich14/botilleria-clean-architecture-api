using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.EntityFrameworkCore;
using botilleria_clean_architecture_api.Core.Application.Services;
using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;
using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;
using Microsoft.AspNetCore.Http;

namespace botilleria_clean_architecture_api.Tests;

/// <summary>
/// Pruebas unitarias para ProductService
/// </summary>
public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly AuditService _auditService;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _auditService = new AuditService(_mockUnitOfWork.Object, _mockHttpContextAccessor.Object);
        _productService = new ProductService(_mockUnitOfWork.Object, _auditService);
    }

    [Fact]
    public async Task CrearProductoAsync_DebeCrearProducto_CuandoLosDatosSonValidos()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Vino Tinto Test",
            Description = "Vino de prueba",
            Price = 10000,
            Stock = 50,
            IsAvailable = true,
            CategoryId = 1,
            BrandId = 1,
            ProductTypeId = 1
        };

        var mockCategory = new Category { Id = 1, Name = "Vinos Tintos" };
        var mockBrand = new Brand { Id = 1, Name = "Test Brand" };
        var mockProductType = new ProductType { Id = 1, Name = "Vino" };
        var mockAuditLogRepo = new Mock<IAuditLogRepository>();

        _mockUnitOfWork.Setup(x => x.Categories.GetByIdAsync(1))
            .ReturnsAsync(mockCategory);
        _mockUnitOfWork.Setup(x => x.Brands.GetByIdAsync(1))
            .ReturnsAsync(mockBrand);
        _mockUnitOfWork.Setup(x => x.ProductTypes.GetByIdAsync(1))
            .ReturnsAsync(mockProductType);
        
        _mockUnitOfWork.Setup(x => x.Products.AddAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);
        
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockUnitOfWork.Setup(x => x.AuditLogs)
            .Returns(mockAuditLogRepo.Object);

        // Act
        var result = await _productService.CreateProductAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Vino Tinto Test");
        result.Description.Should().Be("Vino de prueba");
        result.Price.Should().Be(10000);
        result.Stock.Should().Be(50);
        result.IsAvailable.Should().BeTrue();

        _mockUnitOfWork.Verify(x => x.Products.AddAsync(It.IsAny<Product>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ObtenerProductoAsync_DebeRetornarProducto_CuandoElProductoExiste()
    {
        // Preparar (Arrange)
        var idProducto = 1;
        var productoEsperado = new Product
        {
            Id = idProducto,
            Name = "Vino Tinto Existente",
            Description = "Descripción del vino",
            Price = 15000,
            Stock = 20,
            IsAvailable = true
        };

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(idProducto))
            .ReturnsAsync(productoEsperado);

        // Actuar (Act)
        var resultado = await _productService.GetProductAsync(
            new Core.Application.DTOs.Queries.GetProductQuery { Id = idProducto });

        // Verificar (Assert)
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(idProducto);
        resultado.Name.Should().Be("Vino Tinto Existente");
        resultado.Price.Should().Be(15000);
        resultado.Stock.Should().Be(20);

        _mockUnitOfWork.Verify(x => x.Products.GetByIdAsync(idProducto), Times.Once);
    }
}
