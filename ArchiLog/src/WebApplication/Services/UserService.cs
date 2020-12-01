using APILibrary.core.Attributes;
using APILibrary.core.Helpers;
using APILibrary.core.Interfaces;
using APILibrary.core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class UserService : IAuthenticationService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<User> _users = new List<User>
        {
            new User { ID = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test" }
        };

        public UserService(IOptions<AppSettings> appSettings) : base(appSettings.Value)
        {
        }

        override public object Authentication(AuthenticateRequest model)
        {
            var user = _users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);
            return this.Authenticate(user);
        }

        override public UserBase GetById(int id)
        {
            var user = _users.FirstOrDefault(x => x.ID == id);
            return user;
        }
    }
}