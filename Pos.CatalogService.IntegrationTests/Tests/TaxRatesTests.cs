using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Pos.CatalogService.Application.Features.TaxRates.Commands.CreateCommand;

namespace Pos.CatalogService.IntegrationTests.Tests
{
    public class TaxRatesTests
    {
        private readonly TestFixture _fixture;

        public TaxRatesTests()
        {
            _fixture = new TestFixture();
        }

        [Fact]
        public async Task CreateTaxRate_WhenRequestIsValid_ShouldCreateActiveTaxRate()
        {
            // Arrange
            var command = new CreateTaxRateCommand
            {
                Name = "  VAT 14%  ",
                Rate = 14,
                IsDefault = false
            };

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var taxRate = await _fixture.DbContext.TaxRates
                .FirstOrDefaultAsync(x => x.Id == result.Value);

            taxRate.Should().NotBeNull();
            taxRate!.TenantId.Should().Be(TestDataFactory.TenantId);
            taxRate.Name.Should().Be("VAT 14%");
            taxRate.Rate.Should().Be(14);
            taxRate.IsActive.Should().BeTrue();
            taxRate.IsDefault.Should().BeFalse();
        }

        [Fact]
        public async Task CreateTaxRate_WhenNameAlreadyExists_ShouldFail()
        {
            // Arrange
            var existingTaxRate = TestDataFactory.CreateTaxRate(
                name: "VAT 14%");

            await _fixture.DbContext.TaxRates.AddAsync(
                existingTaxRate);

            await _fixture.SaveChangesAndClearAsync();

            var command = new CreateTaxRateCommand
            {
                Name = "VAT 14%",
                Rate = 14
            };

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(
                "Tax rate name already exists.");
        }

        [Fact]
        public async Task CreateDefaultTaxRate_WhenDefaultExists_ShouldReplaceDefault()
        {
            // Arrange
            var currentDefault = TestDataFactory.CreateTaxRate(
                name: "VAT 14%",
                rate: 14,
                isDefault: true);

            await _fixture.DbContext.TaxRates.AddAsync(
                currentDefault);

            await _fixture.SaveChangesAndClearAsync();

            var command = new CreateTaxRateCommand
            {
                Name = "Reduced VAT",
                Rate = 5,
                IsDefault = true
            };

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            _fixture.DbContext.ChangeTracker.Clear();

            var oldTaxRate = await _fixture.DbContext.TaxRates
                .FirstAsync(x => x.Id == currentDefault.Id);

            var newTaxRate = await _fixture.DbContext.TaxRates
                .FirstAsync(x => x.Id == result.Value);

            oldTaxRate.IsDefault.Should().BeFalse();
            newTaxRate.IsDefault.Should().BeTrue();
        }
    }
}