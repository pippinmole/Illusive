using System;
using System.Collections.Generic;
using System.Linq;
using Illusive.Models;

namespace Illusive.Utility {
    public static class ForumFilters {

        private static readonly Dictionary<string, Func<List<ForumData>, List<ForumData>>> Filters = new Dictionary<string, Func<List<ForumData>, List<ForumData>>>
        {
            {
                "date", (x) => x.OrderByDescending(y => y.TimeCreated).ToList()
            },
            {
                "views", (x) => x.OrderByDescending(y => y.Views).ToList()
            }
        };
        
        public static List<ForumData> FilterBy(this List<ForumData> forums, string filterName) {
            var filter = Filters.FirstOrDefault(x => x.Key == filterName);

            if ( filter.Key == default )
                return forums;

            return filter.Value(forums);
        }
    }
}