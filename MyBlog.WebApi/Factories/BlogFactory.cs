using Core;
using Core.Domain.Blogs;
using Microsoft.AspNetCore.Mvc;
using MyBlog.WebApi.DTOs.Blogs;
using MyBlog.WebApi.Infrastructure.Mapper.Extensions;
using Services.Blogs;
using Services.Helpers;
using Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace MyBlog.WebApi.Factories
{
    /// <summary>
    /// Represents the blog model factory
    /// </summary>
    public class BlogFactory : IBlogFactory
    {
        #region Fields
        private readonly IBlogService _blogService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IUserService _userService;
        #endregion

        #region Constructor
        public BlogFactory(IBlogService blogService,
            IDateTimeHelper dateTimeHelper,
            IUserService userService)
        {
            _blogService = blogService;
            _dateTimeHelper = dateTimeHelper;
            _userService = userService;
        }
        #endregion


        #region Utilities
        private BaseLinkInfo CreateLink(IUrlHelper urlHelper, string routeName, object values, string rel, string method)
        {
            return new BaseLinkInfo
            {
                Href = urlHelper.Link(routeName, values),
                Rel = rel,
                Method = method
            };
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prepare blog post response
        /// </summary>
        /// <param name="blogPostDto">Blog post dto</param>
        /// <param name="blogPost">Blog post entity</param>
        /// <param name="prepareComments">Whether to prepare blog comments</param>
        public virtual void PrepareBlogPostResponse(BlogPostDto blogPostDto, BlogPost blogPost, bool prepareComments)
        {
            if (blogPostDto == null)
                throw new ArgumentNullException(nameof(blogPostDto));

            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            blogPostDto = blogPost.ToDto(blogPostDto);
            blogPostDto.CreatedOn = _dateTimeHelper.ConvertToUserTime(blogPost.StartDateUtc ?? blogPost.CreatedOnUtc, DateTimeKind.Utc).ToString("D"); //TODO: Can be changed according to need
            blogPostDto.Tags = _blogService.ParseTags(blogPost);

            blogPostDto.NumberOfComments = _blogService.GetBlogCommentsCount(blogPost, true);

            if (prepareComments)
            {
                var blogComments = _blogService.GetAllComments(
                    blogPostId: blogPost.Id,
                    approved: true);

                if (blogComments.Any())
                    blogPostDto.Comments = new List<BlogCommentDto>();

                foreach (var bc in blogComments)
                {
                    var commentModel = PrepareBlogPostCommentModel(bc);
                    blogPostDto.Comments.Add(commentModel);
                }
            }
        }


        /// <summary>
        /// Prepare blog comment model
        /// </summary>
        /// <param name="blogComment">Blog comment entity</param>
        /// <returns>Blog comment dto</returns>
        public virtual BlogCommentDto PrepareBlogPostCommentModel(BlogComment blogComment)
        {
            if (blogComment == null)
                throw new ArgumentNullException(nameof(blogComment));

            var user = _userService.GetUserById(blogComment.CustomerId);

            var blogCommentDto = blogComment.ToDto<BlogCommentDto>();
            blogCommentDto.CustomerName = user?.Firstname + " " + user?.Lastname;
            blogCommentDto.CustomerAvatarUrl = user?.ProfileUrl;
            blogCommentDto.AllowViewingProfiles = false;
            blogCommentDto.CreatedOn = _dateTimeHelper.ConvertToUserTime(blogComment.CreatedOnUtc, DateTimeKind.Utc).ToString("g"); //TODO: Can be changed according to need

            return blogCommentDto;

        }


        /// <summary>
        /// Prepare blog post list dto
        /// </summary>
        /// <param name="urlHelper">UrlHelper</param>
        /// <param name="pagingDto">BlogPostForPagingDto</param>
        /// <returns>BlogPostListDto</returns>
        public virtual BlogPostListDto PrepareBlogPostListDto(IUrlHelper urlHelper, BlogPostForPagingDto pagingDto)
        {
            if (pagingDto == null)
                throw new ArgumentNullException(nameof(pagingDto));

            var dto = new BlogPostListDto();

            if (pagingDto.PageSize <= 0) pagingDto.PageSize = 10;
            if (pagingDto.PageNumber <= 0) pagingDto.PageNumber = 1;

            IPagedList<BlogPost> blogPosts = _blogService.GetAllBlogPosts(pagingDto.PageNumber - 1, pagingDto.PageSize);

            dto.BlogPostPagingContext.LoadPagedList(blogPosts);

            dto.BlogPosts = blogPosts
                .Select(x =>
                {
                    var blogPostDto = new BlogPostDto();
                    PrepareBlogPostResponse(blogPostDto, x, false);
                    blogPostDto.PostWithCommentsLink = CreateLink(urlHelper, "GetPostWithComments", new { blogPostDto.Id }, "postWithCommentsPage", HttpMethod.Get.ToString());
                    return blogPostDto;
                })
                .ToList();

            dto.Links.Add(CreateLink(urlHelper, "GetBlogPostAll",
                new BlogPostForPagingDto { PageNumber = dto.BlogPostPagingContext.PageNumber, PageSize = dto.BlogPostPagingContext.PageSize },
                "self",
               HttpMethod.Get.ToString()));

            if (dto.BlogPostPagingContext.HasPreviousPage)
                dto.Links.Add(CreateLink(urlHelper, "GetBlogPostAll",
                    new BlogPostForPagingDto { PageNumber = dto.BlogPostPagingContext.PreviousPageNumber, PageSize = dto.BlogPostPagingContext.PageSize },
                    "previousPage",
                    "GET"));

            if (dto.BlogPostPagingContext.HasNextPage)
                dto.Links.Add(CreateLink(urlHelper, "GetBlogPostAll",
                    new BlogPostForPagingDto { PageNumber = dto.BlogPostPagingContext.NextPageNumber, PageSize = dto.BlogPostPagingContext.PageSize },
                    "nextPage",
                    "GET"));

            return dto;
        }


        /// <summary>
        /// Prepare blog post seach dto
        /// </summary>
        /// <param name="urlHelper">UrlHelper</param>
        /// <param name="filteringDto">BlogPostForFilteringDto</param>
        /// <returns>BlogPostListDto</returns>
        public virtual BlogPostListDto PrepareBlogPostSearchDto(IUrlHelper urlHelper, BlogPostForFilteringDto filteringDto)
        {
            if (filteringDto == null)
                throw new ArgumentNullException(nameof(filteringDto));

            var dto = new BlogPostListDto();

            if (filteringDto.PageSize <= 0) filteringDto.PageSize = 10;
            if (filteringDto.PageNumber <= 0) filteringDto.PageNumber = 1;

            IPagedList<BlogPost> blogPosts = _blogService.GetAllBlogPostsByTag(filteringDto.Tag, filteringDto.PageNumber - 1, filteringDto.PageSize);

            dto.BlogPostPagingContext.LoadPagedList(blogPosts);

            dto.BlogPosts = blogPosts
                .Select(x =>
                {
                    var blogPostDto = new BlogPostDto();
                    PrepareBlogPostResponse(blogPostDto, x, false);
                    blogPostDto.PostWithCommentsLink = CreateLink(urlHelper, "GetPostWithComments", new { blogPostDto.Id }, "postWithCommentsPage", HttpMethod.Get.ToString());
                    return blogPostDto;
                })
                .ToList();

            dto.Links.Add(CreateLink(urlHelper, "GetBlogPostSearch",
                new BlogPostForPagingDto { PageNumber = dto.BlogPostPagingContext.PageNumber, PageSize = dto.BlogPostPagingContext.PageSize },
                "self",
               HttpMethod.Get.ToString()));

            if (dto.BlogPostPagingContext.HasPreviousPage)
                dto.Links.Add(CreateLink(urlHelper, "GetBlogPostSearch",
                    new BlogPostForPagingDto { PageNumber = dto.BlogPostPagingContext.PreviousPageNumber, PageSize = dto.BlogPostPagingContext.PageSize },
                    "previousPage",
                    "GET"));

            if (dto.BlogPostPagingContext.HasNextPage)
                dto.Links.Add(CreateLink(urlHelper, "GetBlogPostSearch",
                    new BlogPostForPagingDto { PageNumber = dto.BlogPostPagingContext.NextPageNumber, PageSize = dto.BlogPostPagingContext.PageSize },
                    "nextPage",
                    "GET"));

            return dto;
        }
        #endregion
    }
}
