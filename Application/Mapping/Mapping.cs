using AutoMapper;
using home_pisos_vinilicos.Shared.DTOs;
using home_pisos_vinilicos.Domain;

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
