using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Pos.CatalogService.Application.Features.Units.Commands.CreateCommand;


namespace Pos.CatalogService.IntegrationTests.Tests
{
    public class UnitsTests
    {
        private readonly TestFixture _fixture;

        public UnitsTests()
        {
            _fixture = new TestFixture();
        }

        [Fact]
        public async Task CreateUnit_WhenRequestIsValid_ShouldCreateTenantUnit()
        {
            // Arrange
            var command = new CreateUnitCommand
            {
                Name = "  Kilogram  ",
                Symbol = "  KG  ",
                IsDecimalAllowed = true
            };

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var unit = await _fixture.DbContext.Units
                .FirstOrDefaultAsync(x => x.Id == result.Value);

            unit.Should().NotBeNull();
            unit!.TenantId.Should().Be(TestDataFactory.TenantId);
            unit.Name.Should().Be("Kilogram");
            unit.Symbol.Should().Be("KG");
            unit.IsDecimalAllowed.Should().BeTrue();
        }

        [Fact]
        public async Task CreateUnit_WhenSymbolExistsForTenant_ShouldFail()
        {
            // Arrange
            var existingUnit = TestDataFactory.CreateUnit(
                symbol: "KG");

            await _fixture.DbContext.Units.AddAsync(existingUnit);
            await _fixture.SaveChangesAndClearAsync();

            var command = new CreateUnitCommand
            {
                Name = "Another Kilogram",
                Symbol = "KG"
            };

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(
                "Unit symbol already exists.");
        }

        [Fact]
        public async Task CreateUnit_WhenSymbolExistsGlobally_ShouldFail()
        {
            // Arrange
            var globalUnit = TestDataFactory.CreateUnit(
                symbol: "PC");

            globalUnit.TenantId = null;

            await _fixture.DbContext.Units.AddAsync(globalUnit);
            await _fixture.SaveChangesAndClearAsync();

            var command = new CreateUnitCommand
            {
                Name = "Tenant Piece",
                Symbol = "PC"
            };

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(
                "Unit symbol already exists.");
        }
    }
}
