using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DS.Api.Base.Filters
{
    public class FileValidationActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if(context.Exception!=null)
            {
                context.Result= new BadRequestObjectResult(context.Exception.Message);
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if(context.HttpContext.Request.Form.Files.Count<=0 ||
                !context.HttpContext.Request.Form.Files[0].FileName.ToLower().EndsWith(".csv"))
            {
                context.HttpContext.Response.StatusCode = 400;
                context.HttpContext.Response.WriteAsync("no csv file");
            }
        }
    }
}
