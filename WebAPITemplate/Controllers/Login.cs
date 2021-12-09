using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITemplate.Helpers;
using WebAPITemplate.Interface;
using WebAPITemplate.Models;

namespace WebAPITemplate.Controllers
{
    [Route("api/Login")]
    [ApiController]
    public class LoginController : Controller, ILoginController
    {
        private readonly IAuthService _authService;

        public LoginController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Token")]
        public IActionResult GetToken(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var identity = _authService.GetClaimsIdentity(user);

            if (identity == null)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid username or password.", ModelState));
            }

            var jwt = _authService.GenerateJwt(identity, user.UserName);
            return Ok(jwt);
        }
    }
}
