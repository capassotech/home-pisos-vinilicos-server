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
            var result = _mapper.Map<List<ProductDto>>(products);
            return result;
        }

        public async Task<ProductDto?> GetByIdAsync(string id)
        {
            var product = await _productRepository.GetByIdWithCategory(id);
            if (product == null) return null;

            if (!string.IsNullOrEmpty(product.IdCategory))
            {
                product.Category = await _categoryRepository.GetById(product.IdCategory);
            }
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> DeleteAsync(string idProduct)
        {
            return await _productRepository.Delete(idProduct);
        }

        public async Task<ProductDto> UpdateAsync(ProductDto productDto, Stream? imageStream = null)
        {
            await EnsureFeaturedProductLimitNotExceeded(productDto);

            var product = _mapper.Map<Product>(productDto);
            var success = await _productRepository.Update(product, imageStream);

            if (!success)
            {
                throw new Exception("Failed to update product");
            }

            var updatedProduct = await _productRepository.GetByIdWithCategory(product.IdProduct);
            return _mapper.Map<ProductDto>(updatedProduct);
        }

        public async Task<ProductDto> SaveAsync(ProductDto productDto, Stream? imageStream = null)
        {
            await EnsureFeaturedProductLimitNotExceeded(productDto);

            var product = _mapper.Map<Product>(productDto);
            product.CreatedDate = DateTime.UtcNow;

            var success = await _productRepository.Insert(product, imageStream);

            if (!success)
            {
                throw new Exception("Failed to save product");
            }

            await EnsureFeaturedProductsAsync();

            var savedProduct = await _productRepository.GetByIdWithCategory(product.IdProduct);
            return _mapper.Map<ProductDto>(savedProduct);
        }

        public async Task<List<ProductDto>> SearchAsync(string query)
        {
            Expression<Func<Product, bool>> filter = p =>
                p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(query, StringComparison.OrdinalIgnoreCase);

            var products = await _productRepository.GetAll(filter);
            return _mapper.Map<List<ProductDto>>(products);
        }

        private async Task EnsureFeaturedProductLimitNotExceeded(ProductDto productDto)
        {
            if (productDto.IsFeatured)
            {
                var featuredProducts = await _productRepository.GetAll(p => p.IsFeatured && p.IdProduct != productDto.IdProduct);
                if (featuredProducts.Count >= 6)
                {
                    throw new InvalidOperationException("No se pueden destacar más de 6 productos.");
                }
            }
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