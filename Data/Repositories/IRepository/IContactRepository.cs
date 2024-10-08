﻿using home_pisos_vinilicos.Domain.Entities;
using home_pisos_vinilicos_admin.Domain;
using home_pisos_vinilicos_admin.Domain.Entities;
using System.Linq.Expressions;

namespace home_pisos_vinilicos.Data.Repositories.IRepository
{
    public interface IContactRepository : IRepository<Contact>
    {
        Task<List<Contact>> GetAll(Expression<Func<Contact, bool>>? filter = null);
    }
}
