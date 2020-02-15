namespace Core
{
    /// <summary>
    /// Represents base link info for action responses
    /// </summary>
    public class BaseLinkInfo
    {
        /// <summary>
        /// Link Path
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// The link belongs to which page, previous, next, self ... etc
        /// </summary>
        public string Rel { get; set; }

        /// <summary>
        /// Hangi HTTP metot kullanılacak GET,POST,PUT ... vs
        /// </summary>
        public string Method { get; set; }
    }
}
