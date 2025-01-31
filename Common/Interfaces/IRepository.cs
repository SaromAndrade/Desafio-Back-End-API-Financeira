using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        // Obtém uma entidade pelo ID
        Task<T> GetByIdAsync(int id);

        // Obtém todas as entidades
        Task<IEnumerable<T>> GetAllAsync();

        // Obtém todas as entidades de acordo com a Expression
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null);

        // Adiciona uma entidade
        Task AddAsync(T entity);

        // Atualiza uma entidade
        void Update(T entity);

        // Remove uma entidade
        void Delete(T entity);
    }
}
