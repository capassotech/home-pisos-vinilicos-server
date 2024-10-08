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
    public class ContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;

        public ContactService(IContactRepository contactRepository, IMapper mapper)
        {
            _contactRepository = contactRepository;
            _mapper = mapper;
        }

        public async Task<Contact> GetByIdAsync(string id)
        {
            var contactDto = await _contactRepository.GetById(id);
            var contact = _mapper.Map<Contact>(contactDto);
            return contact;
        }

        public async Task<List<Contact>> GetAllAsync(Expression<Func<Contact, bool>>? filter = null)
        {
            var contactDto = await _contactRepository.GetAll(filter);
            var contacts = _mapper.Map<List<Contact>>(contactDto);
            return contacts;
        }

        public async Task<bool> DeleteAsync(string idContact)
        {
            return await _contactRepository.Delete(idContact);
        }

        public async Task<bool> UpdateAsync(ContactDto contactDto)
        {
            var contact = _mapper.Map<Contact>(contactDto);
            return await _contactRepository.Update(contact);
        }

        public async Task<bool> SaveAsync(ContactDto contactDto)
        {
            var contact = _mapper.Map<Contact>(contactDto);
            var result = await _contactRepository.Insert(contact);
            return result;
        }

    }
}
