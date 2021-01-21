using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ParsVK.Models;
using ParsVK.Repositories;
using ParsVK.Services;

namespace ParsVK.Controllers
{
    [Route("api")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private IVkApiService _vkApiService;
        private IProfileRepository _profiles;
        public HomeController(IVkApiService vkApiService, IProfileRepository profiles)
        {
            _profiles = profiles;
            _vkApiService = vkApiService;
        }
        [Route("gettoken")]
        public async Task<IActionResult> GetTokenAsync(string code)
        {
            await _vkApiService.GetTokenAsync(code);
            return Ok();
        }

        [Route("parseprofile")]
        public async Task<IActionResult> ParseProfile(string link)
        {
            if (_vkApiService.AccessToken == "")
                return StatusCode(StatusCodes.Status500InternalServerError, "no access_token");
            string id = link.Split('/').Last();
            var json = await _vkApiService.GetUsersAsync(id);
            dynamic res = JsonConvert.DeserializeObject(json);
            Profile profile = new Profile
            {
                Id = res.response[0].id,
                FirstName = res.response[0].first_name,
                LastName = res.response[0].last_name,
                City = res.response[0].city.title,
                Bdate = res.response[0].bdate,
                PhotoUrl = res.response[0].photo_100,
                Audios = res.response[0].counters.audios,
                Friends = res.response[0].counters.friends,
                Groups = res.response[0].counters.groups,
                Photos = res.response[0].counters.photos
            };
            if (profile != null)
            {
                if (_profiles.GetByIdAsync(profile.Id) != null)
                    await _profiles.UpdateAsync(profile);
                else
                    await _profiles.CreateAsync(profile);
                return Ok(profile);
            }    
            return StatusCode(StatusCodes.Status400BadRequest);
        }
    }
}