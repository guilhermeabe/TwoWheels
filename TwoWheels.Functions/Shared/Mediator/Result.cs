namespace TwoWheels.Functions.Shared.Mediator
{
    public class Result
    {
        public bool IsSuccess { get; protected set; }
        public string Message { get; protected set; }
        public List<string> Errors { get; protected set; }

        protected Result(bool isSuccess, string message, List<string> errors)
        {
            IsSuccess = isSuccess;
            Message = message;
            Errors = errors;
        }

        public static Result Success(string message = "") => new(true, message, []);
        public static Result Failure(string message, List<string> errors) => new(false, message, errors);
        public static Result Failure(string message) => new(false, message, [message]);
    }

    public class Result<T> : Result
    {
        public T? Data { get; private set; }

        private Result(bool isSuccess, T? data, string message, List<string> errors)
            : base(isSuccess, message, errors)
        {
            Data = data;
        }

        public static Result<T> Success(T data, string message = "") => new(true, data, message, []);
        public static new Result<T> Failure(string message, List<string> errors) => new(false, default, message, errors);
        public static new Result<T> Failure(string message) => new(false, default, message, [message]);
    }
}
