using AutoMapper;
using home_pisos_vinilicos.Application.DTOs;
using home_pisos_vinilicos.Domain.Entities;
using home_pisos_vinilicos.Domain.Models;
using home_pisos_vinilicos_admin.Domain.Entities;


namespace home_pisos_vinilicos.Application.Mapping
{
    public class Mapping : Profile
    {
        public Mapping()
        {

            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<SubCategory, SubCategoryDto>().ReverseMap();
            CreateMap<LoginDto, Login>().ReverseMap();

        }
    }

}
