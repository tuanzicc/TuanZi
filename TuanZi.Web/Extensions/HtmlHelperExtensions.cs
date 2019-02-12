using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System.ComponentModel;
using TuanZi;
using Microsoft.AspNetCore.Hosting;
using TuanZi.Extensions;

namespace TuanZi
{
    public enum UploaderTypes
    {
        [Description("Unpecified")] Unpecified = 0,
        [Description("File")] File,
        [Description("Image")] Image,
        [Description("Video")] Video,
    }

    public static class HtmlHelperExtensions
    {
        public static IHtmlContent DropifyFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,  object htmlAttributes=null, string baseName=null, string baseUrl=null)
        {
            var fieldName = ((MemberExpression)expression.Body).Member.Name;

            var modelExplorer = ExpressionMetadataProvider.FromLambdaExpression(expression, htmlHelper.ViewData, htmlHelper.MetadataProvider);

            var id = modelExplorer.Model;

            var html = string.Empty;



            var filename = string.Empty;
            var img = string.Empty;


            if (id != null)
            {
                img = $"data-default-file='{baseUrl}/file/images?id={id}&.jpg'";
            }
           

            #region original html
            /*
                <input type="hidden" asp-for="Photo"/>
                <input type="file" asp-for="Photo" class="dropify" data-height="170"  data-default-file="/ondemand/web/file/images?id=@Model.Photo&w=300&.jpg" />
               */
            #endregion

            var guid = Guid.NewGuid();
           

            html += $"<input type='hidden' name='{(baseName!=null?baseName+".":"")}{fieldName}' id='{fieldName + guid}-hidden' value='{id}' >";
            html += $"<input type='file'  class='dropify'  id='{fieldName + guid}' name = '{fieldName}' {htmlAttributes} {img} value='{id}' title='{fieldName}' data-max-file-size='6M' />";


            return new HtmlString(html);
        }

    }

}
