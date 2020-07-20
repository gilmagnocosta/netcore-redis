using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using NetCoreRedis.Api.Models;

namespace NetCoreRedis.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        /// <summary>
        /// Gets a list of users stored in cache or create a new one
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Get([FromServices] IDistributedCache cache)
        {
            var cacheValue = await cache.GetStringAsync("Users");
            
            if (cacheValue == null)
            {
                cacheValue = JsonSerializer.Serialize(GenerateUsers());
                await cache.SetStringAsync("Users", cacheValue);
            }

            return Ok(cacheValue);
        }

        private List<UserModel> GenerateUsers()
        {
            return new List<UserModel>
            {
                new UserModel() { Id = 1, Name = "Test 1"},
                new UserModel() { Id = 2, Name = "Test 2"},
                new UserModel() { Id = 3, Name = "Test 3"},
                new UserModel() { Id = 4, Name = "Test 4"},
                new UserModel() { Id = 5, Name = "Test 5"},
                new UserModel() { Id = 6, Name = "Test 6"},
                new UserModel() { Id = 7, Name = "Test 7"},
                new UserModel() { Id = 8, Name = "Test 8"},
                new UserModel() { Id = 9, Name = "Test 9"},
                new UserModel() { Id = 10, Name = "Test 10"}
            };
        }
    }
}
