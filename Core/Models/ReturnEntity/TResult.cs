namespace Core.Models.ReturnEntity
{

    public abstract class EntityOfTResult
    {
        public errorCode ErrorCode { get; set; }
        public bool IsCompleted { get; set; }
        //public string? ErrorMessage { get; set; }
    }


    public class TResult<T> : EntityOfTResult
    {
        internal TResult() { }        
        public T? Value { get; set; }
        public static TResult<T> CompletedOperation(T value)
            => new TResult<T> { Value = value, IsCompleted = true, ErrorCode = errorCode.None};
        public static TResult<T> FailedOperation(errorCode errorCode, string MessageForUser = null)
            => new TResult<T> {IsCompleted = false, ErrorCode = errorCode };

        public static implicit operator TResult(TResult<T> value)
        {
            return new TResult { ErrorCode = value.ErrorCode, IsCompleted = value.IsCompleted};
        }

    }


    public class TResult : EntityOfTResult
    {
        public static TResult CompletedOperation() => new TResult {IsCompleted = true, ErrorCode = errorCode.None };
        public static TResult FailedOperation(errorCode errorCode, string message = null)
            => new TResult{ IsCompleted = false, ErrorCode = errorCode };
 
    }

}
