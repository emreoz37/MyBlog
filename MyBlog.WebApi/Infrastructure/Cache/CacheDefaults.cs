namespace MyBlog.WebApi.Infrastructure.Cache
{
    public static partial class CacheDefaults
    {
        /// <summary>
        /// Key for number of blog comments
        /// </summary>
        /// <remarks>
        /// {0} : blog post ID
        /// {1} : are only approved comments?
        /// </remarks>
        public static string BlogCommentsNumberKey => "Nop.pres.blog.comments.number-{0}-{1}";
        public static string BlogCommentsPrefixCacheKey => "Nop.pres.blog.comments";
    }
}
