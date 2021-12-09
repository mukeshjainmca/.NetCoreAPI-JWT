using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using WebAPITemplate.Entities;
using WebAPITemplate.Helpers;
using WebAPITemplate.Interface;
using WebAPITemplate.Models;

namespace WebAPITemplate.Services
{
    public class AuthService :IAuthService
    {
        private readonly Logger _logger = null;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly UserManager<IdentityUser> _userManager;
        public AuthService(UserManager<IdentityUser> userManager, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _userManager = userManager;
        }

        public IdentityResult RegisterUser(UserRegistration userModel)
        {
            _logger.Trace("Enter AuthService.RegisterUser");
            IdentityResult identityResult = null;
            try
            {
                if (userModel.UserName == null)
                    throw new Exception("Invalid Username");
                if (userModel.Password == null || userModel.ConfirmPassword == null || userModel.Password != userModel.ConfirmPassword)
                    throw new Exception("Invalid Password");
                IdentityUser user = new IdentityUser
                {
                    UserName = userModel.UserName,
                    PasswordHash = GetHash(userModel.Password)
                };
                using (var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(new AuthContext(new DbContextOptionsBuilder<AuthContext>().Options)), null, null, null, null, null, null, null, null))
                {
                    identityResult = userManager.CreateAsync(user).Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in AuthService.RegisterUser = " + ex.Message + " trace=" + ex.StackTrace);
                throw new Exception("User Registration failed", ex.InnerException);
            }
            return identityResult;
        }

        public int AddClient(ClientBase clientbase)
        {
            _logger.Trace("Enter AuthService.AddClient");
            try
            {
                if (clientbase.Id == null)
                    throw new Exception("Invalid Client Id");
                if (clientbase.Secret == null || clientbase.Name == null)
                    throw new Exception("Invalid Cleint details");

                Client client = new Client
                {
                    Id = clientbase.Id,
                    Name = clientbase.Name,
                    Secret = GetHash(clientbase.Secret),
                    ApplicationType = ApplicationTypes.NativeConfidential,
                    RefreshTokenLifeTime = 14400,
                    AllowedOrigin = "*",
                    Active = true
                };

                using (var context = new AuthContext(new DbContextOptionsBuilder<AuthContext>().Options))
                {
                    var result = context.Clients.Add(client);
                    return context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in AuthService.AddClient = " + ex.Message + " trace=" + ex.StackTrace);
                throw new Exception("Add Client failed", ex.InnerException);
            }
        }

        public ClaimsIdentity GetClaimsIdentity(User user)
        {
            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
                return Task.FromResult<ClaimsIdentity>(null).Result;

            //verify client
            var client = FindClient(user.ClientId);
            if (client != null && client.Secret == GetHash(user.ClientSecret))
            {
                IdentityUser userToVerify = null;
                //verify user
                lock (_userManager)
                {
                    userToVerify = _userManager.FindByNameAsync(user.UserName).Result;
                }
                if (userToVerify == null) return Task.FromResult<ClaimsIdentity>(null).Result;

                //verify credentials
                if (GetHash(user.Password) == userToVerify.PasswordHash)
                {
                    return Task.FromResult(_jwtFactory.GenerateClaimsIdentity(user.UserName, userToVerify.Id)).Result;
                }
            }

            // Client or Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null).Result;

        }

        public JWTToken GenerateJwt(ClaimsIdentity identity, string userName)
        {
            return Tokens.GenerateJwt(identity, _jwtFactory, userName, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        public IdentityUser FindUser(User login)
        {
            _logger.Trace("Enter AuthService.FindUser");
            IdentityUser identityUser = null;
            try
            {
                var client = FindClient(login.ClientId);
                if (client != null && client.Secret == GetHash(login.ClientSecret))
                    lock (_userManager)
                    {
                        identityUser = _userManager.FindByNameAsync(login.UserName).Result;
                    }

                if (identityUser != null && identityUser.PasswordHash != GetHash(login.Password))
                    identityUser = null;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in AuthService.FindUser = " + ex.Message + " trace=" + ex.StackTrace);
                throw;
            }
            _logger.Trace("Exit AuthService.FindUser");
            return identityUser;
        }

        public Client FindClient(string clientId)
        {
            _logger.Trace("Enter AuthRepository.FindClient");
            Client client = null;
            try
            {
                using (var context = new AuthContext(new DbContextOptionsBuilder<AuthContext>().Options))
                {
                    client = context.Clients.Find(clientId);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in AuthRepository.FindClient = " + ex.Message + " trace=" + ex.StackTrace);
                throw;
            }
            _logger.Trace("Exit AuthRepository.FindClient");
            return client;
        }

        private string GetHash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }
    }
}
