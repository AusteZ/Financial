using Microsoft.AspNetCore.SignalR.Protocol;

namespace Financial.Models
{
    public class ExceptionLoggingException : Exception
    {
        const string exceptionMessage = "There has been an error logging exceptions to file";

        public ExceptionLoggingException() :
            base(exceptionMessage)
        { }

        public ExceptionLoggingException(string auxMessage) :
            base(String.Format("{0} - {1}",
                exceptionMessage, auxMessage))
        { }

        public ExceptionLoggingException(string auxMessage, Exception inner) :
        base(String.Format("{0} - {1}", exceptionMessage, auxMessage), inner)
        { }
    }
}
