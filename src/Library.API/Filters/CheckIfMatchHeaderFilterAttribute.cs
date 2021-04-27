using Library.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

namespace Library.API.Filters
{
    public class CheckIfMatchHeaderFilterAttribute : ActionFilterAttribute
    {
        /**
         * 并发：当多个客户端同时请求一个资源
         *  当多个客户端同时请求一个资源，并且都对该资源进行数据修改时，由于客户端向服务器提交的时间不一致，结果导致先提交的数据被后提交的数据覆盖（后修改者有效）
         * 
         * 并发处理策略：
         *  1>. 保守式并发控制（悲观并发控制）：当客户端修改资源时，有服务器将其锁定，这样其他用户就不能进行修改
         *  2>. 开放式并发控制（乐观并发控制）：通过表示资源的当前状态的散列值实现的。当客户修改资源时，将其散列值一并提交给服务器（使用If-Match消息头），服务器检查该值是否有效
         *      有效：该资源在此期间未被修改，服务器允许进行修改
         *      失效：该资源已经被修改过，服务器将拒绝客户端的修改请求（返回412PreconditionFailed状态码，表示先决条件失败）
         *      
         * If-None-Match：验证成功,返回304NotModified状态码(未发生变化)
         * If-Match：验证成功，则继续执行操作，否则返回412PreconditionFailed状态码
         */
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.ContainsKey(HeaderNames.IfMatch))
            {
                context.Result = new BadRequestObjectResult(new ApiError
                {
                    Message = "必须提供If-Match消息头"
                });
            }

            return base.OnActionExecutionAsync(context, next);
        }
    }
}
