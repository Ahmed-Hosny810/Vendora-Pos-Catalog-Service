using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Pos.CatalogService.Application.Features.ProductVariants.Commands.CreateCommand;
using Pos.CatalogService.Domain.Constants;

namespace Pos.CatalogService.IntegrationTests.Tests
{
    public class ProductVariantsTests
    {
        private readonly TestFixture _fixture;

        public ProductVariantsTests()
        {
            _fixture = new TestFixture();
        }

        [Fact]
        public async Task CreateProductVariant_WhenRequestIsValid_ShouldCreateActiveVariant()
        {
            // Arrange
            var category = TestDataFactory.CreateCategory();
            var unit = TestDataFactory.CreateUnit();
            var taxRate = TestDataFactory.CreateTaxRate();

            var product = TestDataFactory.CreateProduct(
                category.Id,
                unit.Id,
                taxRate.Id);

            await _fixture.DbContext.AddRangeAsync(
                category,
                unit,
                taxRate,
                product);

            await _fixture.SaveChangesAndClearAsync();

            var command = new CreateProductVariantCommand
            {
                ProductId = product.Id,
                Name = "  Large  ",
                Sku = "  COFFEE-LARGE  ",
                Barcode = "  622100000002  ",
                CostPrice = 50,
                SellingPrice = 80,
                OptionKey1 = "  Size  ",
                OptionValue1 = "  Large  "
            };

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var variant = await _fixture.DbContext.ProductVariants
                .FirstOrDefaultAsync(x => x.Id == result.Value);

            variant.Should().NotBeNull();
            variant!.TenantId.Should().Be(TestDataFactory.TenantId);
            variant.ProductId.Should().Be(product.Id);
            variant.Name.Should().Be("Large");
            variant.Sku.Should().Be("COFFEE-LARGE");
            variant.Barcode.Should().Be("622100000002");
            variant.OptionKey1.Should().Be("Size");
            variant.OptionValue1.Should().Be("Large");

            variant.Status.Should().Be(
                ProductVariantStatuses.Active);
        }

        [Fact]
        public async Task CreateProductVariant_WhenProductIsInvalid_ShouldFail()
        {
            // Arrange
            var command = new CreateProductVariantCommand
            {
                ProductId = Guid.NewGuid(),
                Name = "Large",
                Sku = "COFFEE-LARGE",
                CostPrice = 50,
                SellingPrice = 80
            };

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain("Product is invalid.");

            var variantsCount =
                await _fixture.DbContext.ProductVariants.CountAsync();

            variantsCount.Should().Be(0);
        }

        [Fact]
        public async Task CreateProductVariant_WhenSkuAlreadyExists_ShouldFail()
        {
            // Arrange
            var category = TestDataFactory.CreateCategory();
            var unit = TestDataFactory.CreateUnit();
            var taxRate = TestDataFactory.CreateTaxRate();

            var product = TestDataFactory.CreateProduct(
                category.Id,
                unit.Id,
                taxRate.Id);

            var existingVariant =
                TestDataFactory.CreateProductVariant(
                    product.Id,
                    sku: "COFFEE-LARGE");

            await _fixture.DbContext.AddRangeAsync(
                category,
                unit,
                taxRate,
                product,
                existingVariant);

            await _fixture.SaveChangesAndClearAsync();

            var command = new CreateProductVariantCommand
            {
                ProductId = product.Id,
                Name = "Another Large",
                Sku = "COFFEE-LARGE",
                CostPrice = 50,
                SellingPrice = 80
            };

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(
                "Variant SKU already exists.");

            var variantsCount =
                await _fixture.DbContext.ProductVariants.CountAsync();

            variantsCount.Should().Be(1);
        }
    }
}