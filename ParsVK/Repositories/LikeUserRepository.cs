using Microsoft.EntityFrameworkCore;
using ParsVK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParsVK.Repositories
{
    public class LikeUserRepository : IRepository<LikeUser>
    {
        private AppDBContext _ctx;
        public LikeUserRepository(AppDBContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<LikeUser> CreateAsync(LikeUser item)
        {
            await _ctx.AddAsync(item);
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<List<LikeUser>> CreateAsync(List<LikeUser> items)
        {
            await _ctx.AddRangeAsync(items);
            await _ctx.SaveChangesAsync();
            return items;
        }

        public Task<List<LikeUser>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<LikeUser> GetByIdAsync(int id)
        {
            return await _ctx.LikeUsers.FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task<LikeUser> UpdateAsync(LikeUser item)
        {
            throw new NotImplementedException();
        }
    }
}
