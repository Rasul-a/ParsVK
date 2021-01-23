using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParsVK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParsVK.Repositories
{
    public interface IRepository<T> where T: class
    {
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<T> CreateAsync(T item);
        Task<List<T>> CreateAsync(List<T> items);
        Task<T> UpdateAsync(T item);
        
    }
}
