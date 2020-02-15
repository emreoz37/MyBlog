using MyBlog.WebApi.Framework.DTOs;

namespace MyBlog.WebApi.DTOs.Users
{
    /// <summary>
    /// User for authenticate dto
    /// </summary>
    public class UserForAuthenticateDto : BaseRequestDto
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}
