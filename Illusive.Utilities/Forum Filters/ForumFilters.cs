using System;
using System.Collections.Generic;
using System.Linq;
using Illusive.Models;

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

            if ( order.Key == default )
                return forums;

            return order.Value(forums);
        }
    }
}