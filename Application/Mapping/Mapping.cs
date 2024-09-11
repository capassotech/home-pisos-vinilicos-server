using AutoMapper;
using home_pisos_vinilicos.Application.DTOs;
using home_pisos_vinilicos_admin.Domain.Entities;


namespace home_pisos_vinilicos.Application.Mapping
{
    public class Mapping : Profile
    {
        public Mapping()
        {

            CreateMap<Product, ProductDto>().ReverseMap();

        }
    }

}
