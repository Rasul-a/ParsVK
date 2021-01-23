using ParsVK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParsVK.Repositories
{
    public interface IProfileRepository
    {
        Task<Profile> GetByIdAsync(string id);
        Task<List<Profile>> GetAllAsync();
        Task<Profile> CreateAsync(Profile profile);
        Task<Profile> UpdateAsync(Profile profile);
        Task<Profile> Delete(string id);
    }
}
