namespace Core.Caching
{
    /// <summary>
    /// Represents default values related to caching
    /// </summary>
    public static partial class CachingDefaults
    {
        /// <summary>
        /// Gets the default cache time in minutes
        /// </summary>
        public static int CacheTime => 60;

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : Entity type name
        /// {1} : Entity id
        /// </remarks>
        public static string EntityCacheKey => "MyBlog.Entity.{0}.id-{1}";

        public static string BlogTagsCacheKey => "MyBlog.Presentation.blog.tags";
        public static string BlogPrefixCacheKey => "MyBlog.Presentation.blog";

        /// <summary>
        /// Key for number of blog comments
        /// </summary>
        /// <remarks>
        /// {0} : blog post ID
        /// {1} : are only approved comments?
        /// </remarks>
        public static string BlogCommentsNumberKey => "MyBlog.blog.comments.number-{0}-{1}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string BlogCommentsPrefixCacheKey => "MyBlog.blog.comments";


    }
}