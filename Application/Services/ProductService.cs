using AutoMapper;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using System.Linq.Expressions;
using home_pisos_vinilicos_admin.Domain.Entities;
using home_pisos_vinilicos.Application.DTOs;

namespace home_pisos_vinilicos.Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<ProductDto>> GetAllAsync(Expression<Func<Product, bool>>? filter = null)
        {
            var products = await _productRepository.GetAllWithCategories();
            var productDtos = _mapper.Map<List<ProductDto>>(products);
            foreach (var productDto in productDtos)
            {
                var category = await _categoryRepository.GetById(productDto.IdCategory);
                productDto.Category = _mapper.Map<CategoryDto>(category);
            }
            return productDtos;
        }


        public async Task<ProductDto> GetByIdAsync(string id)
        {
            var product = await _productRepository.GetByIdWithCategory(id);
            if (product != null && !string.IsNullOrEmpty(product.IdCategory))
            {
                product.Category = await _categoryRepository.GetById(product.IdCategory);
            }
            var productDto = _mapper.Map<ProductDto>(product);
            return productDto;
        }


        public async Task<bool> DeleteAsync(string idProduct)
        {
            return await _productRepository.Delete(idProduct);
        }

        


        public async Task<bool> UpdateAsync(ProductDto productDto)
        {
            if (productDto.IsFeatured)
            {
                var featuredProducts = await _productRepository.GetAll(p => p.IsFeatured && p.IdProduct != productDto.IdProduct);
                if (featuredProducts.Count >= 6)
                {
                    throw new InvalidOperationException("No se pueden destacar más de 6 productos.");
                }
            }

            var product = _mapper.Map<Product>(productDto);
            return await _productRepository.Update(product);
        }

        public async Task<bool> SaveAsync(ProductDto productDto)
        {
            if (productDto.IsFeatured)
            {
                var featuredProducts = await _productRepository.GetAll(p => p.IsFeatured);
                if (featuredProducts.Count >= 6)
                {
                    throw new InvalidOperationException("No se pueden destacar más de 6 productos.");
                }
            }

            var product = _mapper.Map<Product>(productDto);

            product.CreatedDate = DateTime.UtcNow;

            var result = await _productRepository.Insert(product);

            if (result)
            {
                await EnsureFeaturedProductsAsync();
            }

            return result;
        }

        public async Task<List<ProductDto>> SearchAsync(string query)
        {
            Expression<Func<Product, bool>> filter = p =>
                p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(query, StringComparison.OrdinalIgnoreCase);

            var products = await _productRepository.GetAll(filter);

            var productDtos = _mapper.Map<List<ProductDto>>(products);

            return productDtos;
        }


        private async Task EnsureFeaturedProductsAsync()
        {
            var featuredProducts = await GetFeaturedProductsAsync();

            if (NeedMoreFeaturedProducts(featuredProducts))
            {
                var productsToFeature = await GetProductsToFeatureAsync(featuredProducts.Count);
                await FeatureProductsAsync(productsToFeature);
            }
        }

        private async Task<List<Product>> GetFeaturedProductsAsync()
        {
            var allProducts = await _productRepository.GetAll();
            return allProducts
                .OrderByDescending(p => p.CreatedDate)
                .Where(p => p.IsFeatured)
                .ToList();
        }

        private bool NeedMoreFeaturedProducts(List<Product> featuredProducts)
        {
            return featuredProducts.Count < 6;
        }

        private async Task<List<Product>> GetProductsToFeatureAsync(int currentFeaturedCount)
        {
            var allProducts = await _productRepository.GetAll();

            return allProducts
                .OrderByDescending(p => p.CreatedDate)
                .Where(p => !p.IsFeatured)  
                .Take(6 - currentFeaturedCount)  
                .ToList();
        }

        private async Task FeatureProductsAsync(List<Product> productsToFeature)
        {
            foreach (var product in productsToFeature)
            {
                product.IsFeatured = true;
                await _productRepository.Update(product);
            }
        }
    }
}
