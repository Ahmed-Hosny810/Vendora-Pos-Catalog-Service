using MediatR;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using Pos.CatalogService.Domain.Constants;
using Pos.CatalogService.Domain.Models;


namespace Pos.CatalogService.Application.Features.ProductImages.Commands.CompleteUploadCommand
{
    public class CompleteProductImageUploadCommand : IRequest<Result<Guid>>
    {
        public Guid UploadSessionId { get; set; }

        public int SortOrder { get; set; }

        public bool IsMain { get; set; }
    }

    public class CompleteProductImageUploadCommandHandler: IRequestHandler<CompleteProductImageUploadCommand, Result<Guid>>
    {
        private readonly IImageUploadSessionRepositoryAsync _uploadSessionRepository;
        private readonly IProductImageRepositoryAsync _imageRepository;
        private readonly IImageStorageService _imageStorageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public CompleteProductImageUploadCommandHandler(
            IImageUploadSessionRepositoryAsync uploadSessionRepository,
            IProductImageRepositoryAsync imageRepository,
            IImageStorageService imageStorageService,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _uploadSessionRepository = uploadSessionRepository;
            _imageRepository = imageRepository;
            _imageStorageService = imageStorageService;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(
            CompleteProductImageUploadCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue ||
                tenantId.Value == Guid.Empty)
            {
                return Result<Guid>.Failure(
                    "TenantId claim is missing.");
            }

            var uploadSession =
                await _uploadSessionRepository
                    .GetByIdAndTenantIdAsync(
                        request.UploadSessionId,
                        tenantId.Value,
                        cancellationToken);

            if (uploadSession == null)
            {
                return Result<Guid>.Failure(
                    "Upload session is invalid.");
            }

            if (uploadSession.Status ==
                ImageUploadStatuses.Completed)
            {
                return Result<Guid>.Failure(
                    "Upload has already been completed.");
            }

            if (uploadSession.Status !=
                ImageUploadStatuses.Pending)
            {
                return Result<Guid>.Failure(
                    "Upload session is no longer active.");
            }

            if (uploadSession.ExpiresAt <
                DateTimeOffset.UtcNow)
            {
                uploadSession.Expire();

                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);

                return Result<Guid>.Failure(
                    "Upload session has expired.");
            }

            var blobInfo =
                await _imageStorageService.GetBlobInfoAsync(
                    uploadSession.StorageKey,
                    cancellationToken);

            if (blobInfo == null)
            {
                return Result<Guid>.Failure(
                    "Uploaded image was not found.");
            }

            if (blobInfo.Size >
                uploadSession.MaxSizeBytes)
            {
                uploadSession.Reject();

                await _imageStorageService.DeleteAsync(
                    uploadSession.StorageKey,
                    cancellationToken);

                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);

                return Result<Guid>.Failure(
                    "Uploaded image exceeds the allowed size.");
            }

            if (!string.Equals(
                    blobInfo.ContentType,
                    uploadSession.ExpectedContentType,
                    StringComparison.OrdinalIgnoreCase))
            {
                uploadSession.Reject();

                await _imageStorageService.DeleteAsync(
                    uploadSession.StorageKey,
                    cancellationToken);

                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);

                return Result<Guid>.Failure(
                    "Uploaded image type is invalid.");
            }

            if (request.IsMain)
            {
                var currentMainImage =
                    await _imageRepository
                        .GetMainProductImageAsync(
                            tenantId.Value,
                            uploadSession.ProductId,
                            cancellationToken);

                if (currentMainImage != null)
                {
                    currentMainImage.UnmarkAsMain();

                    _imageRepository.Update(
                        currentMainImage);
                }
            }

            var image = new ProductImage
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId.Value,
                ProductId =uploadSession.ProductId,
                StorageKey =uploadSession.StorageKey,
                ImageUrl =blobInfo.Url,
                SortOrder =request.SortOrder,
                IsMain =request.IsMain,
                CreatedAt =DateTime.UtcNow
            };

            await _imageRepository.AddAsync(
                image,
                cancellationToken);

            uploadSession.Complete();

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return Result<Guid>.Success(
                image.Id);
        }
    }
}
