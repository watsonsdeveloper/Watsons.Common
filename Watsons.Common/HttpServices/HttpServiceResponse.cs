using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Watsons.Common.HttpHelpers
{
    public class HttpServiceResponse<T>
    {
        public bool IsSuccess { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
        public T? Data { get; set; }

        public static HttpServiceResponse<T> Success(T data, HttpStatusCode statusCode = HttpStatusCode.OK) 
            => new HttpServiceResponse<T> { Data = data, StatusCode = statusCode, IsSuccess = true };

        public static HttpServiceResponse<T> Fail(string errorMessage, HttpStatusCode statusCode)
            => new HttpServiceResponse<T> { ErrorMessage = errorMessage, StatusCode = statusCode, IsSuccess = false };
    }
}
