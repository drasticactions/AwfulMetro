using System;

namespace BusinessObjects.Manager
{
    public class LoginFailedException : Exception
    {
        public LoginFailedException() 
        {
        }
        public LoginFailedException(string message) : base(message)
        {
        }
    }
}