using MyBlog.WebApi.Framework.DTOs;

namespace MyBlog.WebApi.DTOs.Blogs
{
    public class BlogCommentForCreationDto : BaseRequestDto
    {
        public int BlogPostId { get; set; }

        public int CustomerId { get; set; }

        public string CommentText { get; set; }
    }
}
