using System;

namespace WSCT.Fake.JavaCard
{
    /// <summary>
    /// ISOException class encapsulates an ISO 7816-4 response status word as its reason code.
    /// </summary>
    public class ISOException : Exception
    {
        /// <summary>
        /// Status word of the exception.
        /// </summary>
        public short StatusWord { get; set; }

        /// <summary>
        /// Constructs an ISOException instance with the specified status word.
        /// </summary>
        /// <param name="statusWord">ISO 7816-4 defined status word.</param>
        public ISOException(short statusWord) : base($"JavaCard ISOException throws with SW: {statusWord}")
        {
            StatusWord = statusWord;
        }


        /// <summary>
        /// Throws the Java Card runtime environment-owned instance of the ISOException class with the specified status word.
        /// </summary>
        /// <param name="sw">ISO 7816-4 defined status word.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public static void throwIt(short sw)
        {
            throw new ISOException(sw);
        }
    }
}
