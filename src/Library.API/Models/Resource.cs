using Newtonsoft.Json;
using System.Collections.Generic;

namespace Library.API.Models
{
    public class Resource
    {
        /// <summary>
        /// 所有资源的基类
        /// </summary>
        [JsonProperty("_links", Order = 100)]   //order属性指定所标识属性序列化的位置
        public List<Link> Links { get; } = new List<Link>();
    }
}
