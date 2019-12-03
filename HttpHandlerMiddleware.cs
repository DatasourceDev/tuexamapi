using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace tuexamapi.CustomMiddleware
{
    public class HttpHandlerMiddleware
    {
        //2019-08-28 Jay: Add ASP Log into All catch exception
        private readonly RequestDelegate _next;

        private SecurityToken validatedToken;
        private JwtSecurityTokenHandler handler;
        //private MySQLContext db;

        public HttpHandlerMiddleware(RequestDelegate next, ILogger<Controller> logger)
        {
            //2019-08-28 Jay: Add ASP Log into All controller and MySqlDb
            this._next = next;
            handler = new JwtSecurityTokenHandler();
        }

        public async Task Invoke(HttpContext context, IConfiguration config)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtAuthentication:YXV0b2Rlc2tzZXJ2aWNlc2VjcmV0a2V5"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            var validationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.FromMinutes(5),
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["JwtAuthentication:ValidIssuer"],
                ValidAudience = config["JwtAuthentication:ValidAudience"],
                IssuerSigningKey = credentials.Key
            };
            var requestHeader = context.Request.Headers["Authorization"].ToString();

            if (context.Request.GetDisplayUrl().Contains("api/Language")
                || context.Request.GetDisplayUrl().Contains("api/Dictionaries")
                || context.Request.GetDisplayUrl().Contains("BrowserList/?currentBrowser=")
                || context.Request.GetDisplayUrl().Contains("/login")
                || context.Request.GetDisplayUrl().Contains("/LoginStudent")
                || context.Request.GetDisplayUrl().Contains("/CertificateDownload/where/")
                //|| context.Request.GetDisplayUrl().Contains("/CertificateBackground/")
                || context.Request.GetDisplayUrl().Contains("/EmailExist/")
                || context.Request.GetDisplayUrl().Contains("api/Countries")
                || context.Request.GetDisplayUrl().Contains("/Register")
                || context.Request.GetDisplayUrl().Contains("Auth/ResetPassword/")
                || context.Request.GetDisplayUrl().Contains("/MainContact/FirstLogin/")
                || context.Request.GetDisplayUrl().Contains("/MainContact/FirstLoginStudent/")
                || context.Request.GetDisplayUrl().Contains("/ChangePassword")
                || context.Request.GetDisplayUrl().Contains("/auth/ActivationCheck/")
                || context.Request.GetDisplayUrl().Contains("/Auth/ExpiredActivation/")
                 || context.Request.GetDisplayUrl().Contains("/Auth/ForgetPassword")
                || context.Request.GetDisplayUrl().Contains("/Auth/ForgetPasswordStudent")
                || context.Request.GetDisplayUrl().Contains("/Auth/ResetPasswordStudent")
                || context.Request.GetDisplayUrl().Contains("/EvaluationAnswer/DownloadExcelEval")
                || context.Request.GetDisplayUrl().Contains("/Auth/GeneratePassStudent")
                || context.Request.GetDisplayUrl().Contains("/Student/ForgotPwdStudent")
                || context.Request.GetDisplayUrl().Contains("api/MainOrganization/getFile")
                || context.Request.GetDisplayUrl().Contains("api/MainContact/where/")
                || context.Request.GetDisplayUrl().Contains("/CertificatePreview/where/")
                )
            {
                await _next?.Invoke(context);
            }
            else
            {

                try
                {
                    if (!string.IsNullOrEmpty(requestHeader))
                    {
                        //var token = requestHeader.Remove(0, 7);
                        //var items = handler.ValidateToken(token, validationParameters, out validatedToken);

                        //var userVersion = items.Identities.Select(x => x.Claims.SingleOrDefault(c => c.Type == "sid")?.Value).SingleOrDefault();
                        //var userVersionDec = Convert.ToDecimal(userVersion);
                        //var isClaim = items.Claims.Select(x => x.Type).ToList();


                        await _next?.Invoke(context);


                    }
                    else
                    {
                        JsonResult result = new JsonResult(new { msg = "Request is not Valid" });
                        string respondBody = JsonConvert.SerializeObject(result);

                        context.Response.ContentType = "application/json";
                        context.Response.Headers["Authorization"] = string.Empty;
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        await context.Response.WriteAsync(respondBody, Encoding.UTF8);
                    }
                }
                catch (Exception ex)
                {
                    JsonResult result = new JsonResult(new { msg = $"Validation is not valid for Token {requestHeader}" });
                    string respondBody = JsonConvert.SerializeObject(result);

                    context.Response.ContentType = "application/json";
                    context.Response.Headers["Authorization"] = string.Empty;
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsync(respondBody, Encoding.UTF8);
                }
            }
        }
    }
}
