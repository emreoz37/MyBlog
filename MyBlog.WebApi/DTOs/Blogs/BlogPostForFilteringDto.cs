using MyBlog.WebApi.Framework.DTOs;

namespace MyBlog.WebApi.DTOs.Blogs
{
    public class BlogPostForFilteringDto : BasePagingQueryStringParameters
    {
        public string Tag { get; set; }
    }
}
