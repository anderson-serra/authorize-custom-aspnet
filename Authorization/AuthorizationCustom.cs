using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace CustomAuthorizeAttribute.Authorization
{
    /// <summary>
    /// BASE: https://www.c-sharpcorner.com/article/how-to-override-customauthorization-class-in-net-core/
    /// </summary>
    //[AttributeUsage(AttributeTargets.Class)]
    public class AuthorizationCustom : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {

            if (filterContext != null)
            {
                Microsoft.Extensions.Primitives.StringValues qualquerToken;
                filterContext.HttpContext.Request.Headers.TryGetValue("qualquer-header", out qualquerToken);

                var header = qualquerToken.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(header))
                {
                    if (IsValidToken(header))
                    {
                        filterContext.HttpContext.Response.Headers.Add("authToken", header);
                        filterContext.HttpContext.Response.Headers.Add("AuthStatus", "Authorized");

                        filterContext.HttpContext.Response.Headers.Add("storeAccessiblity", "Authorized");

                        return;
                    }
                    else
                    {
                        filterContext.HttpContext.Response.Headers.Add("authToken", header);
                        filterContext.HttpContext.Response.Headers.Add("AuthStatus", "NotAuthorized");

                        filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        filterContext.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Not Authorized";
                        filterContext.Result = new JsonResult("NotAuthorized")
                        {
                            Value = new
                            {
                                Status = "Error",
                                Message = "Invalid Token"
                            },
                        };
                    }
                }
                else
                    EnviarFalharNaAuthorizacao(filterContext);
            }
        }

        private static void EnviarFalharNaAuthorizacao(AuthorizationFilterContext filterContext)
        {
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
            filterContext.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Please Provide authToken";
            filterContext.Result = new JsonResult("Please Provide authToken")
            {
                Value = new
                {
                    Status = "Error",
                    Message = "Please Provide authToken"
                },
            };
        }

        private bool IsValidToken(string authToken)
            => "01234567890".Equals(authToken);
    }
}
