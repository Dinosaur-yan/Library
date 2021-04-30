using System.Collections.Generic;

namespace Library.API.Models
{
    public class ResourceCollect<T> : Resource
        where T : Resource
    {
        public ResourceCollect(List<T> items)
        {
            Items = items;
        }

        public List<T> Items { get; private set; }
    }
}
