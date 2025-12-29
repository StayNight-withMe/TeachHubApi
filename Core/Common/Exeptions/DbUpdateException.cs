using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
