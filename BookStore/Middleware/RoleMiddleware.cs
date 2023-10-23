using BookStore.CustomAtrribute;
using System.Reflection;
using Newtonsoft.Json;
using NuGet.Protocol;
using BookStore.DTO;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using BookStore.Models;

namespace BookStore.Middleware
{
    public class RoleMiddleware
    {
        private readonly RequestDelegate _next;
        
        public RoleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        //httpContext di qua middleware trong pipeline
        public async Task InvokeAsync(HttpContext context)
        {
            // Lấy RouteData từ HttpContext
            var routeData = context.GetRouteData();
            if (routeData != null)
            {
                // Lấy thông tin về action và controller từ RouteData
                string? controllerName = routeData.Values["controller"]?.ToString();
                string? actionName = routeData.Values["action"]?.ToString();


                if (actionName != null)
                {
                    // Lấy MethodInfo của action bằng reflection
                    Type? controllerType = Type.GetType($"BookStore.Controllers.{controllerName}Controller");
                    MethodInfo? methodInfo = controllerType.GetMethod(actionName, Type.EmptyTypes);

                    // Lấy danh sách các thuộc tính trên phương thức
                    var roleAttributes = methodInfo.GetCustomAttributes<RoleAttribute>();
                    List<string> roleNameAttrs = new List<string>();
                    foreach (var roleAttribute in roleAttributes)
                        {
                            roleNameAttrs.Add(roleAttribute.role);
                        }
                    if (roleNameAttrs.Count() > 0)
                    {
                        //lay ra account trong cookie
                        var cookie = context.Request.Cookies["account"];

                        //kiem tra login
                        if(cookie != null)
                        {
                            AccountDTO account = JsonConvert.DeserializeObject<AccountDTO>(cookie);
                            string roleCookie = account.role;
                            if (roleNameAttrs.Contains(roleCookie))
                            {
                                await _next(context);
                                return;
                            }
                            else
                            {
                                await Task.Run(
                                  async () => {
                                      string html = "<h1>KHONG DUOC TRUY CAP</h1>";
                                      context.Response.StatusCode = StatusCodes.Status403Forbidden;
                                      await context.Response.WriteAsync(html);
                                  }
                                );
                                return;
                            }
                        }
                        else
                        {
                            await Task.Run(
                              async () =>
                              {
                                  string html = "<h1>Ban phai <a href=\"/login\">dang nhap</a></h1>";
                                  context.Response.StatusCode = StatusCodes.Status403Forbidden;
                                  await context.Response.WriteAsync(html);
                              }
                            );
                            return;
                        }

                    }
                }
            }

            await _next(context);
        }
    }
}
