﻿using Library.API.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Library.API.Filters
{
    public class JsonExceptionFilter : IExceptionFilter
    {
        public JsonExceptionFilter(ILogger<Program> logger, IWebHostEnvironment env)
        {
            Logger = logger;
            Environment = env;
        }

        public ILogger Logger { get; }

        public IWebHostEnvironment Environment { get; }

        public void OnException(ExceptionContext context)
        {
            var error = new ApiError();

            if (Environment.IsDevelopment())
            {
                error.Message = context.Exception.Message;
                error.Detail = context.Exception.ToString();
            }
            else
            {
                error.Message = "服务器出错";
                error.Detail = context.Exception.Message;
            }

            context.Result = new ObjectResult(error)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"服务器发生异常：{context.Exception.Message}");
            sb.AppendLine(context.Exception.ToString());
            Logger.LogCritical(sb.ToString());
        }
    }
}
