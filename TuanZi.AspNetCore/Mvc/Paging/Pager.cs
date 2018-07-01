﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using TuanZi.AspNetCore.Http;
using TuanZi.Extensions;

namespace TuanZi.Collections
{

    public class Pager : IHtmlContent
    {
        private readonly IHtmlHelper htmlHelper;
        private readonly int pageSize;
        private readonly int currentPage;
        private int totalItemCount;
        protected readonly PagerOptions pagerOptions;


        public Pager(IHtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount)
        {
            this.htmlHelper = htmlHelper;
            this.pageSize = pageSize;
            this.currentPage = currentPage;
            this.totalItemCount = totalItemCount;
            this.pagerOptions = new PagerOptions();
        }

        public Pager Options(Action<PagerOptionsBuilder> buildOptions)
        {
            buildOptions(new PagerOptionsBuilder(this.pagerOptions));
            return this;
        }

        public virtual PaginationModel BuildPaginationModel(Func<int, string> generateUrl)
        {
            int pageCount;
            if (this.pagerOptions.UseItemCountAsPageCount)
            {
                // Set page count directly from total item count instead of calculating. Then calculate totalItemCount based on pageCount and pageSize;
                pageCount = this.totalItemCount;
                this.totalItemCount = pageCount * this.pageSize;
            }
            else
            {
                pageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
            }

            var model = new PaginationModel { PageSize = this.pageSize, CurrentPage = this.currentPage, TotalItemCount = this.totalItemCount, PageCount = pageCount };

            // First page
            if (this.pagerOptions.DisplayFirstAndLastPage)
            {
                model.PaginationLinks.Add(new PaginationLink { Active = (currentPage > 1 ? true : false), DisplayText = this.pagerOptions.FirstPageText, DisplayTitle = this.pagerOptions.FirstPageTitle, PageIndex = 1, Url = generateUrl(1) });
            }

            // Previous page
            if (!this.pagerOptions.HidePreviousAndNextPage)
            {
                var previousPageText = this.pagerOptions.PreviousPageText;
                model.PaginationLinks.Add(currentPage > 1 ? new PaginationLink { Active = true, DisplayText = previousPageText, DisplayTitle = this.pagerOptions.PreviousPageTitle, PageIndex = currentPage - 1, Url = generateUrl(currentPage - 1) } : new PaginationLink { Active = false, DisplayText = previousPageText });
            }
            var start = 1;
            var end = pageCount;
            var nrOfPagesToDisplay = this.pagerOptions.MaxNrOfPages;

            if (pageCount > nrOfPagesToDisplay)
            {
                var middle = (int)Math.Ceiling(nrOfPagesToDisplay / 2d) - 1;
                var below = (currentPage - middle);
                var above = (currentPage + middle);

                if (below < 2)
                {
                    above = nrOfPagesToDisplay;
                    below = 1;
                }
                else if (above > (pageCount - 2))
                {
                    above = pageCount;
                    below = (pageCount - nrOfPagesToDisplay + 1);
                }

                start = below;
                end = above;
            }

            if (start > 1)
            {
                model.PaginationLinks.Add(new PaginationLink { Active = true, PageIndex = 1, DisplayText = "1", Url = generateUrl(1) });
                if (start > 3)
                {
                    model.PaginationLinks.Add(new PaginationLink { Active = true, PageIndex = 2, DisplayText = "2", Url = generateUrl(2) });
                }
                if (start > 2)
                {
                    model.PaginationLinks.Add(new PaginationLink { Active = false, DisplayText = "...", IsSpacer = true });
                }
            }

            for (var i = start; i <= end; i++)
            {
                if (i == currentPage || (currentPage <= 0 && i == 1))
                {
                    model.PaginationLinks.Add(new PaginationLink { Active = true, PageIndex = i, IsCurrent = true, DisplayText = i.ToString() });
                }
                else
                {
                    model.PaginationLinks.Add(new PaginationLink { Active = true, PageIndex = i, DisplayText = i.ToString(), Url = generateUrl(i) });
                }
            }

            if (end < pageCount)
            {
                if (end < pageCount - 1)
                {
                    model.PaginationLinks.Add(new PaginationLink { Active = false, DisplayText = "...", IsSpacer = true });
                }
                if (pageCount - 2 > end)
                {
                    model.PaginationLinks.Add(new PaginationLink { Active = true, PageIndex = pageCount - 1, DisplayText = (pageCount - 1).ToString(), Url = generateUrl(pageCount - 1) });
                }

                model.PaginationLinks.Add(new PaginationLink { Active = true, PageIndex = pageCount, DisplayText = pageCount.ToString(), Url = generateUrl(pageCount) });
            }

            // Next page
            if (!this.pagerOptions.HidePreviousAndNextPage)
            {
                var nextPageText = this.pagerOptions.NextPageText;
                model.PaginationLinks.Add(currentPage < pageCount ? new PaginationLink { Active = true, PageIndex = currentPage + 1, DisplayText = nextPageText, DisplayTitle = this.pagerOptions.NextPageTitle, Url = generateUrl(currentPage + 1) } : new PaginationLink { Active = false, DisplayText = nextPageText });
            }

            // Last page
            if (this.pagerOptions.DisplayFirstAndLastPage)
            {
                model.PaginationLinks.Add(new PaginationLink { Active = (currentPage < pageCount ? true : false), DisplayText = this.pagerOptions.LastPageText, DisplayTitle = this.pagerOptions.LastPageTitle, PageIndex = pageCount, Url = generateUrl(pageCount) });
            }

            // AjaxOptions
            //if (pagerOptions.AjaxOptions != null)
            //{
            //    model.AjaxOptions = pagerOptions.AjaxOptions;
            //}

            model.Options = pagerOptions;
            return model;
        }


