using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watsons.Common
{
    public interface IRepository<T>
    {
        public Task<T> Insert(T entity);

        public Task<T> Update(T entity);

        public Task<bool> Delete(T entity);

        public Task<T> Select(T entity);

        public Task<IEnumerable<T>> List(T entity);
    }
}
