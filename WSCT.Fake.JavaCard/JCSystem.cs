using System.Collections.Generic;

namespace WSCT.Fake.JavaCard
{
    public class JCSystem
    {
        public const byte CLEAR_ON_DESELECT = 2;
        public const byte CLEAR_ON_RESET = 1;

        private static readonly List<byte[]> ClearOnDeselectTransientArrays = new();
        private static readonly List<byte[]> ClearOnResetTransientArrays = new();

        /// <summary>
        /// Creates a transient byte array with the specified array length.
        /// </summary>
        /// <param name="length">Length of the byte array</param>
        /// <param name="">The CLEAR_ON... event which causes the array elements to be cleared</param>
        /// <returns>The new transient byte array</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public static byte[] makeTransientByteArray(short length, byte eventByte)
        {
            var transientArray = new byte[length];

            switch (eventByte)
            {
                case CLEAR_ON_DESELECT:
                    ClearOnDeselectTransientArrays.Add(transientArray);
                    break;
                case CLEAR_ON_RESET:
                    ClearOnResetTransientArrays.Add(transientArray);
                    break;
                default:
                    ISOException.throwIt(ISO7816.SW_UNKNOWN);
                    break;
            }

            return transientArray;
        }

        /// <summary>
        /// Clears the transient arrays created with <see cref="CLEAR_ON_DESELECT"/>.
        /// </summary>
        internal static void ClearTransientOnDeselect()
        {
            foreach (var transientArray in ClearOnDeselectTransientArrays)
            {
                InitializeByteArray(transientArray);
            }
        }

        /// <summary>
        /// Clears the transient arrays created with <see cref="CLEAR_ON_RESET"/>.
        /// </summary>
        internal static void ClearTransientOnReset()
        {
            foreach (var transientArray in ClearOnResetTransientArrays)
            {
                InitializeByteArray(transientArray);
            }
        }

        private static void InitializeByteArray(byte[] array)
        {
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = 0;
            }
        }
    }
}
