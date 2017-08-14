using System;

namespace RfidConsole
{
    public class LinkageException : Exception
    {
        public LinkageException()
        {
        }

        public LinkageException(string message)
            : base(message)
        {
        }

        public LinkageException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
