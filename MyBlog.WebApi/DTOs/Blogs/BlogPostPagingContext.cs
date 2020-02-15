using MyBlog.WebApi.Framework.DTOs.Paging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MyBlog.WebApi.DTOs.Blogs
{
    public class BlogPostPagingContext : BasePageableDto
    {
        public string ToJson() => JsonConvert.SerializeObject(this,
                                new JsonSerializerSettings
                                {
                                    ContractResolver = new
                CamelCasePropertyNamesContractResolver()
                                });
    }
}
