using AutoMapper;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using System.Linq.Expressions;
using home_pisos_vinilicos_admin.Domain.Entities;
using home_pisos_vinilicos.Application.DTOs;
using home_pisos_vinilicos.Data.Repositories;
using home_pisos_vinilicos.Domain.Entities;

namespace home_pisos_vinilicos.Application.Services
{
    public class SubCategoryService
    {
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly IMapper _mapper;

        public SubCategoryService(ISubCategoryRepository subCategoryRepository, IMapper mapper)
        {
            _subCategoryRepository = subCategoryRepository;
            _mapper = mapper;
        }

        public async Task<SubCategory> GetByIdAsync(string id)
        {
            var subCategoryDto = await _subCategoryRepository.GetById(id);
            var subCategory = _mapper.Map<SubCategory>(subCategoryDto);
            return subCategory;
        }

        public async Task<List<SubCategory>> GetAllAsync(Expression<Func<SubCategory, bool>>? filter = null)
        {
            var subCategoryDto = await _subCategoryRepository.GetAll(filter);
            var subCategorys = _mapper.Map<List<SubCategory>>(subCategoryDto);
            return subCategorys;
        }

        public async Task<bool> DeleteAsync(string idSubCategory)
        {
            return await _subCategoryRepository.Delete(idSubCategory);
        }



        public async Task<bool> UpdateAsync(SubCategoryDto subCategoryDto)
        {
            var subCategory = _mapper.Map<SubCategory>(subCategoryDto);
            return await _subCategoryRepository.Update(subCategory);
        }


        public async Task<bool> SaveAsync(SubCategoryDto SubCategoryDto)
        {
            var SubCategory = _mapper.Map<SubCategory>(SubCategoryDto);
            var result = await _subCategoryRepository.Insert(SubCategory);
            return result;
        }


    }
}
