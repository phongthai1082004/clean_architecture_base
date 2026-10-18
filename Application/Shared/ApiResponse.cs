namespace Application.Shared
{
    public class ApiResponse<T>
    {
        public bool Succeeded { get; init; }

        public int StatusCode { get; init; }

        public string? Message { get; init; }

        public T? Data { get; init; }

        public IReadOnlyList<string>? Errors { get; init; }

        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        protected ApiResponse() { }

        public static ApiResponse<T> Success(T data, string? message = null, int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Succeeded = true,
                StatusCode = statusCode,
                Message = message ?? "Success.",
                Data = data
            };
        }

        public static ApiResponse<T> Success(string? message = null, int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Succeeded = true,
                StatusCode = statusCode,
                Message = message ?? "Success.",
                Data = default
            };
        }

        public static ApiResponse<T> Failure(string errorMessage, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Succeeded = false,
                StatusCode = statusCode,
                Message = errorMessage,
                Errors = [errorMessage]
            };
        }

        public static ApiResponse<T> Failure(IEnumerable<string> errors, string message = "Invalid request.", int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Succeeded = false,
                StatusCode = statusCode,
                Message = message,
                Errors = errors.ToList().AsReadOnly()
            };
        }
    }

    public class ApiResponse : ApiResponse<object>
    {
        public static new ApiResponse Success(string? message = null, int statusCode = 200)
        {
            return new ApiResponse
            {
                Succeeded = true,
                StatusCode = statusCode,
                Message = message ?? "Success."
            };
        }

        public static new ApiResponse Failure(string errorMessage, int statusCode = 400)
        {
            return new ApiResponse
            {
                Succeeded = false,
                StatusCode = statusCode,
                Message = errorMessage,
                Errors = [errorMessage]
            };
        }

        public static new ApiResponse Failure(IEnumerable<string> errors, string message = "Invalid request.", int statusCode = 400)
        {
            return new ApiResponse
            {
                Succeeded = false,
                StatusCode = statusCode,
                Message = message,
                Errors = errors.ToList().AsReadOnly()
            };
        }
    }
}