using MyBlog.WebApi.Framework.DTOs;
using System;

namespace MyBlog.WebApi.DTOs.Blogs
{
    public partial class BlogPostForCreationDto : BaseRequestDto
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public string BodyOverview { get; set; }

        public bool AllowComments { get; set; }

        public int NumberOfComments { get; set; }

        public DateTime? StartDateUtc { get; set; }

        public DateTime? EndDateUtc { get; set; }

        public string Tags { get; set; }

    }
}
