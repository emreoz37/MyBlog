using Core.Domain.Blogs;
using MyBlog.WebApi.DTOs.Blogs;

namespace MyBlog.WebApi.Factories
{
    /// <summary>
    /// Represents the blog model factory
    /// </summary>
    public partial interface IBlogFactory
    {
        /// <summary>
        /// Prepare blog post response
        /// </summary>
        /// <param name="blogPostDto">Blog post dto</param>
        /// <param name="blogPost">Blog post entity</param>
        /// <param name="prepareComments">Whether to prepare blog comments</param>
        void PrepareBlogPostResponse(BlogPostDto blogPostDto, BlogPost blogPost, bool prepareComments);

        /// <summary>
        /// Prepare blog comment model
        /// </summary>
        /// <param name="blogComment">Blog comment entity</param>
        /// <returns>Blog comment dto</returns>
        BlogCommentDto PrepareBlogPostCommentModel(BlogComment blogComment);
    }
}
