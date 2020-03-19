using System;
using System.Runtime.Serialization;

namespace WSCT.Fake.JavaCard
{
    [Serializable]
    public class ArrayIndexOutOfBoundsException : Exception
    {
        public ArrayIndexOutOfBoundsException()
        {
        }

        public ArrayIndexOutOfBoundsException(string message) : base(message)
        {
        }

        public ArrayIndexOutOfBoundsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ArrayIndexOutOfBoundsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}