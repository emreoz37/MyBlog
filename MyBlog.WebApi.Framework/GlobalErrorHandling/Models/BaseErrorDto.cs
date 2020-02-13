using Newtonsoft.Json;

namespace MyBlog.WebApi.Framework.GlobalErrorHandling.Models
{
    /// <summary>
    /// Represent error details
    /// </summary>
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
