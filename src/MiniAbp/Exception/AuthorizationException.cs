using System;

namespace MiniAbp
{
    public class UserFriendlyException : Exception
    {
        public UserFriendlyException(string message) : base(message) { }
        public new Exception InnerException { get; set; }
    }
}
