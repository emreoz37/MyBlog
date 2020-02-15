using MyBlog.WebApi.Framework.DTOs;
using System;

namespace MyBlog.WebApi.DTOs.Blogs
{
    //TODO: you can create an additional abstract class, extract properties to it and then just force these classes to inherit from the abstract class
    public partial class BlogPostForUpdateDto : BaseRequestDto
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
