using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace microservice_sketch.Permission
{
    public class JwtToken
    {
        public static dynamic BuildJwtToken(Claim[] claims, PermissionRequirement permissionRequirement)
        {
            var jwt = new JwtSecurityToken(
                issuer: permissionRequirement.Issuer,
                audience: permissionRequirement.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.Add(permissionRequirement.Expiration),
                signingCredentials: permissionRequirement.SigningCredentials
                );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new
            {
                status = true,
                access_token = encodedJwt,
                expires_in = permissionRequirement.Expiration.TotalMilliseconds,
                token_type = "Bearer"
            };
        }

        public static dynamic ReadToken(string jwt_input) {
            var handler = new JwtSecurityTokenHandler();
            var jwt_output = string.Empty;


            var token = handler.ReadJwtToken(jwt_input);
            var jwtHeader = JsonConvert.SerializeObject(token.Header.Select(h => new { h.Key, h.Value }));
            jwt_output = $"{{\r\n\"Header\":\r\n{JToken.Parse(jwtHeader)},";

            // Re-serialize the Token Claims to just Type and Values
            var jwtPayload = JsonConvert.SerializeObject(token.Claims.Select(c => new { c.Type, c.Value }));
            jwt_output += $"\r\n\"Payload\":\r\n{JToken.Parse(jwtPayload)}\r\n}}";


            //return handler.ReadJwtToken(token);
            return JToken.Parse(jwt_output).ToString(Formatting.Indented);
        }

        public static async Task<string> ReadTokenAsync(string jwtInput)
        {
            return await Task.Run(() =>
            {
                return ReadToken(jwtInput);
            });
        }

        //public static ClaimsPrincipal ValidateToken(string jwtToken)
        //{
        //    IdentityModelEventSource.ShowPII = true;

        //    SecurityToken validatedToken;
        //    TokenValidationParameters validationParameters = new TokenValidationParameters();

        //    validationParameters.ValidateLifetime = true;

        //    validationParameters.ValidAudience = _audience.ToLower();
        //    validationParameters.ValidIssuer = _issuer.ToLower();
        //    validationParameters.IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret));

        //    ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);


        //    return principal;
        //}

    }
}
