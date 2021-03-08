using System;

namespace WSCT.Fake.JavaCard
{
    /// <summary>
    /// ISOException class encapsulates an ISO 7816-4 response status word as its reason code.
    /// </summary>
    public class ISOException : CardRuntimeException
    {
        /// <summary>
        /// Constructs an ISOException instance with the specified status word.
        /// </summary>
        /// <param name="sw">ISO 7816-4 defined status word.</param>
        public ISOException(short sw) : base(sw)
        {
        }


        /// <summary>
        /// Throws the Java Card runtime environment-owned instance of the ISOException class with the specified status word.
        /// </summary>
        /// <param name="sw">ISO 7816-4 defined status word.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public new static void throwIt(short sw)
        {
            throw new ISOException(sw);
        }
    }
}
