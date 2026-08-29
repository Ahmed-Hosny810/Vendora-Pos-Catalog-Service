using Pos.CatalogService.Application.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Categories.Queries.GetAllQuery
{
    public class GetAllCategoriesQueryParameter: RequestParameter<CategoryOrderKey>
    {
        public CategoryFilter? Filter { get; set; }

        public CategoryIncludes? Includes { get; set; }
    }

    public class CategoryFilter
    {
        public Guid? Id { get; set; }

        public Guid? ParentCategoryId { get; set; }

        public string? NameAr { get; set; }

        public string? NameEn { get; set; }

        public bool? IsVisible { get; set; }

        public string? Status { get; set; }
    }

    public class CategoryIncludes
    {
        public bool ParentCategory { get; set; }

        public bool Children { get; set; }

        public bool Products { get; set; }
    }

    public enum CategoryOrderKey
    {
        SortOrder,
        NameAr,
        NameEn,
        Status,
        CreatedAt,
        UpdatedAt
    }
}
