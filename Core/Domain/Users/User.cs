namespace Core.Domain.Users
{
    /// <summary>
    /// Represents user
    /// </summary>
    public partial class User : BaseEntity
    {
        /// <summary>
        /// Gets or sets the firstname
        /// </summary>
        public string Firstname { get; set; }

        /// <summary>
        /// Gets or sets the lastname
        /// </summary>
        public string Lastname { get; set; }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the profile url
        /// </summary>
        public string ProfileUrl { get; set; }

        /// <summary>
        /// Gets or sets the token
        /// </summary>
        public string Token { get; set; }
    }
}