        public override string ToString()
        {
            var model = BuildPaginationModel(GeneratePageUrl);

            if (!string.IsNullOrEmpty(this.pagerOptions.DisplayTemplate))
            {
                var templatePath = string.Format("Pagings/{0}", this.pagerOptions.DisplayTemplate);

                var content = htmlHelper.Partial(templatePath, model);

                var writer = new StringWriter();
                content.WriteTo(writer, HtmlEncoder.Default);
                return writer.ToString();

            }
            else
            {
                var sb = new StringBuilder();

                foreach (var paginationLink in model.PaginationLinks)
                {
                    if (paginationLink.Active)
                    {
                        if (paginationLink.IsCurrent)
                        {
                            sb.AppendFormat("<span class=\"current\">{0}</span>", paginationLink.DisplayText);
                        }
                        else if (!paginationLink.PageIndex.HasValue)
                        {
                            sb.AppendFormat(paginationLink.DisplayText);
                        }
                        else
                        {
                            var linkBuilder = new StringBuilder("<a");

                            //if (pagerOptions.AjaxOptions != null)
                            //    foreach (var ajaxOption in pagerOptions.AjaxOptions.ToUnobtrusiveHtmlAttributes())
                            //        linkBuilder.AppendFormat(" {0}=\"{1}\"", ajaxOption.Key, ajaxOption.Value);

                            linkBuilder.AppendFormat(" href=\"{0}\" title=\"{1}\">{2}</a>", paginationLink.Url, paginationLink.DisplayTitle, paginationLink.DisplayText);

                            sb.Append(linkBuilder.ToString());
                        }
                    }
                    else
                    {
                        if (!paginationLink.IsSpacer)
                        {
                            sb.AppendFormat("<span class=\"disabled\">{0}</span>", paginationLink.DisplayText);
                        }
                        else
                        {
                            sb.AppendFormat("<span class=\"spacer\">{0}</span>", paginationLink.DisplayText);
                        }
                    }
                }
                return sb.ToString();
            }



        }

        protected virtual string GeneratePageUrl(int pageNumber)
        {
            var viewContext = this.htmlHelper.ViewContext;
            var routeDataValues = viewContext.HttpContext.GetRouteData().Values;
            RouteValueDictionary pageLinkValueDictionary;

            // Avoid canonical errors when pageNumber is equal to 1.
            if (pageNumber == 1 && !this.pagerOptions.AlwaysAddFirstPageNumber)
            {
                pageLinkValueDictionary = new RouteValueDictionary(this.pagerOptions.RouteValues);

                if (routeDataValues.ContainsKey(this.pagerOptions.PageRouteValueKey))
                {
                    routeDataValues.Remove(this.pagerOptions.PageRouteValueKey);
                }
            }
            else
            {
                pageLinkValueDictionary = new RouteValueDictionary(this.pagerOptions.RouteValues) { { this.pagerOptions.PageRouteValueKey, pageNumber } };
            }

            // To be sure we get the right route, ensure the controller and action are specified.
            if (!pageLinkValueDictionary.ContainsKey("controller") && routeDataValues.ContainsKey("controller"))
            {
                pageLinkValueDictionary.Add("controller", routeDataValues["controller"]);
            }

            if (!pageLinkValueDictionary.ContainsKey("action") && routeDataValues.ContainsKey("action"))
            {
                pageLinkValueDictionary.Add("action", routeDataValues["action"]);
            }

            if (!pageLinkValueDictionary.ContainsKey("area") && routeDataValues.ContainsKey("area"))
            {
                pageLinkValueDictionary.Add("area", routeDataValues["area"]);
            }

            // Fix the dictionary if there are arrays in it.
            pageLinkValueDictionary = pageLinkValueDictionary.FixListRouteDataValues();

            // 'Render' virtual path.

            var vpc = new VirtualPathContext(viewContext.HttpContext, null, pageLinkValueDictionary);

            var path = "~/";
            foreach (var obj in pageLinkValueDictionary)
            {
                Path.Combine(path, obj.Value.ToStringSafe());
            }

            // var virtualPathForArea = RouteTable.Routes.GetVirtualPathForArea(viewContext.RequestContext, pageLinkValueDictionary);

            return path;

            //return virtualPathForArea == null ? null : virtualPathForArea.VirtualPath;
        }

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (encoder == null)
            {
                throw new ArgumentNullException(nameof(encoder));
            }

            writer.Write(ToString());
        }
    }

    public class Pager<TModel> : Pager
    {
        private IHtmlHelper<TModel> htmlHelper;

        public Pager(IHtmlHelper<TModel> htmlHelper, int pageSize, int currentPage, int totalItemCount)
            : base(htmlHelper, pageSize, currentPage, totalItemCount)
        {
            this.htmlHelper = htmlHelper;
        }

        public Pager<TModel> Options(Action<PagerOptionsBuilder<TModel>> buildOptions)
        {
            buildOptions(new PagerOptionsBuilder<TModel>(this.pagerOptions, htmlHelper));
            return this;
        }
    }
}