using MediatR;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;

namespace Pos.CatalogService.Application.Features.Units.Commands.CreateCommand
{
    public class CreateUnitCommand : IRequest<Result<Guid>>
    {
        public string Name { get; set; } = null!;

        public string Symbol { get; set; } = null!;

        public bool IsDecimalAllowed { get; set; }
    }

    public class CreateUnitCommandHandler
        : IRequestHandler<CreateUnitCommand, Result<Guid>>
    {
        private readonly IUnitRepositoryAsync _unitRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUnitCommandHandler(
            IUnitRepositoryAsync unitRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _unitRepository = unitRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(
            CreateUnitCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result<Guid>.Failure("TenantId claim is missing.");

            var normalizedName = request.Name.Trim();
            var normalizedSymbol = request.Symbol.Trim();

            var symbolExists = await _unitRepository.IsSymbolExistsInTenantScopeAsync(
                tenantId.Value,
                normalizedSymbol,
                cancellationToken);

            if (symbolExists)
                return Result<Guid>.Failure("Unit symbol already exists.");

            var unit = new Domain.Models.Unit
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId.Value,
                Name = normalizedName,
                Symbol = normalizedSymbol,
                IsDecimalAllowed = request.IsDecimalAllowed,
                CreatedAt = DateTime.UtcNow
            };

            await _unitRepository.AddAsync(unit, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(unit.Id);
        }
    }
}