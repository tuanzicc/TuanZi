using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;

namespace TuanZi.AspNetCore.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static HtmlString ToHtmlString(this IHtmlContent htmlContent)
        {
            var writer = new StringWriter();
            htmlContent.WriteTo(writer, HtmlEncoder.Default);

            return new HtmlString(writer.ToString());
        }

      
    }
}
