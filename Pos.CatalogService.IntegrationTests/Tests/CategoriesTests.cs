using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Pos.CatalogService.Application.Features.Categories.Commands.CreateCommand;
using Pos.CatalogService.Domain.Constants;

namespace Pos.CatalogService.IntegrationTests.Tests
{
    public class CategoriesTests
    {
        private readonly TestFixture _fixture;

        public CategoriesTests()
        {
            _fixture = new TestFixture();
        }

        [Fact]
        public async Task CreateCategory_WhenRequestIsValid_ShouldCreateActiveCategory()
        {
            // Arrange
            var command = new CreateCategoryCommand
            {
                NameAr = "مشروبات",
                NameEn = "  Beverages  ",
                SortOrder = 1,
                IsVisible = true
            };

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();

            var category = await _fixture.DbContext.Categories
                .FirstOrDefaultAsync(x => x.Id == result.Value);

            category.Should().NotBeNull();
            category!.TenantId.Should().Be(TestDataFactory.TenantId);
            category.NameEn.Should().Be("Beverages");
            category.NameAr.Should().Be("مشروبات");
            category.Status.Should().Be(CategoryStatuses.Active);
            category.IsVisible.Should().BeTrue();
        }

        [Fact]
        public async Task CreateCategory_WhenNameAlreadyExists_ShouldFail()
        {
            // Arrange
            var category = TestDataFactory.CreateCategory(
                name: "Beverages");

            await _fixture.DbContext.Categories.AddAsync(category);
            await _fixture.SaveChangesAndClearAsync();

            var command = new CreateCategoryCommand
            {
                NameEn = "Beverages",
                SortOrder = 2
            };

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(
                "Category name already exists.");

            var categoriesCount =
                await _fixture.DbContext.Categories.CountAsync();

            categoriesCount.Should().Be(1);
        }

        [Fact]
        public async Task CreateCategory_WhenParentBelongsToAnotherTenant_ShouldFail()
        {
            // Arrange
            var otherTenantCategory = TestDataFactory.CreateCategory(
                tenantId: TestDataFactory.OtherTenantId);

            await _fixture.DbContext.Categories.AddAsync(
                otherTenantCategory);

            await _fixture.SaveChangesAndClearAsync();

            var command = new CreateCategoryCommand
            {
                ParentCategoryId = otherTenantCategory.Id,
                NameEn = "Child Category"
            };

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(
                "Parent category is invalid.");
        }
    }
}
