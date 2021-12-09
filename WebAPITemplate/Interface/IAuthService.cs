using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPITemplate.Models;

namespace WebAPITemplate.Interface
{
    public interface IAuthService
    {
        IdentityUser FindUser(User login);
        IdentityResult RegisterUser(UserRegistration userModel);
        int AddClient(ClientBase client);
        ClaimsIdentity GetClaimsIdentity(User user);
        JWTToken GenerateJwt(ClaimsIdentity identity, string userName);
    }
}
