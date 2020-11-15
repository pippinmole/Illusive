using System;
using System.Collections.Generic;
using System.Linq;
using Illusive.Models;

namespace Illusive.Utility {
    public static class ForumFilters {

        private static readonly Dictionary<string, Func<IEnumerable<ForumData>, List<ForumData>>> Orders = new Dictionary<string, Func<IEnumerable<ForumData>, List<ForumData>>>
        {
            {
                "date", x => x.OrderByDescending(y => y.TimeCreated).ToList()
            },
            {
                "views", x => x.OrderByDescending(y => y.Views).ToList()
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