using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace Library.API.Filters
{
    /// <summary>
    /// 注册日期超过3天后才有权限访问
    /// </summary>
    public class RegisteredMoreThan3DaysRequirement :
        AuthorizationHandler<RegisteredMoreThan3DaysRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RegisteredMoreThan3DaysRequirement requirement)
        {
            if (!context.User.HasClaim(claim => claim.Type == "RegisterDate"))
            {
                return Task.CompletedTask;
            }

            var registerDate = Convert.ToDateTime(context.User.FindFirst(claim => claim.Type == "RegisterDate").Value);
            var timeSpan = DateTime.Now - registerDate;
            if (timeSpan.TotalDays > 3)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
