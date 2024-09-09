using AutoMapper;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using System.Linq.Expressions;
using home_pisos_vinilicos.Domain;
using home_pisos_vinilicos.Shared.DTOs;

namespace home_pisos_vinilicos.Application
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

        public async Task<bool> SaveAsync(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            return await _productRepository.Insert(product);
        }

        public async Task<bool> UpdateAsync(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            return await _productRepository.Update(product);
        }
    }
}

