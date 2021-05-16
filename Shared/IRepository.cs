using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Shared.Repository
{
    public interface IRepository<T>
    {
        Task<T> Find(string Key);
        Task<bool> Exists(string Key);
        Task Remove(string Key);
        Task<T> Update(string Key, Action<T> Procedure);
        Task Add(string Key, T Value);
    }
}
