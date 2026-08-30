using MediatR;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;


namespace Pos.CatalogService.Application.Features.Units.Commands.UpdateCommand
{
    public class UpdateUnitCommand : IRequest<Result<Guid>>
    {
        public Guid UnitId { get; set; }

        public string Name { get; set; } = null!;

        public string Symbol { get; set; } = null!;

        public bool IsDecimalAllowed { get; set; }
    }

    public class UpdateUnitCommandHandler: IRequestHandler<UpdateUnitCommand, Result<Guid>>
    {
        private readonly IUnitRepositoryAsync _unitRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUnitCommandHandler(
            IUnitRepositoryAsync unitRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _unitRepository = unitRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(
            UpdateUnitCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result<Guid>.Failure("TenantId claim is missing.");

            var unit = await _unitRepository.GetTenantUnitByIdAsync(
                tenantId.Value,
                request.UnitId,
                cancellationToken);

            if (unit == null)
                return Result<Guid>.Failure($"Unit with Id {request.UnitId} not found.");

            var normalizedName = request.Name.Trim();
            var normalizedSymbol = request.Symbol.Trim();

            var symbolExistsForAnotherUnit =
                await _unitRepository.IsSymbolExistsForAnotherTenantUnitAsync(
                    tenantId.Value,
                    unit.Id,
                    normalizedSymbol,
                    cancellationToken);

            if (symbolExistsForAnotherUnit)
                return Result<Guid>.Failure("Unit symbol already exists.");

            unit.Name = normalizedName;
            unit.Symbol = normalizedSymbol;
            unit.IsDecimalAllowed = request.IsDecimalAllowed;
            unit.UpdatedAt = DateTime.UtcNow;

            _unitRepository.Update(unit);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(unit.Id);
        }
    }
}
