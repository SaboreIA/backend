using Microsoft.AspNetCore.Mvc;
using SaboreIA.Database;
using System.Diagnostics;

namespace SaboreIA.Controllers
{
    [ApiController]
    [Route("api")]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;

        public HealthController(IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        [HttpGet("health")]
        public async Task<IActionResult> Health()
        {
            var health = new
            {
                status = "ok",
                timestamp = DateTime.UtcNow,
                uptime = GetUptime(),
                version = "1.0.0",
                checks = new
                {
                    database = await CheckDatabase()
                }
            };

            return Ok(health);
        }

        private async Task<string> CheckDatabase()
        {
            try
            {
                await _dbContext.Database.CanConnectAsync();
                return "healthy";
            }
            catch
            {
                return "unhealthy";
            }
        }

        private TimeSpan GetUptime()
        {
            return DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();
        }
    }
}
