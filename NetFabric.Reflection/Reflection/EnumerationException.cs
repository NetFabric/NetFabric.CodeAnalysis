using System;

namespace NetFabric.Reflection
{
    public class EnumerationException
        : Exception
    {
        public EnumerationException(string message, Exception innerException)
            : base (message, innerException)
        {
            
        }
    }
}