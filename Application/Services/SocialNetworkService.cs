using AutoMapper;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using System.Linq.Expressions;
using home_pisos_vinilicos_admin.Domain.Entities;
using home_pisos_vinilicos.Application.DTOs;
using home_pisos_vinilicos.Data.Repositories;
using home_pisos_vinilicos.Domain.Entities;
using System.Diagnostics.Contracts;

namespace home_pisos_vinilicos.Application.Services
{
    public class SocialNetworkService
    {
        private readonly ISocialNetworkRepository _socialNetworkRepository;
        private readonly IMapper _mapper;

        public SocialNetworkService(ISocialNetworkRepository socialNetworkRepository, IMapper mapper)
        {
            _socialNetworkRepository = socialNetworkRepository;
            _mapper = mapper;
        }

        public async Task<SocialNetwork> GetByIdAsync(string id)
        {
            var socialNetworkDto = await _socialNetworkRepository.GetById(id);
            var socialNetwork = _mapper.Map<SocialNetwork>(socialNetworkDto);
            return socialNetwork;
        }

        public async Task<List<SocialNetwork>> GetAllAsync(Expression<Func<SocialNetwork, bool>>? filter = null)
        {
            var socialNetworkDto = await _socialNetworkRepository.GetAll(filter);
            var socialNetworks = _mapper.Map<List<SocialNetwork>>(socialNetworkDto);
            return socialNetworks;
        }

        public async Task<bool> DeleteAsync(string idSocialNetwork)
        {
            return await _socialNetworkRepository.Delete(idSocialNetwork);
        }

        public async Task<bool> UpdateAsync(SocialNetworkDto socialNetworkDto)
        {
            var socialNetwork = _mapper.Map<SocialNetwork>(socialNetworkDto);
            return await _socialNetworkRepository.Update(socialNetwork);
        }

        public async Task<bool> SaveAsync(SocialNetworkDto socialNetworkDto)
        {
            var socialNetwork = _mapper.Map<SocialNetwork>(socialNetworkDto);
            var result = await _socialNetworkRepository.Insert(socialNetwork);
            return result;
        }

    }
}
