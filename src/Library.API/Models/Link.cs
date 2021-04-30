using Newtonsoft.Json;

namespace Library.API.Models
{
    public class Link
    {
        public Link(string href, string method, string relation)
        {
            Href = href;
            Method = method;
            Relation = relation;
        }

        /// <summary>
        /// 用户可以检索资源或改变应用状态的URL
        /// </summary>
        public string Href { get; private set; }

        /// <summary>
        /// 请求该URL要是用的http方法
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// 描述href指向的资源和现有资源的关系
        /// </summary>
        [JsonProperty("rel")]
        public string Relation { get; private set; }
    }
}
