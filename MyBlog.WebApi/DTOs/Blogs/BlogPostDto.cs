using MyBlog.WebApi.Framework.DTOs;
using System;
using System.Collections.Generic;

namespace MyBlog.WebApi.DTOs.Blogs
{
    public class BlogPostDto : BaseEntityDto
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public string BodyOverview { get; set; }

        public bool AllowComments { get; set; }

        public int NumberOfComments { get; set; }

        public DateTime? StartDateUtc { get; set; }

        public DateTime? EndDateUtc { get; set; }

        public string CreatedOn { get; set; }

        public IList<string> Tags { get; set; }

        public IList<BlogCommentDto> Comments { get; set; }
    }
}
