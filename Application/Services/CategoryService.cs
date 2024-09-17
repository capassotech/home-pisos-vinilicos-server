using AutoMapper;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using System.Linq.Expressions;
using home_pisos_vinilicos_admin.Domain.Entities;
using home_pisos_vinilicos.Application.DTOs;
using home_pisos_vinilicos.Data.Repositories;
using home_pisos_vinilicos.Domain.Entities;

namespace home_pisos_vinilicos.Application.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<Category> GetByIdAsync(string id)
        {
            var categoryDto = await _categoryRepository.GetById(id);
            var category = _mapper.Map<Category>(categoryDto);
            return category;
        }

        public async Task<List<Category>> GetAllAsync(Expression<Func<Category, bool>>? filter = null)
        {
            var categoryDto = await _categoryRepository.GetAll(filter);
            var categorys = _mapper.Map<List<Category>>(categoryDto);
            return categorys;
        }

        public async Task<bool> DeleteAsync(string idCategory)
        {
            return await _categoryRepository.Delete(idCategory);
        }


        public async Task<bool> SaveAsync(CategoryDto categoryDto)
        {

            var category = _mapper.Map<Category>(categoryDto);

            var result = await _categoryRepository.Insert(category);

            return result;
        }

        



        


    }
}
