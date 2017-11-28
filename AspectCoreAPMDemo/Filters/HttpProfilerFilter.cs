using System.Diagnostics;
using System.Web.Mvc;
using AspectCore.APM.HttpProfiler;
using AspectCore.APM.Profiler;

namespace AspectCoreAPMDemo.Filters
{
    public class HttpProfilerFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            filterContext.HttpContext.Items["stopwatch"] = stopwatch;
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            if (filterContext.HttpContext.Items.Contains("stopwatch"))
            {
                var stopwatch = filterContext.HttpContext.Items["stopwatch"] as Stopwatch;
                var profiler = DependencyResolver.Current.GetService<IProfiler<HttpProfilingContext>>();
                var context = new HttpProfilingContext
                {
                    Elapsed = stopwatch.ElapsedMilliseconds,
                    HttpHost = filterContext.HttpContext.Request.Url.Host,
                    HttpMethod = filterContext.HttpContext.Request.HttpMethod,
                    HttpPath = filterContext.HttpContext.Request.Path,
                    HttpPort = filterContext.HttpContext.Request.Url.Port.ToString(),
                    HttpScheme = filterContext.HttpContext.Request.Url.Scheme,
                    IdentityAuthenticationType = filterContext.HttpContext.User?.Identity?.AuthenticationType,
                    IdentityName = filterContext.HttpContext.User?.Identity?.Name,
                    RequestContentType = filterContext.HttpContext.Request.ContentType,
                    ResponseContentType = filterContext.HttpContext.Response.ContentType,
                    StatusCode = filterContext.HttpContext.Response.StatusCode.ToString()
                };
                profiler.Invoke(context);
            }
        }
    }
}