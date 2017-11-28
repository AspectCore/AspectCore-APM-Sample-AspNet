using System.Web;
using System.Web.Mvc;
using AspectCoreAPMDemo.Filters;

namespace AspectCoreAPMDemo
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new HttpProfilerFilter());
        }
    }
}
