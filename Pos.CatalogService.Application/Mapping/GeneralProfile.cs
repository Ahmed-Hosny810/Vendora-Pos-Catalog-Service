using AutoMapper;
using Pos.CatalogService.Application.Features.Categories.DTOS;
using Pos.CatalogService.Application.Features.ProductImages.DTOS;
using Pos.CatalogService.Application.Features.Products.DTOS;
using Pos.CatalogService.Application.Features.ProductVariants.DTOS;
using Pos.CatalogService.Application.Features.TaxRates.DTOS;
using Pos.CatalogService.Domain.Models;


namespace Pos.CatalogService.Application.Mapping
{
    public class GeneralProfile:Profile
    {
        public GeneralProfile()
        {
            CreateMap<TaxRate, TaxRateDto>();

            CreateMap<ProductVariant, ProductVariantDto>();

            CreateMap<Category, CategoryDto>();

            CreateMap<ProductImage, ProductImageDto>();

            CreateMap<Product, ProductDto>()
               .ForMember(
                   dest => dest.CategoryNameEn,
                   opt => opt.MapFrom(src => src.Category != null ? src.Category.NameEn : null))
               .ForMember(
                   dest => dest.CategoryNameAr,
                   opt => opt.MapFrom(src => src.Category != null ? src.Category.NameAr : null))
               .ForMember(
                   dest => dest.UnitName,
                   opt => opt.MapFrom(src => src.Unit != null ? src.Unit.Name : null))
               .ForMember(
                   dest => dest.UnitSymbol,
                   opt => opt.MapFrom(src => src.Unit != null ? src.Unit.Symbol : null))
               .ForMember(
                   dest => dest.TaxRateName,
                   opt => opt.MapFrom(src => src.TaxRate != null ? src.TaxRate.Name : null))
               .ForMember(
                   dest => dest.TaxRateValue,
                   opt => opt.MapFrom(src => src.TaxRate != null ? (decimal?) src.TaxRate.Rate : null));

        }
    }
}
