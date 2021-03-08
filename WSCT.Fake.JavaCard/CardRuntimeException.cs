using System;

namespace WSCT.Fake.JavaCard
{
    public class CardRuntimeException : Exception
    {
        private short _reason;

        /// <summary>
        /// Constructs a CardRuntimeException instance with the specified reason.
        /// </summary>
        /// <param name="reason"></param>
        public CardRuntimeException(short reason) : base($"{reason}")
        {
            setReason(reason);
        }

        /// <summary>
        /// Gets the reason code
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public short getReason() => _reason;

        /// <summary>
        /// Sets the reason code. Even if a transaction is in progress, the update of the internal reason field shall not participate in the transaction.
        /// </summary>
        /// <param name="reason"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public void setReason(short reason)
        {
            _reason = reason;
        }

        /// <summary>
        /// Throws the Java Card runtime environment-owned instance of the CardRuntimeException class with the specified reason.
        /// </summary>
        /// <param name="reason"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public static void throwIt(short reason)
        {
            throw new CardRuntimeException(reason);
        }
    }
}
