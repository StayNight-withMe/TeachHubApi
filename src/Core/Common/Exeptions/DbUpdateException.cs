

namespace Core.Common.Exeptions
{
    public class DbUpdateException : Exception
    {
        public string ErrorCode { get; }
        public DbUpdateException(string message, string errorCode) : base(message)  
        {
            ErrorCode = errorCode;
        }
    }
}
