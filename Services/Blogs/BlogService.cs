﻿using Core;
using Core.Caching;
using Core.Domain.Blogs;
using Data;
using Services.Caching.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Blogs
{
    /// <summary>
    /// Blog service
    /// </summary>
    public partial class BlogService : IBlogService
    {
        #region Fields
        private readonly IRepository<BlogComment> _blogCommentRepository;
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public BlogService(
            IRepository<BlogComment> blogCommentRepository,
            IRepository<BlogPost> blogPostRepository,
            IStaticCacheManager cacheManager)
        {
            _blogCommentRepository = blogCommentRepository;
            _blogPostRepository = blogPostRepository;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        #region Blog posts

        /// <summary>
        /// Deletes a blog post
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        public virtual void DeleteBlogPost(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            _blogPostRepository.Delete(blogPost); //TODO : Update can be done instead of deleting from the database.

            _cacheManager.RemoveByPrefix("pres.blog");
        }

        /// <summary>
        /// Gets a blog post
        /// </summary>
        /// <param name="blogPostId">Blog post identifier</param>
        /// <returns>Blog post</returns>
        public virtual BlogPost GetBlogPostById(int blogPostId)
        {
            if (blogPostId == 0)
                return null;

            return _blogPostRepository.ToCachedGetById(blogPostId);
        }

        /// <summary>
        /// Gets blog posts
        /// </summary>
        /// <param name="blogPostIds">Blog post identifiers</param>
        /// <returns>Blog posts</returns>
        public virtual IList<BlogPost> GetBlogPostsByIds(int[] blogPostIds)
        {
            var query = _blogPostRepository.Table;
            return query.Where(bp => blogPostIds.Contains(bp.Id)).ToList();
        }

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="dateFrom">Filter by created date; null if you want to get all records</param>
        /// <param name="dateTo">Filter by created date; null if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Blog posts</returns>
        public virtual IPagedList<BlogPost> GetAllBlogPosts(int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query =  _blogPostRepository.Table;
            if (!showHidden)
            {
                query = query.Where(b => !b.StartDateUtc.HasValue || b.StartDateUtc <= DateTime.UtcNow);
                query = query.Where(b => !b.EndDateUtc.HasValue || b.EndDateUtc >= DateTime.UtcNow);
            }

            query = query.OrderByDescending(b => b.StartDateUtc ?? b.CreatedOnUtc);

            var blogPosts = new PagedList<BlogPost>(query, pageIndex, pageSize);
            return blogPosts;
        }

        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="languageId">Language identifier. 0 if you want to get all blog posts</param>
        /// <param name="tag">Tag</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Blog posts</returns>
        public virtual IPagedList<BlogPost> GetAllBlogPostsByTag(string tag = "",
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            tag = tag.Trim();

            //we load all records and only then filter them by tag
            var blogPostsAll = GetAllBlogPosts(showHidden: showHidden);
            var taggedBlogPosts = new List<BlogPost>();
            foreach (var blogPost in blogPostsAll)
            {
                var tags = ParseTags(blogPost);
                if (!string.IsNullOrEmpty(tags.FirstOrDefault(t => t.Equals(tag, StringComparison.InvariantCultureIgnoreCase))))
                    taggedBlogPosts.Add(blogPost);
            }

            //server-side paging
            var result = new PagedList<BlogPost>(taggedBlogPosts, pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// Gets all blog post tags
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="languageId">Language identifier. 0 if you want to get all blog posts</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Blog post tags</returns>
        public virtual IList<BlogPostTag> GetAllBlogPostTags(int storeId, int languageId, bool showHidden = false)
        {
            var blogPostTags = new List<BlogPostTag>();

            var blogPosts = GetAllBlogPosts(showHidden: showHidden);
            foreach (var blogPost in blogPosts)
            {
                var tags = ParseTags(blogPost);
                foreach (var tag in tags)
                {
                    var foundBlogPostTag = blogPostTags.Find(bpt => bpt.Name.Equals(tag, StringComparison.InvariantCultureIgnoreCase));
                    if (foundBlogPostTag == null)
                    {
                        foundBlogPostTag = new BlogPostTag
                        {
                            Name = tag,
                            BlogPostCount = 1
                        };
                        blogPostTags.Add(foundBlogPostTag);
                    }
                    else
                        foundBlogPostTag.BlogPostCount++;
                }
            }

            var cacheKey = string.Format("blog.tags");

            return _cacheManager.Get(cacheKey, () => blogPostTags);
        }

        /// <summary>
        /// Inserts a blog post
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        public virtual void InsertBlogPost(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            _blogPostRepository.Insert(blogPost);

            _cacheManager.RemoveByPrefix("pres.blog");

        }

        /// <summary>
        /// Updates the blog post
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        public virtual void UpdateBlogPost(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            _blogPostRepository.Update(blogPost);

            _cacheManager.RemoveByPrefix("pres.blog");
        }

        /// <summary>
        /// Returns all posts published between the two dates.
        /// </summary>
        /// <param name="blogPosts">Source</param>
        /// <param name="dateFrom">Date from</param>
        /// <param name="dateTo">Date to</param>
        /// <returns>Filtered posts</returns>
        public virtual IList<BlogPost> GetPostsByDate(IList<BlogPost> blogPosts, DateTime dateFrom, DateTime dateTo)
        {
            if (blogPosts == null)
                throw new ArgumentNullException(nameof(blogPosts));

            return blogPosts
                .Where(p => dateFrom.Date <= (p.StartDateUtc ?? p.CreatedOnUtc) && (p.StartDateUtc ?? p.CreatedOnUtc).Date <= dateTo)
                .ToList();
        }

        /// <summary>
        /// Parse tags
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        /// <returns>Tags</returns>
        public virtual IList<string> ParseTags(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            if (blogPost.Tags == null)
                return new List<string>();

            var tags = blogPost.Tags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(tag => tag.Trim())
                .Where(tag => !string.IsNullOrEmpty(tag)).ToList();

            return tags;
        }

        /// <summary>
        /// Get a value indicating whether a blog post is available now (availability dates)
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        /// <param name="dateTime">Datetime to check; pass null to use current date</param>
        /// <returns>Result</returns>
        public virtual bool BlogPostIsAvailable(BlogPost blogPost, DateTime? dateTime = null)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            if (blogPost.StartDateUtc.HasValue && blogPost.StartDateUtc.Value >= (dateTime ?? DateTime.UtcNow))
                return false;

            if (blogPost.EndDateUtc.HasValue && blogPost.EndDateUtc.Value <= (dateTime ?? DateTime.UtcNow))
                return false;

            return true;
        }

        #endregion

        #region Blog comments

        /// <summary>
        /// Gets all comments
        /// </summary>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="blogPostId">Blog post ID; 0 or null to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item creation to; null to load all records</param>
        /// <param name="commentText">Search comment text; null to load all records</param>
        /// <returns>Comments</returns>
        public virtual IList<BlogComment> GetAllComments(int customerId = 0, int storeId = 0, int? blogPostId = null,
            bool? approved = null, DateTime? fromUtc = null, DateTime? toUtc = null, string commentText = null)
        {
            var query = _blogCommentRepository.Table;

            if (approved.HasValue)
                query = query.Where(comment => comment.IsApproved == approved);

            if (blogPostId > 0)
                query = query.Where(comment => comment.BlogPostId == blogPostId);

            if (customerId > 0)
                query = query.Where(comment => comment.CustomerId == customerId);

            if (fromUtc.HasValue)
                query = query.Where(comment => fromUtc.Value <= comment.CreatedOnUtc);

            if (toUtc.HasValue)
                query = query.Where(comment => toUtc.Value >= comment.CreatedOnUtc);

            if (!string.IsNullOrEmpty(commentText))
                query = query.Where(c => c.CommentText.Contains(commentText));

            query = query.OrderBy(comment => comment.CreatedOnUtc);

            return query.ToList();
        }

        /// <summary>
        /// Gets a blog comment
        /// </summary>
        /// <param name="blogCommentId">Blog comment identifier</param>
        /// <returns>Blog comment</returns>
        public virtual BlogComment GetBlogCommentById(int blogCommentId)
        {
            if (blogCommentId == 0)
                return null;

            return _blogCommentRepository.ToCachedGetById(blogCommentId);
        }

        /// <summary>
        /// Get blog comments by identifiers
        /// </summary>
        /// <param name="commentIds">Blog comment identifiers</param>
        /// <returns>Blog comments</returns>
        public virtual IList<BlogComment> GetBlogCommentsByIds(int[] commentIds)
        {
            if (commentIds == null || commentIds.Length == 0)
                return new List<BlogComment>();

            var query = from bc in _blogCommentRepository.Table
                        where commentIds.Contains(bc.Id)
                        select bc;
            var comments = query.ToList();
            //sort by passed identifiers
            var sortedComments = new List<BlogComment>();
            foreach (var id in commentIds)
            {
                var comment = comments.Find(x => x.Id == id);
                if (comment != null)
                    sortedComments.Add(comment);
            }

            return sortedComments;
        }

        /// <summary>
        /// Get the count of blog comments
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="isApproved">A value indicating whether to count only approved or not approved comments; pass null to get number of all comments</param>
        /// <returns>Number of blog comments</returns>
        public virtual int GetBlogCommentsCount(BlogPost blogPost, bool? isApproved = null)
        {
            var query = _blogCommentRepository.Table.Where(comment => comment.BlogPostId == blogPost.Id);

            if (isApproved.HasValue)
                query = query.Where(comment => comment.IsApproved == isApproved.Value);

            return query.Count();
        }

        /// <summary>
        /// Updates the blog comment
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        public virtual void UpdateBlogComment(BlogComment blogComment)
        {
            if (blogComment == null)
                throw new ArgumentNullException(nameof(blogComment));

            _blogCommentRepository.Update(blogComment);

            _cacheManager.RemoveByPrefix("pres.blogcomment");
        }

        /// <summary>
        /// Deletes a blog comment
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        public virtual void DeleteBlogComment(BlogComment blogComment)
        {
            if (blogComment == null)
                throw new ArgumentNullException(nameof(blogComment));

            _blogCommentRepository.Delete(blogComment);


            _cacheManager.RemoveByPrefix("pres.blogcomment");
        }

        /// <summary>
        /// Deletes blog comments
        /// </summary>
        /// <param name="blogComments">Blog comments</param>
        public virtual void DeleteBlogComments(IList<BlogComment> blogComments)
        {
            if (blogComments == null)
                throw new ArgumentNullException(nameof(blogComments));

            foreach (var blogComment in blogComments)
            {
                DeleteBlogComment(blogComment);
            }
        }

        /// <summary>
        /// Inserts a blog comment
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        public virtual void InsertBlogComment(BlogComment blogComment)
        {
            if (blogComment == null)
                throw new ArgumentNullException(nameof(blogComment));

            _blogCommentRepository.Insert(blogComment);

            _cacheManager.RemoveByPrefix("pres.blogcomment");
        }

        #endregion

        #endregion
    }
}