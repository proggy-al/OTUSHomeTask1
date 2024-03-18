using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Otus.Teaching.PromoCodeFactory.Core.Abstractions.Repositories;
using Otus.Teaching.PromoCodeFactory.Core.Domain;

namespace Otus.Teaching.PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>
        : IRepository<T>
        where T: BaseEntity
    {
        protected IEnumerable<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data;
        }
        
        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task<T> CreateAsync(T item)
        {
            item.Id = Guid.NewGuid();
            Data = Data.Append(item);
            return Task.FromResult(item);
        }

        public Task<T> UpdateAsync(T item)
        {
            var updItem = Data.FirstOrDefault(x => x.Id == item.Id);
            if (updItem != null)
            {
                updItem = item;
            }
            return Task.FromResult(updItem);
        }

        public Task DeleteAsync(T entity)
        {
            Data = Data.Where(x => x.Id != entity.Id);
            return Task.CompletedTask;
        }
    }
}