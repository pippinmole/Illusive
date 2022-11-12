using System;
using System.Collections.Generic;
using System.Linq;
using Illusive.Models;
using Illusive.Pages;
using Microsoft.Extensions.Logging;

namespace Illusive.Utility {
    public static class ForumFilters {

        private static readonly Dictionary<string, Func<IEnumerable<ForumData>, List<ForumData>>> Orders = new Dictionary<string, Func<IEnumerable<ForumData>, List<ForumData>>>
        {
            {
                "views-asc", x => x.OrderBy(y => y.Views).ToList()
            },
            {
                "views-desc", x => x.OrderByDescending(y => y.Views).ToList()
            },
            {
                "likes-asc", x => x.OrderBy(y => y.Likes.Count).ToList()
            },
            {
                "likes-desc", x => x.OrderByDescending(y => y.Likes.Count).ToList()
            },
            {
                "date-asc", x => x.OrderBy(y => y.TimeCreated).ToList()
            },
            {
                "date-desc", x => x.OrderByDescending(y => y.TimeCreated).ToList()
            }
        };
        
        public static IEnumerable<ForumData> OrderBy(this IEnumerable<ForumData> forums, string orderName) {
            var order = Orders.FirstOrDefault(x => x.Key == orderName);
            return order.Key == default ? forums : order.Value(forums);
        }

        public static IEnumerable<ForumData> Test(this IEnumerable<ForumData> forums, int pageCount, int length) {
            return forums.Skip((pageCount - 1) * length).Take(length);
        }
        
        public static IEnumerable<ForumData> WithTags(this IEnumerable<ForumData> forums, string[] filterByTags) {
            if ( filterByTags == null || filterByTags.Length == 0 )
                return forums;
            
            return forums.Where(x => 
                x.Tags != null &&
                x.Tags.Split(",")
                    .Any(y => filterByTags.Contains(y.Trim())));
        }
    }
}