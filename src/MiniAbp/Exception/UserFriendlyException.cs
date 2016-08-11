using System;

namespace MiniAbp
{
    public class AuthorizationException : Exception
    {
        public bool IsNoUserLogin { get; set; }

        public AuthorizationException(string message, bool isNoUserLogin = false) : base(message)
        {
            IsNoUserLogin = isNoUserLogin;
        }
    }
}
