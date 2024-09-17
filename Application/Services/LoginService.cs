using AutoMapper;
using FirebaseAdmin.Auth;
using home_pisos_vinilicos.Application.DTOs;
using home_pisos_vinilicos.Data.Repositories;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Models;
using System.Linq.Expressions;

namespace home_pisos_vinilicos.Application.Services
{
    public class LoginService
    {
        private readonly ILoginRepository _loginRepository;
        private readonly IMapper _mapper;

        public LoginService(ILoginRepository loginRepository, IMapper mapper)
        {
            _loginRepository = loginRepository;
            _mapper = mapper;
        }

        public async Task<List<Login>> GetAllAsync(Expression<Func<Login, bool>>? filter = null)
        {
            var loginDto = await _loginRepository.GetAll(filter);
            var logins = _mapper.Map<List<Login>>(loginDto);
            return logins;
        }


        public async Task<UserRecord> GetUserByEmailAsync(string email)
        {
            try
            {
                return await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
            }
            catch (FirebaseAuthException ex)
            {
                throw new Exception($"Error al obtener usuario: {ex.Message}", ex);
            }
        }



    }
}
