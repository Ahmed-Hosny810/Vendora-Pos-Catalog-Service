using Pos.CatalogService.Application.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Units.Queries.GetAllQuery
{
    public class GetUnitsQueryParameter : RequestParameter<UnitOrderKey>
    {
        public UnitFilter? Filter { get; set; }
    }

    public class UnitFilter
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public string? Symbol { get; set; }

        public bool? IsDecimalAllowed { get; set; }

        public bool? IsGlobal { get; set; }
    }

    public enum UnitOrderKey
    {
        Name,
        Symbol,
        CreatedAt,
        UpdatedAt
    }
}
