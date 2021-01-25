using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParsVK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParsVK.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private AppDBContext _ctx;
        public ProfileRepository(AppDBContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<Profile> CreateAsync(Profile profile)
        {
            _ctx.Add(profile);
            await _ctx.SaveChangesAsync();
            return profile;
        }

        public async Task<Profile> Delete(string id)
        {

            var profile = await _ctx.Profiles.Include(p => p.WallItems).Include(p => p.LikeUsers).FirstOrDefaultAsync(p => p.Id == id);
            if (profile != null)
            {
                
                _ctx.Profiles.Remove(profile);
                await _ctx.SaveChangesAsync();
            }
                
            return profile;
        }

        public async Task<List<Profile>> GetAllAsync()
        {
            return await _ctx.Profiles.ToListAsync();
        }

        public async Task<Profile> GetByIdAsync(string id)
        {
            return await _ctx.Profiles.Include(p=>p.WallItems).Include(p=>p.LikeUsers).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Profile> UpdateAsync(Profile profile)
        {
            _ctx.Update(profile);
            await _ctx.SaveChangesAsync();
            return profile;
        }
    }
}
