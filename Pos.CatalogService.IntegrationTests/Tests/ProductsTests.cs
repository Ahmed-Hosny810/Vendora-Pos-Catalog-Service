using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Pos.CatalogService.Application.Features.Products.Commands.CreateCommand;
using Pos.CatalogService.Domain.Constants;

namespace Pos.CatalogService.IntegrationTests.Tests
{
    public class ProductsTests
    {
        private readonly TestFixture _fixture;

        public ProductsTests()
        {
            _fixture = new TestFixture();
        }

        [Fact]
        public async Task CreateProduct_WhenRequestIsValid_ShouldCreateActiveProduct()
        {
            // Arrange
            var category = TestDataFactory.CreateCategory();
            var unit = TestDataFactory.CreateUnit();
            var taxRate = TestDataFactory.CreateTaxRate();

            await _fixture.DbContext.AddRangeAsync(
                category,
                unit,
                taxRate);

            await _fixture.SaveChangesAndClearAsync();

            var command = new CreateProductCommand
            {
                CategoryId = category.Id,
                UnitId = unit.Id,
                TaxRateId = taxRate.Id,
                NameAr = "قهوة",
                NameEn = "  Coffee  ",
                Sku = "  COFFEE-001  ",
                Barcode = "  622100000001  ",
                CostPrice = 40,
                SellingPrice = 60,
                TrackInventory = true
            };

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var product = await _fixture.DbContext.Products
                .FirstOrDefaultAsync(x => x.Id == result.Value);

            product.Should().NotBeNull();
            product!.TenantId.Should().Be(TestDataFactory.TenantId);
            product.NameEn.Should().Be("Coffee");
            product.NameAr.Should().Be("قهوة");
            product.Sku.Should().Be("COFFEE-001");
            product.Barcode.Should().Be("622100000001");
            product.Status.Should().Be(ProductStatuses.Active);

            _fixture.TenantBillingClient
                .IncreaseCallCount.Should().Be(1);
        }

        [Fact]
        public async Task CreateProduct_WhenCategoryIsInactive_ShouldFail()
        {
            // Arrange
            var category = TestDataFactory.CreateCategory(
                status: CategoryStatuses.Inactive);

            var unit = TestDataFactory.CreateUnit();
            var taxRate = TestDataFactory.CreateTaxRate();

            await _fixture.DbContext.AddRangeAsync(
                category,
                unit,
                taxRate);

            await _fixture.SaveChangesAndClearAsync();

            var command = CreateCommand(
                category.Id,
                unit.Id,
                taxRate.Id);

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain("Category is inactive.");

            var productsCount =
                await _fixture.DbContext.Products.CountAsync();

            productsCount.Should().Be(0);
        }

        [Fact]
        public async Task CreateProduct_WhenSkuAlreadyExists_ShouldFail()
        {
            // Arrange
            var category = TestDataFactory.CreateCategory();
            var unit = TestDataFactory.CreateUnit();
            var taxRate = TestDataFactory.CreateTaxRate();

            var existingProduct = TestDataFactory.CreateProduct(
                category.Id,
                unit.Id,
                taxRate.Id,
                sku: "COFFEE-001");

            await _fixture.DbContext.AddRangeAsync(
                category,
                unit,
                taxRate,
                existingProduct);

            await _fixture.SaveChangesAndClearAsync();

            var command = CreateCommand(
                category.Id,
                unit.Id,
                taxRate.Id);

            command.Sku = "COFFEE-001";

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(
                "Product SKU already exists.");

            _fixture.TenantBillingClient
                .IncreaseCallCount.Should().Be(0);
        }

        [Fact]
        public async Task CreateProduct_WhenBillingRejectsUsage_ShouldNotPersistProduct()
        {
            // Arrange
            var category = TestDataFactory.CreateCategory();
            var unit = TestDataFactory.CreateUnit();
            var taxRate = TestDataFactory.CreateTaxRate();

            await _fixture.DbContext.AddRangeAsync(
                category,
                unit,
                taxRate);

            await _fixture.SaveChangesAndClearAsync();

            _fixture.TenantBillingClient.ShouldFailIncrease = true;

            var command = CreateCommand(
                category.Id,
                unit.Id,
                taxRate.Id);

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(
                "Maximum product limit reached.");

            var productsCount =
                await _fixture.DbContext.Products.CountAsync();

            productsCount.Should().Be(0);
        }

        private static CreateProductCommand CreateCommand(
            Guid categoryId,
            Guid unitId,
            Guid taxRateId)
        {
            return new CreateProductCommand
            {
                CategoryId = categoryId,
                UnitId = unitId,
                TaxRateId = taxRateId,
                NameEn = "Coffee",
                Sku = "COFFEE-001",
                Barcode = "622100000001",
                CostPrice = 40,
                SellingPrice = 60,
                TrackInventory = true
            };
        }
    }
}