using MediatR;
using Microsoft.Extensions.Options;
using Pos.CatalogService.Application.DTOS.Storage;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Settings;
using Pos.CatalogService.Application.Wrappers;
using Pos.CatalogService.Domain.Models;


namespace Pos.CatalogService.Application.Features.ProductImages.Commands.StartUploadCommand
{
    public class StartProductImageUploadCommand: IRequest<Result<StartProductImageUploadResponse>>
    {
        public Guid ProductId { get; set; }

        public string FileName { get; set; } = null!;

        public string ContentType { get; set; } = null!;

        public long FileSize { get; set; }
    }

    public class StartProductImageUploadCommandHandler
        : IRequestHandler<
            StartProductImageUploadCommand,
            Result<StartProductImageUploadResponse>>
    {
        private readonly IProductRepositoryAsync _productRepository;
        private readonly IImageUploadSessionRepositoryAsync _uploadSessionRepository;
        private readonly IImageStorageService _imageStorageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        private readonly ProductImageUploadSettings _settings;

        public StartProductImageUploadCommandHandler(
            IProductRepositoryAsync productRepository,
            IImageUploadSessionRepositoryAsync uploadSessionRepository,
            IImageStorageService imageStorageService,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            IOptions<ProductImageUploadSettings> options)
        {
            _productRepository = productRepository;
            _uploadSessionRepository = uploadSessionRepository;
            _imageStorageService = imageStorageService;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;

            _settings = options.Value;
        }

        public async Task<Result<StartProductImageUploadResponse>> Handle(
            StartProductImageUploadCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue ||
                tenantId.Value == Guid.Empty)
            {
                return Result<StartProductImageUploadResponse>
                    .Failure("TenantId claim is missing.");
            }

            var product =
                await _productRepository
                    .GetProductByIdAndTenantIdForUpdateAsync(
                        tenantId.Value,
                        request.ProductId,
                        cancellationToken);

            if (product == null)
            {
                return Result<StartProductImageUploadResponse>
                    .Failure("Product is invalid.");
            }

            // Validate file size
            if (request.FileSize <= 0)
            {
                return Result<StartProductImageUploadResponse>
                    .Failure("File size is invalid.");
            }

            if (request.FileSize > _settings.MaxSizeBytes)
            {
                return Result<StartProductImageUploadResponse>
                    .Failure("Image exceeds the maximum allowed size.");
            }

            // Validate Content-Type
            if (string.IsNullOrWhiteSpace(request.ContentType) ||
                !_settings.AllowedContentTypes.Contains(
                    request.ContentType,
                    StringComparer.OrdinalIgnoreCase))
            {
                return Result<StartProductImageUploadResponse>
                    .Failure("Unsupported image content type.");
            }

            // Validate filename
            if (string.IsNullOrWhiteSpace(request.FileName))
            {
                return Result<StartProductImageUploadResponse>
                    .Failure("File name is required.");
            }

            var extension = Path
                .GetExtension(request.FileName)
                .ToLowerInvariant();

            // Validate extension
            if (string.IsNullOrWhiteSpace(extension) ||
                !_settings.AllowedExtensions.Contains(
                    extension,
                    StringComparer.OrdinalIgnoreCase))
            {
                return Result<StartProductImageUploadResponse>
                    .Failure("Unsupported image extension.");
            }


            var storageKey =
                $"tenants/{tenantId.Value}/products/{product.Id}/" +$"{Guid.NewGuid():N}{extension}";

            var expiresIn =
                TimeSpan.FromMinutes(
                    _settings.UploadExpirationMinutes);

            var expiresAt =
                DateTimeOffset.UtcNow.Add(expiresIn);

            var uploadSession = new ImageUploadSession
            {
                TenantId = tenantId.Value,
                ProductId = product.Id,
                StorageKey = storageKey,
                ExpectedContentType = request.ContentType,
                MaxSizeBytes = _settings.MaxSizeBytes,
                ExpiresAt =expiresAt
            };

            await _uploadSessionRepository.AddAsync(
                uploadSession,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            ImageUploadAccess uploadAccess;

            try
            {
                uploadAccess =
                    await _imageStorageService
                        .CreateUploadAccessAsync(
                            storageKey,
                            expiresIn,
                            cancellationToken);
            }
            catch
            {
                uploadSession.Reject();

                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);

                throw;
            }

            var response =
                new StartProductImageUploadResponse(
                    uploadSession.Id,
                    uploadAccess.UploadUrl,
                    uploadAccess.ExpiresAt);

            return Result<StartProductImageUploadResponse>.Success(response);
        }
    }
}
