using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TuanZi.Collections
{
    public static class PagingExtensions
    {
        #region HtmlHelper extensions

        public static Pager Pager(this IHtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount)
        {
            return new Pager(htmlHelper, pageSize, currentPage, totalItemCount);
        }

        //public static Pager Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, AjaxOptions ajaxOptions)
        //{
        //    return new Pager(htmlHelper, pageSize, currentPage, totalItemCount).Options(o => o.AjaxOptions(ajaxOptions));
        //}

        public static Pager Pager(this IHtmlHelper htmlHelper, IPagedList pageList)
        {
            return new Pager(htmlHelper, pageList.PageSize, pageList.PageNumber, pageList.TotalItemCount);
        }

        //public static Pager Pager(this HtmlHelper htmlHelper, IPagedList pageList, AjaxOptions ajaxOptions)
        //{
        //    return new Pager(htmlHelper, pageList.PageSize, pageList.PageNumber, pageList.TotalItemCount).Options(o => o.AjaxOptions(ajaxOptions));
        //}

        public static Pager<TModel> Pager<TModel>(this IHtmlHelper<TModel> htmlHelper, int pageSize, int currentPage, int totalItemCount)
        {
            return new Pager<TModel>(htmlHelper, pageSize, currentPage, totalItemCount);
        }

        public static Pager<TModel> Pager<TModel>(this IHtmlHelper<TModel> htmlHelper, IPagedList<TModel> pageList)
        {
            return new Pager<TModel>(htmlHelper, pageList.PageSize, pageList.PageNumber, pageList.TotalItemCount);
        }

        //public static Pager<TModel> Pager<TModel>(this HtmlHelper<TModel> htmlHelper, IPagedList<TModel> pageList, AjaxOptions ajaxOptions)
        //{
        //    return new Pager<TModel>(htmlHelper, pageList.PageSize, pageList.PageNumber, pageList.TotalItemCount).Options(o => o.AjaxOptions(ajaxOptions));
        //}


        //public static Pager<TModel> Pager<TModel>(this HtmlHelper<TModel> htmlHelper, int pageSize, int currentPage, int totalItemCount, AjaxOptions ajaxOptions)
        //{
        //    return new Pager<TModel>(htmlHelper, pageSize, currentPage, totalItemCount).Options(o => o.AjaxOptions(ajaxOptions));
        //}

        #endregion

        #region IQueryable<T> extensions


        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int? pageIndex, int? pageSize = null, int? totalCount = null)
        {
            int currentPageIndex = pageIndex.HasValue ? pageIndex.Value - 1 : 0;

            return new PagedList<T>(source, currentPageIndex, pageSize, totalCount);
        }

        public static Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int? pageIndex, int? pageSize = null, int? totalCount = null)
        {
            return Task.Run(() =>
            {
                return source.ToPagedList(pageIndex, pageSize, totalCount);
            });

        }




        #endregion

        #region IEnumerable<T> extensions

        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int? pageIndex, int? pageSize = null, int? totalCount = null)
        {
            int currentPageIndex = pageIndex.HasValue ? pageIndex.Value - 1 : 0;
            return new PagedList<T>(source, currentPageIndex, pageSize, totalCount);
        }


        #endregion
    }
}