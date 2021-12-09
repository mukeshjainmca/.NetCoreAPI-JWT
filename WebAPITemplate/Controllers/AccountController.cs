using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITemplate.Interface;
using WebAPITemplate.Models;

namespace WebAPITemplate.Controllers
{
    public class AccountController : ControllerBase, IAccountController
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [Authorize]
        [HttpPost]
        [Route("Register")]
        public IActionResult CreateUser([FromBody] UserRegistration user)
        {
            try
            {
                if (user != null)
                {
                    return Ok(_authService.RegisterUser(user));
                }
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
            return BadRequest(new { message = "Unable to register user" });
        }

        [Authorize]
        [HttpPost]
        [Route("AddClient")]
        public IActionResult AddClient([FromBody] ClientBase client)
        {
            try
            {
                if (client != null)
                {
                    return Ok(_authService.AddClient(client));
                }
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
            return BadRequest(new { message = "Unable to add client" });
        }
    }
}
