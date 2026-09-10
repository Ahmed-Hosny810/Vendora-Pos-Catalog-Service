using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Pos.CatalogService.Application.DTOS.Storage;
using Pos.CatalogService.Application.Features.ProductImages.Commands.CompleteUploadCommand;
using Pos.CatalogService.Application.Features.ProductImages.Commands.StartUploadCommand;
using Pos.CatalogService.Domain.Constants;

namespace Pos.CatalogService.IntegrationTests.Tests
{
    public class ProductImagesTests
    {
        private readonly TestFixture _fixture;

        public ProductImagesTests()
        {
            _fixture = new TestFixture();
        }

        [Fact]
        public async Task StartProductImageUpload_WhenRequestIsValid_ShouldCreatePendingSession()
        {
            // Arrange
            var product = await SeedProductAsync();

            var command = new StartProductImageUploadCommand
            {
                ProductId = product.Id,
                FileName = "coffee.jpg",
                ContentType = "image/jpeg",
                FileSize = 1024
            };

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.UploadUrl.Should().StartWith(
                "https://fake-storage.test/upload/");

            var session = await _fixture.DbContext.ImageUploadSessions
                .FirstOrDefaultAsync(
                    x => x.Id == result.Value.UploadSessionId);

            session.Should().NotBeNull();
            session!.TenantId.Should().Be(TestDataFactory.TenantId);
            session.ProductId.Should().Be(product.Id);
            session.Status.Should().Be(ImageUploadStatuses.Pending);
            session.ExpectedContentType.Should().Be("image/jpeg");
            session.StorageKey.Should().EndWith(".jpg");
        }

        [Fact]
        public async Task StartProductImageUpload_WhenFileIsTooLarge_ShouldFail()
        {
            // Arrange
            var product = await SeedProductAsync();

            var command = new StartProductImageUploadCommand
            {
                ProductId = product.Id,
                FileName = "coffee.jpg",
                ContentType = "image/jpeg",
                FileSize = (5 * 1024 * 1024) + 1
            };

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsFailure.Should().BeTrue();

            result.Errors.Should().Contain(
                "Image exceeds the maximum allowed size.");

            var sessionsCount =
                await _fixture.DbContext.ImageUploadSessions
                    .CountAsync();

            sessionsCount.Should().Be(0);
        }

        [Fact]
        public async Task StartProductImageUpload_WhenContentTypeIsUnsupported_ShouldFail()
        {
            // Arrange
            var product = await SeedProductAsync();

            var command = new StartProductImageUploadCommand
            {
                ProductId = product.Id,
                FileName = "document.pdf",
                ContentType = "application/pdf",
                FileSize = 1024
            };

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsFailure.Should().BeTrue();

            result.Errors.Should().Contain(
                "Unsupported image content type.");
        }

        [Fact]
        public async Task CompleteProductImageUpload_WhenBlobIsValid_ShouldCreateImage()
        {
            // Arrange
            var product = await SeedProductAsync();

            var uploadSession =
                TestDataFactory.CreateUploadSession(product.Id);

            await _fixture.DbContext.ImageUploadSessions.AddAsync(
                uploadSession);

            await _fixture.SaveChangesAndClearAsync();

            _fixture.ImageStorageService.BlobInfo =
                new StoredImageInfo(
                    uploadSession.StorageKey,
                    "https://fake-storage.test/images/product.jpg",
                    1024,
                    "image/jpeg",
                    DateTimeOffset.UtcNow);

            var command = new CompleteProductImageUploadCommand
            {
                UploadSessionId = uploadSession.Id,
                SortOrder = 1,
                IsMain = true
            };

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            _fixture.DbContext.ChangeTracker.Clear();

            var image = await _fixture.DbContext.ProductImages
                .FirstOrDefaultAsync(x => x.Id == result.Value);

            image.Should().NotBeNull();
            image!.TenantId.Should().Be(TestDataFactory.TenantId);
            image.ProductId.Should().Be(product.Id);
            image.StorageKey.Should().Be(uploadSession.StorageKey);

            image.ImageUrl.Should().Be(
                "https://fake-storage.test/images/product.jpg");

            image.IsMain.Should().BeTrue();

            var completedSession =
                await _fixture.DbContext.ImageUploadSessions
                    .FirstAsync(x => x.Id == uploadSession.Id);

            completedSession.Status.Should().Be(
                ImageUploadStatuses.Completed);
        }

        [Fact]
        public async Task CompleteProductImageUpload_WhenBlobIsTooLarge_ShouldRejectAndDeleteBlob()
        {
            // Arrange
            var product = await SeedProductAsync();

            var uploadSession =
                TestDataFactory.CreateUploadSession(product.Id);

            await _fixture.DbContext.ImageUploadSessions.AddAsync(
                uploadSession);

            await _fixture.SaveChangesAndClearAsync();

            _fixture.ImageStorageService.BlobInfo =
                new StoredImageInfo(
                    uploadSession.StorageKey,
                    "https://fake-storage.test/images/product.jpg",
                    uploadSession.MaxSizeBytes + 1,
                    "image/jpeg",
                    DateTimeOffset.UtcNow);

            var command = new CompleteProductImageUploadCommand
            {
                UploadSessionId = uploadSession.Id
            };

            // Act
            var result = await _fixture.Mediator.Send(command);

            // Assert
            result.IsFailure.Should().BeTrue();

            result.Errors.Should().Contain(
                "Uploaded image exceeds the allowed size.");

            _fixture.ImageStorageService.DeletedStorageKeys
                .Should()
                .Contain(uploadSession.StorageKey);

            _fixture.DbContext.ChangeTracker.Clear();

            var rejectedSession =
                await _fixture.DbContext.ImageUploadSessions
                    .FirstAsync(x => x.Id == uploadSession.Id);

            rejectedSession.Status.Should().Be(
                ImageUploadStatuses.Rejected);

            var imagesCount =
                await _fixture.DbContext.ProductImages.CountAsync();

            imagesCount.Should().Be(0);
        }

        private async Task<Domain.Models.Product> SeedProductAsync()
        {
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

            return product;
        }
    }
}