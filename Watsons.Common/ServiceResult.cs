using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Watsons.Common
{
    public class ServiceResult<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; set; } = false;
        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; }

        // Factory methods for success/failure
        public static ServiceResult<T> Success(T data) => new ServiceResult<T> { Data = data, IsSuccess = true };
        public static ServiceResult<T> Fail(string message) => new ServiceResult<T> { IsSuccess = false, ErrorMessage = message };
        public static ServiceResult<T> FailureData(T data, string message) => new ServiceResult<T> { Data = data, IsSuccess = false, ErrorMessage = message,  };
    }

}
