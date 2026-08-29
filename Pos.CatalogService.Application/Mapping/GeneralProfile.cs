using AutoMapper;
using Pos.CatalogService.Application.Features.Categories.DTOS;
using Pos.CatalogService.Application.Features.TaxRates.DTOS;
using Pos.CatalogService.Domain.Models;


namespace Pos.CatalogService.Application.Mapping
{
    public class GeneralProfile:Profile
    {
        public GeneralProfile()
        {
            CreateMap<TaxRate, TaxRateDto>();

            CreateMap<Category, CategoryDto>();
        }
    }
}
