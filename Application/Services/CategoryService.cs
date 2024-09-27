using AutoMapper;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using System.Linq.Expressions;
using home_pisos_vinilicos_admin.Domain.Entities;
using home_pisos_vinilicos.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<List<CategoryDto>> GetAllAsync(Expression<Func<Category, bool>>? filter = null)
        {
            var categories = await _categoryRepository.GetAllWithSubCategories();
            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
            return categoryDtos;
        }

        public async Task<CategoryDto?> GetByIdAsync(string id)
        {
            var category = await _categoryRepository.GetByIdWithSubCategories(id);
            if (category == null)
                return null;

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return categoryDto;
        }

        public async Task<bool> DeleteAsync(string idCategory)
        {
            return await _categoryRepository.Delete(idCategory);
        }

        public async Task<bool> UpdateAsync(CategoryDto categoryDto)
        {
            await ValidateFeaturedCategoriesAsync(categoryDto.IdCategory, categoryDto.IsFeatured);
            var category = _mapper.Map<Category>(categoryDto);
            return await _categoryRepository.Update(category);
        }

        public async Task<bool> SaveAsync(CategoryDto categoryDto)
        {
            await ValidateFeaturedCategoriesAsync(null, categoryDto.IsFeatured);
            var category = _mapper.Map<Category>(categoryDto);
            var result = await _categoryRepository.Insert(category);

            if (result)
            {
                await EnsureFeaturedCategoriesAsync();
            }

            return result;
        }

        private async Task ValidateFeaturedCategoriesAsync(string? currentCategoryId, bool isFeatured)
        {
            if (isFeatured)
            {
                var featuredCategories = await _categoryRepository.GetAll(c => c.IsFeatured && (currentCategoryId == null || c.IdCategory != currentCategoryId));
                if (featuredCategories.Count >= 2)
                {
                    throw new InvalidOperationException("No se pueden destacar más de 2 categorías.");
                }
            }
        }

        private async Task EnsureFeaturedCategoriesAsync()
        {
            var featuredCategories = await GetFeaturedCategoriesAsync();
            if (NeedMoreFeaturedCategories(featuredCategories))
            {
                var categoriesToFeature = await GetCategoriesToFeatureAsync(featuredCategories.Count);
                await FeatureCategoriesAsync(categoriesToFeature);
            }
        }

        private async Task<List<Category>> GetFeaturedCategoriesAsync()
        {
            var allCategories = await _categoryRepository.GetAll();
            return allCategories.Where(c => c.IsFeatured).ToList();
        }

        private async Task<List<Category>> GetCategoriesToFeatureAsync(int currentFeaturedCount)
        {
            var allCategories = await _categoryRepository.GetAll();
            return allCategories
                .Where(c => !c.IsFeatured)
                .Take(2 - currentFeaturedCount)
                .ToList();
        }

        private bool NeedMoreFeaturedCategories(List<Category> featuredCategories)
        {
            return featuredCategories.Count < 2;
        }

        private async Task FeatureCategoriesAsync(List<Category> categoriesToFeature)
        {
            foreach (var category in categoriesToFeature)
            {
                category.IsFeatured = true;
                await _categoryRepository.Update(category);
            }
        }
    }
}
