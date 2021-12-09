using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPITemplate.Interface;
using WebAPITemplate.Models;

namespace WebAPITemplate.Helpers
{
    public class Tokens
    {
        public static JWTToken GenerateJwt(ClaimsIdentity identity, IJwtFactory jwtFactory, string userName, JwtIssuerOptions jwtOptions, JsonSerializerSettings serializerSettings)
        {
            var response = new JWTToken
            {
                Id = identity.Claims.Single(c => c.Type == "id").Value,
                Auth_Token = jwtFactory.GenerateEncodedToken(userName, identity).Result,
                Expires_in = (int)jwtOptions.ValidFor.TotalSeconds
            };

            return response;
            //return JsonConvert.SerializeObject(response, serializerSettings);
        }
    }
}
