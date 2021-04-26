using Library.API.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Library.API.Extensions
{
    public static class IQueryableExtension
    {
        public static IQueryable<T> Sort<T>(this IQueryable<T> source, string orderBy, Dictionary<string, PropertyMapping> mapping)
            where T : class
        {
            var allQueryParts = orderBy.Split(",");
            List<string> sortParts = new List<string>();
            foreach (var item in allQueryParts)
            {
                string property = string.Empty;
                bool isDescending = false;
                if (item.ToLower().EndsWith(SortDirection.Desc.ToString().ToLower()))
                {
                    property = item.Substring(0, item.Length - SortDirection.Desc.ToString().Length);
                }
                else
                {
                    property = item.Trim();
                }

                if (mapping.Keys.Any(t => t.ToLower() == property.ToLower()))
                {
                    property = mapping.Keys.First(t => t.ToLower() == property.ToLower());
                    if (mapping[property].IsRevert)
                    {
                        isDescending = !isDescending;
                    }

                    if (isDescending)
                    {
                        sortParts.Add($"{mapping[property].TargetProperty} {SortDirection.Desc}");
                    }
                    else
                    {
                        sortParts.Add($"{mapping[property].TargetProperty} {SortDirection.Asc}");
                    }
                }
            }

            string finalExpression = string.Join(",", sortParts);
            source = source.OrderBy(finalExpression);
            return source;
        }
    }

    public enum SortDirection
    {
        [Description("正序")]
        Asc,

        [Description("倒序")]
        Desc
    }
}
