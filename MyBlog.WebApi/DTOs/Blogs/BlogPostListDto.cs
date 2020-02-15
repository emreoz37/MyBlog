using Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace MyBlog.WebApi.DTOs.Blogs
{
    public class BlogPostListDto
    {
        public BlogPostListDto()
        {
            BlogPosts = new List<BlogPostDto>();
            BlogPostPagingContext = new BlogPostPagingContext();
            Links = new List<BaseLinkInfo>();
        }

        [JsonIgnore]
        public BlogPostPagingContext BlogPostPagingContext { get; set; }
        public IList<BlogPostDto> BlogPosts { get; set; }
        public IList<BaseLinkInfo> Links { get; set; }
     
    }
}
