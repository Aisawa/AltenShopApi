using System.Net;
using System.Text.Json.Serialization;

namespace AltenShopApi.Models
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("statusCode")]
        public HttpStatusCode StatusCode { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        public static ApiResponse<T> CreateSuccess(T data)
        {
            return new ApiResponse<T>
            {
                Success = true,
                StatusCode = HttpStatusCode.OK,
                Data = data
            };
        }

        public static ApiResponse<T> CreateError(HttpStatusCode statusCode, string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = statusCode,
                Message = message
            };
        }
    }
}
