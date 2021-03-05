using System;

namespace WSCT.Fake.JavaCard
{
    public class PINException : Exception
    {
        /// <summary>
        /// This reason code is used to indicate that one or more input parameters is out of allowed bounds.
        /// </summary>
        public const short ILLEGAL_VALUE = 1;

        /// <summary>
        /// Constructs a PINException. To conserve on resources use  <code>throwIt()</code> to employ the Java Card runtime environment-owned instance of this class.
        /// </summary>
        /// <param name="reason">the reason for the exception</param>
        public PINException(short reason) : base($"{reason}")
        {
        }

        /// <summary>
        /// * Throws the Java Card runtime environment-owned instance of
        ///         * <code>PINException</code> with the specified reason.
        ///     * <p>
        ///     * Java Card runtime environment-owned instances of exception classes are
        ///     * temporary Java Card runtime environment Entry Point Objects and can be
        ///         * accessed from any applet context. References to these temporary objects
        ///     * cannot be stored in class variables or instance variables or array
        ///         * components. See
        ///     * <em>Runtime Environment Specification for the Java Card Platform</em>,
        ///     * section 6.2.1 for details.
        /// </summary>
        /// <param name="reason">the reason for the exception</param>
        /// <exception cref="PINException">always</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public static void throwIt(short reason)
        {
            throw new PINException(reason);
        }
    }
}
