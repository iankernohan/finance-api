using System;
using AutoMapper;
using finance_api.Dtos;
using finance_api.Models;

namespace finance_api.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDtoResponse>().ReverseMap();
        CreateMap<Category, CategoryDtoRequest>().ReverseMap();
        CreateMap<SubCategory, SubCategoryDtoResponse>().ReverseMap();
        CreateMap<SubCategory, SubCategoryDtoRequest>().ReverseMap();
    }
}
