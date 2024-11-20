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
            // Mapeo de ProductDto a Product
            CreateMap<ProductDto, Product>()
                .ForMember(dest => dest.IdProduct, opt => opt.MapFrom(src => src.IdProduct)) // Mapeo correcto de IdProduct
                .ForMember(dest => dest.IdCategory, opt => opt.MapFrom(src => src.IdCategory))// Mapeo de la relación IdCategory
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))   // Mapeo de la relación Category
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls))   // Mapeo correcto de ImageUrl
                .ForMember(dest => dest.TechnicalSheet, opt => opt.MapFrom(src => src.TechnicalSheet)) // Mapeo de ficha técnica
                .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => src.Colors)); // Mapeo de lista de colores

            // Mapeo de Product a ProductDto
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.IdCategory, opt => opt.MapFrom(src => src.IdCategory))// Mapeo de la relación IdCategory
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))   // Mapeo de la relación Category
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls))   // Mapeo correcto de ImageUrl
                .ForMember(dest => dest.TechnicalSheet, opt => opt.MapFrom(src => src.TechnicalSheet)) // Mapeo de ficha técnica
                .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => src.Colors)); // Mapeo de lista de colores

            // Mapeo de Category a CategoryDto y viceversa
            CreateMap<Category, CategoryDto>().ReverseMap()
                .ForMember(dest => dest.IdCategory, opt => opt.MapFrom(src => src.IdCategory))
                .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories));

            // Mapeos de otras entidades y DTOs
            CreateMap<FAQ, FAQDto>().ReverseMap();
            CreateMap<Contact, ContactDto>().ReverseMap();
            CreateMap<SocialNetwork, SocialNetworkDto>().ReverseMap();
            CreateMap<LoginDto, Login>().ReverseMap();
        }
    }
}