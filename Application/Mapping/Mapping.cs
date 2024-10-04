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

            CreateMap<ProductDto, Product>()
                .ForMember(dest => dest.IdProduct, opt => opt.MapFrom(src => src.ImageUrl));
            CreateMap<Product, ProductDto>();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<FAQ, FAQDto>().ReverseMap();
            CreateMap<Contact, ContactDto>().ReverseMap();
            CreateMap<SocialNetwork, SocialNetworkDto>().ReverseMap();
            CreateMap<LoginDto, Login>().ReverseMap();

        }
    }

}
