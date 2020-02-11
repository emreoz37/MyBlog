using Core.Infrastructure;

namespace Core
{
    /// <summary>
    /// Represents a common helper
    /// </summary>
    public partial class CommonHelper
    {
        #region Properties

        /// <summary>
        /// Gets or sets the default file provider
        /// </summary>
        public static IProjectFileProvider DefaultFileProvider { get; set; }

        #endregion
    }
}
