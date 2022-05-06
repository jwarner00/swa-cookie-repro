using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Repro.Function
{
    public static class CookeFunctions
    {
        const string CookieName = "rp_cookie";

        [FunctionName("SetCookie")]
        public static async Task<IActionResult> SetCookie(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Setting Cookie...");

            try
            {

                var payload = new SamplePOCO()
                {
                    Name = "Bob Smith",
                    Age = 32
                };

                var cookieText = JsonConvert.SerializeObject(payload);

                req.HttpContext.Response.Cookies.Append(CookieName, cookieText, new CookieOptions()
                {
                    Expires = DateTime.Now.AddMinutes(60)
                });


                return new RedirectResult("/");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Unhandled error in SetCookie");
                return new OkObjectResult($"Error: Unhandled exception in SetCookie: {ex.ToString()}");
            }
        }

        [FunctionName("GetCookie")]
        public static async Task<IActionResult> GetCookie(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Getting Cookie...");

            try
            {
                var foundCookie = req.Cookies.TryGetValue(CookieName, out var cookieText);
                if (!foundCookie)
                {
                    return new OkObjectResult($"Could not find cookie with name {CookieName}");
                }

                log.LogInformation(cookieText);
                var cookieObj = JsonConvert.DeserializeObject<SamplePOCO>(cookieText);

                return new OkObjectResult(cookieObj);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Unhandled error in GetCookie");
                return new OkObjectResult($"Error: Unhandled exception in GetCookie: {ex.ToString()}");
            }
        }
    }
}
