using MyBlog.WebApi.Framework.DTOs;

namespace MyBlog.WebApi.DTOs.Users
{
    public class UserDto : BaseEntityDto
    {
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string UserName { get; set; }

        public string ProfileUrl { get; set; }

        public string Token { get; set; }
    }
}
