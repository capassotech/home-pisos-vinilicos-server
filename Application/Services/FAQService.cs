using AutoMapper;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using System.Linq.Expressions;
using home_pisos_vinilicos_admin.Domain.Entities;
using home_pisos_vinilicos.Application.DTOs;
using home_pisos_vinilicos.Data.Repositories;
using home_pisos_vinilicos.Domain.Entities;

namespace home_pisos_vinilicos.Application.Services
{
    public class FAQService
    {
        private readonly IFAQRepository _FAQRepository;
        private readonly IMapper _mapper;

        public FAQService(IFAQRepository fAQRepository, IMapper mapper)
        {
            _FAQRepository = fAQRepository;
            _mapper = mapper;
        }

        public async Task<FAQ> GetByIdAsync(string id)
        {
            var fAQDto = await _FAQRepository.GetById(id);
            var fAQ = _mapper.Map<FAQ>(fAQDto);
            return fAQ;
        }

        public async Task<List<FAQ>> GetAllAsync(Expression<Func<FAQ, bool>>? filter = null)
        {
            var fAQDto = await _FAQRepository.GetAll(filter);
            var fAQs = _mapper.Map<List<FAQ>>(fAQDto);
            return fAQs;
        }

        public async Task<bool> DeleteAsync(string idFAQ)
        {
            return await _FAQRepository.Delete(idFAQ);
        }

        public async Task<bool> UpdateAsync(FAQDto fAQDto)
        {

            var fAQ = _mapper.Map<FAQ>(fAQDto);
            return await _FAQRepository.Update(fAQ);
        }

        public async Task<bool> SaveAsync(FAQDto fAQDto)
        {
            var fAQ = _mapper.Map<FAQ>(fAQDto);
            var result = await _FAQRepository.Insert(fAQ);
            return result;
        }

        
    }
}
