using System;
using System.IO;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Illusive.Utility {
    public static class GoogleReCaptchaTagHelper {
        public static IHtmlContent
            GoogleReCaptcha(this IHtmlHelper htmlHelper, string siteKey, string callback = null) {
            var tagBuilder = GetReCaptchaTag("div", siteKey, callback);
            return GetHtmlContent(htmlHelper, tagBuilder);
        }


        public static IHtmlContent GoogleInvisibleReCaptcha(this IHtmlHelper htmlHelper, string text, string siteKey,
            string callback = null) {
            var tagBuilder = GetReCaptchaTag("button", siteKey, callback);
            tagBuilder.InnerHtml.Append(text);
            return GetHtmlContent(htmlHelper, tagBuilder);
        }

        private static TagBuilder GetReCaptchaTag(string tagName, string siteKey, string callback = null) {
            var tagBuilder = new TagBuilder(tagName);
            tagBuilder.Attributes.Add("class", "g-recaptcha");
            tagBuilder.Attributes.Add("data-sitekey", siteKey);
            if ( callback != null && !string.IsNullOrWhiteSpace(callback) ) {
                tagBuilder.Attributes.Add("data-callback", callback);
            }

            return tagBuilder;
        }

        private static IHtmlContent GetHtmlContent(IHtmlHelper htmlHelper, TagBuilder tagBuilder) {
            using var writer = new StringWriter();
            tagBuilder.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
            var htmlOutput = writer.ToString();
            return htmlHelper.Raw(htmlOutput);
        }
    }
}