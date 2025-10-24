using Xunit;
using FluentAssertions;
using Moq;
using botilleria_clean_architecture_api.Core.Application.Services;
using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;
using botilleria_clean_architecture_api.Core.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace botilleria_clean_architecture_api.Tests;

public class BrandServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly AuditService _auditService;
    private readonly BrandService _brandService;

    public BrandServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _auditService = new AuditService(_mockUnitOfWork.Object, _mockHttpContextAccessor.Object);
        _brandService = new BrandService(_mockUnitOfWork.Object, _auditService);
    }

    [Fact]
    public async Task GetBrandsAsync_ShouldReturnAllBrands()
    {
        // Arrange
        var expectedBrands = new List<Brand>
        {
            new Brand { Id = 1, Name = "Concha y Toro" },
            new Brand { Id = 2, Name = "Santa Rita" },
            new Brand { Id = 3, Name = "Casillero del Diablo" }
        };

        _mockUnitOfWork.Setup(x => x.Brands.GetAllAsync())
            .ReturnsAsync(expectedBrands);

        // Act
        var result = await _brandService.GetBrandsAsync(new GetBrandsQuery());

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(b => b.Name == "Concha y Toro");
        result.Should().Contain(b => b.Name == "Santa Rita");
        result.Should().Contain(b => b.Name == "Casillero del Diablo");

        _mockUnitOfWork.Verify(x => x.Brands.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateBrandAsync_ShouldCreateBrand_WhenValidDataProvided()
    {
        // Arrange
        var command = new CreateBrandCommand
        {
            Name = "Nueva Marca Test"
        };

        var mockAuditLogRepo = new Mock<IAuditLogRepository>();

        _mockUnitOfWork.Setup(x => x.Brands.AddAsync(It.IsAny<Brand>()))
            .Returns(Task.CompletedTask);
        
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockUnitOfWork.Setup(x => x.AuditLogs)
            .Returns(mockAuditLogRepo.Object);

        // Act
        var result = await _brandService.CreateBrandAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Nueva Marca Test");

        _mockUnitOfWork.Verify(x => x.Brands.AddAsync(It.IsAny<Brand>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.AtLeastOnce);
    }
}
