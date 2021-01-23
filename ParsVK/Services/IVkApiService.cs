using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParsVK.Services
{
    public interface IVkApiService
    {
        string AccessToken { get; }
        Task GetTokenAsync(string code);
        Task<string> GetUsersAsync(string Id);
        Task<string> GetWallAsync(string ownerId);
        Task<string> GetLikesAsync(string ownerId, string itemId, string type);
    }
}
