namespace Core
{
    /// <summary>
    /// Base result class for responses
    /// </summary>
    public class BaseResult
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }
    }
}
