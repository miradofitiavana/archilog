using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http.Routing;
namespace APILibrary.core.Helpers
{
    public class PageLinkBuilder
    {
        public Uri FirstPage { get; private set; }
        public Uri LastPage { get; private set; }
        public Uri NextPage { get; private set; }
        public Uri PreviousPage { get; private set; }

        public int _pageFrom { get; private set; }
        public int _pageTo { get; private set; }
        public int _pageSize { get; private set; }

        public PageLinkBuilder(IUrlHelper urlHelper, string routeName, object routeValues, int pageFrom, int pageTo, long totalRecordCount)
        {
            _pageFrom = pageFrom;
            _pageTo = pageTo;
            _pageSize = pageTo - pageFrom + 1;

            int pageSize = pageTo - pageFrom;

            var pageFromFirst = 0;
            var pageToFirst = pageSize;
            FirstPage = new Uri(urlHelper.Link(routeName, new HttpRouteValueDictionary(routeValues) { { "range", pageFromFirst + "-" + pageToFirst } }));

            var pageFromLast = totalRecordCount - 1 - pageSize;
            var pageToLast = totalRecordCount ;
            LastPage = new Uri(urlHelper.Link(routeName, new HttpRouteValueDictionary(routeValues) { { "range", (pageFromLast < pageToFirst ? pageFromLast : pageToFirst + 1) + "-" + pageToLast } }));
            
            if (pageFrom > 0)
            {
                var pageToPrev = pageFrom - 1;
                var pageFromPrev = pageToPrev - pageSize;
                PreviousPage = new Uri(urlHelper.Link(routeName, new HttpRouteValueDictionary(routeValues) { { "range", (pageFromPrev >= 0 ? pageFromPrev : 0) + "-" + pageToPrev } }));
            }
            
            if (pageTo < totalRecordCount)
            {
                var pageFromNext = pageTo + 1;
                var pageToNext = pageFromNext + pageSize;
                NextPage = new Uri(urlHelper.Link(routeName, new HttpRouteValueDictionary(routeValues) { { "range", pageFromNext + "-" + (pageToNext <= totalRecordCount ? pageToNext : totalRecordCount - 1) } }));
            }
        }

        public string GetContentRange()
        {
            return _pageFrom + "-" + _pageTo + "/" + _pageSize;
        }
    }
}
