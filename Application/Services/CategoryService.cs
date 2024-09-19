﻿using AutoMapper;
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



        public async Task<bool> UpdateAsync(CategoryDto categoryDto)
        {
            if (categoryDto.IsFeatured)
            {
                var featuredCategorys = await _categoryRepository.GetAll(c => c.IsFeatured && c.IdCategory != categoryDto.IdCategory);
                if (featuredCategorys.Count >= 2)
                {
                    throw new InvalidOperationException("No se pueden destacar más de 2 categorias.");
                }
            }

            var category = _mapper.Map<Category>(categoryDto);
            return await _categoryRepository.Update(category);
        }


        public async Task<bool> SaveAsync(CategoryDto categoryDto)
        {
            if (categoryDto.IsFeatured)
            {
                var featureCategorys = await _categoryRepository.GetAll(c => c.IsFeatured);
                if (featureCategorys.Count >= 2)
                {
                    throw new InvalidOperationException("No se pueden destacar más de 2 categorías");
                }
            }

            var category = _mapper.Map<Category>(categoryDto);


            var result = await _categoryRepository.Insert(category);

            if (result)
            {
                await EnsureFeaturedCategorysAsync();
            }

            return result;
        }


        private async Task EnsureFeaturedCategorysAsync()
        {
            var featuredCategorys = await GetFeaturedCategorysAsync();

            if (NeedMoreFeaturedCategorys(featuredCategorys))
            {
                var productsToFeature = await GetCategorysToFeatureAsync(featuredCategorys.Count);
                await FeatureCategorysAsync(productsToFeature);
            }
        }


        private async Task<List<Category>> GetFeaturedCategorysAsync()
        {
            var allCategorys = await _categoryRepository.GetAll();
            return allCategorys
                .Where(c => c.IsFeatured)
                .ToList();
        }


        private async Task<List<Category>> GetCategorysToFeatureAsync(int currentFeaturedCount)
        {
            var allCategorys = await _categoryRepository.GetAll();

            return allCategorys
                .Where(c => !c.IsFeatured)  
                .Take(2 - currentFeaturedCount) 
                .ToList();
        }


        private bool NeedMoreFeaturedCategorys(List<Category> featuredCategorys)
        {
            return featuredCategorys.Count < 2;
        }

        private async Task FeatureCategorysAsync(List<Category> categorysToFeature)
        {
            foreach (var category in categorysToFeature)
            {
                category.IsFeatured = true;
                await _categoryRepository.Update(category);
            }
        }


    }
}
