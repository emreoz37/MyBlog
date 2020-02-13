using MyBlog.WebApi.Framework.DTOs;

namespace MyBlog.WebApi.DTOs.Blogs
{
    public class BlogCommentForUpdateDto : BaseRequestDto
    {
        public string CommentText { get; set; }
    }
}
