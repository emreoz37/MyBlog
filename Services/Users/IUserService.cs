using Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Users
{
    /// <summary>
    /// User service interface
    /// </summary>
    public partial interface IUserService
    {
        /// <summary>
        /// Authenticate 
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>User</returns>
        User Authenticate(string username, string password);

        /// <summary>
        /// Get user by identifier
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>User</returns>
        User GetUserById(int userId);
    }
}
