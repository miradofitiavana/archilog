using APILibrary.core.Attributes;
using APILibrary.core.Helpers;
using APILibrary.core.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace APILibrary.core.Interfaces
{
    public abstract class IAuthenticationService
    {
        private readonly AppSettings _appSettings;
        public abstract UserBase GetById(int id);
        public abstract object Authentication(AuthenticateRequest model);

        public IAuthenticationService(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        //   object Authenticate(AuthenticateRequest model);
        protected object Authenticate(UserBase user)
        {
            //var user = _users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);

            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt token
            var token = GenerateJwtToken(user);

            var expandoDict = new ExpandoObject() as IDictionary<string, object>;
            var collectionType = typeof(UserBase);
            foreach (var prop in collectionType.GetProperties())
            {
                var isPresentAttribute = prop.CustomAttributes.
                   Any(x => x.AttributeType == typeof(AuthenticateResponseAttribute));
                if (isPresentAttribute)
                    expandoDict.Add(prop.Name, prop.GetValue(user));
            }
            expandoDict.Add("token", token);

            return expandoDict;
        }

        private string GenerateJwtToken(UserBase user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.ID.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}