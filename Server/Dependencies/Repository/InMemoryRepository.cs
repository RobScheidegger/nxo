using NXO.Shared.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Dependencies
{
    public class InMemoryRepository<T> : IRepository<T>
    {
        private Dictionary<string, T> repository;
        public InMemoryRepository()
        {
            repository = new Dictionary<string, T>();
        }
        public async Task Add(string Key, T Value)
        {
            repository.Add(Key, Value);
        }

        public async Task<bool> Exists(string Key)
        {

            return Key == null ? false : repository.ContainsKey(Key);
        }

        public async Task<T> Find(string Key)
        {
            return repository[Key];
        }

        public async Task Remove(string Key)
        {
            repository.Remove(Key);
        }

        public async Task<T> Update(string Key, Action<T> Procedure)
        {
            T originalValue = await Find(Key);
            Procedure(originalValue);
            repository[Key] = originalValue;
            return await Find(Key);

        }
    }
}
