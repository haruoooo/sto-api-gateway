using Microsoft.AspNetCore.Mvc;
using static sto_api_gateway.src.modules.users.UsersDto;

namespace sto_api_gateway.src.modules.users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _usersService;

        public UsersController(UsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost("user")]
        public IActionResult GetUser([FromBody] UserRequest request)
        {
            try
            {
                var users = _usersService.GetUser(request);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}