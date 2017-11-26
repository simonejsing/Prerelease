using System;

namespace Contracts
{
    internal class UnresolvedBindingException : Exception
    {
        public UnresolvedBindingException()
        {
        }

        public UnresolvedBindingException(string message) : base(message)
        {
        }

        public UnresolvedBindingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}