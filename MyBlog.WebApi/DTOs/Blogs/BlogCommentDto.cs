using MyBlog.WebApi.Framework.DTOs;

namespace MyBlog.WebApi.DTOs.Blogs
{
    public class BlogCommentDto : BaseEntityDto
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerAvatarUrl { get; set; }

        public string CommentText { get; set; }

        public string CreatedOn { get; set; }

        public bool AllowViewingProfiles { get; set; }
    }
}
