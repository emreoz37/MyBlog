using Core.Domain.Users;
using Microsoft.Extensions.Options;
using Services.Helpers;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using System;

namespace Services.Users
{
    public class UserService : IUserService
    {
        #region Fields
        private readonly AppSettings _appSettings;
        #endregion

        #region Utilities
        private List<User> _users = new List<User>
        {
            new User { Id = 1, Firstname = "Emre", Lastname = "Öz", UserName = "emreoz37", ProfileUrl = "https://github.com/emreoz37", Password = "1234" },
            new User { Id = 2, Firstname = "Zişan", Lastname = "Öz", UserName = "zisan37", ProfileUrl = "https://www.linkedin.com/in/emreoz37/", Password = "1234" }
        };

        #endregion

        #region Constructor
        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Authenticate 
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>User</returns>
        public virtual User Authenticate(string username, string password)
        {
            var user = _users.FirstOrDefault(x => x.UserName == username && x.Password == password);

            if (user == null)
                return null;

            // If Authentication is successful, a JWT token is generated.
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return user;
        }

        /// <summary>
        /// Get user by identifier
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>User</returns>
        public virtual User GetUserById(int userId)
        {
            if (userId == 0)
                return null;

            var user = _users.FirstOrDefault(x => x.Id == userId);

            return user;
        }
        #endregion
    }
}
