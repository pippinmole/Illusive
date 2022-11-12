using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Illusive.Utility; 

public static class RedirectUtilities {
    public static RedirectToPageResult PreserveQueryParameters(this RedirectToPageResult result, HttpContext content) {
        var queryCollection = content.Request.Query;
        // Remark: here you could decide if you want to propagate all
        // query string values or a particular one. In my example I am
        // propagating all query string values that are not already part of
        // the route values
        foreach ( var (key, value) in queryCollection ) {
            if ( result.RouteValues == null ) continue;

            if ( !result.RouteValues.ContainsKey(key) ) {
                result.RouteValues.Add(key, value);
            }
        }

        return result;
    }
}