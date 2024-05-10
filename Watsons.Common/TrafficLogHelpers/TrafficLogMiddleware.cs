using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Watsons.Common.TrafficLogHelpers
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class TrafficLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerFactory _loggerFactory;
        //private readonly IDbContextFactory<TrafficContext> _dbContextFactory;
        private readonly TrafficContext _trafficContext; // using delegate factory method => registered in Program.cs


        public TrafficLogMiddleware(RequestDelegate next,
            TrafficContext trafficContext,
            //IDbContextFactory<TrafficContext> dbContextFactory,
            ILoggerFactory loggerFactory
            )
        {
            _next = next;
            _trafficContext = trafficContext;
            //_dbContextFactory = dbContextFactory;
            _loggerFactory = loggerFactory;
            
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            string requestId = Guid.NewGuid().ToString();

            var trafficLog = await FormatRequest(httpContext.Request);
            var originalBodyStream = httpContext.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                httpContext.Response.Body = responseBody;
                await _next(httpContext);

                var response = await FormatResponse(httpContext.Response, trafficLog);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<TrafficLog> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();
            request.Body.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            request.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = Encoding.UTF8.GetString(buffer);

            var absoluteUrlWithQuery = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
            TrafficLog trafficLog = new TrafficLog { AbsoluteUrlWithQuery = absoluteUrlWithQuery, Action = request.Path, RequestDT = DateTime.Now };
            if (!string.IsNullOrEmpty(bodyAsText))
            {
                trafficLog.Request = bodyAsText;
            }
            return trafficLog;
        }     

        private async Task<string> FormatResponse(HttpResponse response, TrafficLog trafficLog)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            string text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            try
            {


                if (!string.IsNullOrEmpty(text))
                {

                    trafficLog.Response = text;
                    trafficLog.ResponseDT = DateTime.Now;
                    trafficLog.HttpStatus = response.StatusCode.ToString();

                    var timeDifference = DateTime.Now - trafficLog.RequestDT;
                    trafficLog.TimeTaken = timeDifference.TotalSeconds;

                    //using var  dbContext = _dbContextFactory.CreateDbContext();
                    _trafficContext.TrafficLogs.Add(trafficLog);
                    _trafficContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return $"{response.StatusCode}:{text}";
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TrafficLogMiddleware>();
        }
    }



}
