
using SportSphere.Domain.Enums;

namespace MagiXSquad.WebApi.Responses
{
    public class ApiResponse<T>
    {
        public T Data { get; set; } = default!;
        public int? Count { get; set; }
        public string? Message { get; set; }
        public bool Success { get; set; } = true;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public ErrorCode? ErrorCode { get; set; } = SportSphere.Domain.Enums.ErrorCode.None;
        public string RequestId { get; set; } = Guid.NewGuid().ToString();
        public ApiResponse()
        {

        }
        public ApiResponse(T data, string message = "Operation completed successfully")
        {
            Data = data;
            Message = message;
            Success = true;
            SetCountFromData();
        }

        public ApiResponse(ErrorCode errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
            Success = false;
            Count = 0;
        }


        public static ApiResponse<T> Ok(T data, string message = "Operation completed successfully")
        {
            return new ApiResponse<T>(data, message);
        }

        public static ApiResponse<T> Error(ErrorCode errorCode, string message) => new(errorCode, message);

        private void SetCountFromData()
        {
            if (Data is System.Collections.ICollection collection)
            {
                Count = collection.Count;
            }
            else if (Data is System.Collections.IEnumerable enumerable && Data is not string)
            {
                Count = enumerable.Cast<object>().Count();
            }
            else if (Data is null)
            {
                Count = 0;
            }
            else
            {
                Count = 1;
            }
        }
    }
}
