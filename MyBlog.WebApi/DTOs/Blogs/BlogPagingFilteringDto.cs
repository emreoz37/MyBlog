using MyBlog.WebApi.Framework.DTOs.Paging;

namespace MyBlog.WebApi.DTOs.Blogs
{
    public class BlogPagingFilteringDto : BasePageableDto
    {
        public string Tag { get; set; }
    }
}
