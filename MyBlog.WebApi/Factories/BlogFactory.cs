using Core.Caching;
using Core.Domain.Blogs;
using MyBlog.WebApi.DTOs.Blogs;
using MyBlog.WebApi.Infrastructure.Cache;
using MyBlog.WebApi.Infrastructure.Mapper.Extensions;
using Services.Blogs;
using Services.Helpers;
using Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyBlog.WebApi.Factories
{
    /// <summary>
    /// Represents the blog model factory
    /// </summary>
    public class BlogFactory : IBlogFactory
    {
        #region Fields
        private readonly IBlogService _blogService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IUserService _userService;
        #endregion

        #region Constructor
        public BlogFactory(IBlogService blogService,
            IStaticCacheManager cacheManager,
            IDateTimeHelper dateTimeHelper,
            IUserService userService)
        {
            _blogService = blogService;
            _cacheManager = cacheManager;
            _dateTimeHelper = dateTimeHelper;
            _userService = userService;
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

            var cacheKey = string.Format(CacheDefaults.BlogCommentsNumberKey, blogPost.Id, true);
            blogPostDto.NumberOfComments = _cacheManager.Get(cacheKey, () => _blogService.GetBlogCommentsCount(blogPost, true));

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
            blogCommentDto.CustomerName = user?.Firstname + user?.Lastname;
            blogCommentDto.CustomerAvatarUrl = user?.ProfileUrl;
            blogCommentDto.AllowViewingProfiles = false;
            blogCommentDto.CreatedOn = _dateTimeHelper.ConvertToUserTime(blogComment.CreatedOnUtc, DateTimeKind.Utc).ToString("g"); //TODO: Can be changed according to need

            return blogCommentDto;

        }
        #endregion
    }
}
