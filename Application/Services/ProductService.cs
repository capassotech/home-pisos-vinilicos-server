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
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<Product> GetByIdAsync(string id)
        {
            var productDto = await _productRepository.GetById(id);
            var product = _mapper.Map<Product>(productDto);
            return product;
        }

        public async Task<List<Product>> GetAllAsync(Expression<Func<Product, bool>>? filter = null)
        {
            var productDto = await _productRepository.GetAll(filter);
            var products = _mapper.Map<List<Product>>(productDto);
            return products;
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
