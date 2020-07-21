using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using NetCoreRedis.Api.Models;
using StackExchange.Redis;

namespace NetCoreRedis.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        ConnectionMultiplexer _redisConnection;

        public UsersController(IConfiguration config)
        {
            _redisConnection = ConnectionMultiplexer.Connect(config.GetConnectionString("RedisConnection"));
        }
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
                // Set a time to expires the cache data
                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
                options.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));

                cacheValue = JsonSerializer.Serialize(GenerateUsers());
                await cache.SetStringAsync("Users", cacheValue, options);
            }

            return Ok(cacheValue);
        }

        [HttpGet]
        [Route("GetAdminUsers/{id}")]
        public async Task<ActionResult> GetAdminUsers([FromRoute] int id)
        {
            var dbRedis = _redisConnection.GetDatabase();

            var usersList = await dbRedis.StringGetAsync("ADMIN-" + id.ToString());
            return Ok(usersList.ToString());
        }

        [HttpPost]
        public async Task<ActionResult> Post(
            [FromServices] IConfiguration config,
            IEnumerable<UserModel> users)
        {
            var dbRedis = _redisConnection.GetDatabase();
            foreach (var user in users)
            {
                await dbRedis.StringSetAsync("ADMIN-" + user.Id.ToString(), JsonSerializer.Serialize(user), expiry: null);
            }

            return CreatedAtAction("GetAdminUsers", null);
        }

        /// <summary>
        /// Generates a list of users
        /// </summary>
        /// <returns></returns>
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
