using System;

namespace WSCT.Fake.JavaCard
{
    /// <summary>
    /// The Util class contains common utility functions. Some of the methods may be implemented as native functions for performance reasons. All methods in Util, class are static methods.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Compares an array from the specified source array, beginning at the specified position, with the specified position of the destination array from left to right. Returns the ternary result of the comparison : less than(-1), equal(0) or greater than(1).
        /// </summary>
        /// <param name="src">Source byte array.</param>
        /// <param name="srcOff">Offset within source byte array to start compare.</param>
        /// <param name="dest">Destination byte array.</param>
        /// <param name="destOff">Offset within destination byte array to start compare.</param>
        /// <param name="length">Byte length to be compared.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public static byte arrayCompare(byte[] src, short srcOff, byte[] dest, short destOff, short length)
        {
            if (src == null || dest == null)
            {
                throw new NullPointerException();
            }
            if (srcOff < 0 || destOff < 0 || length < 0 || srcOff + length > src.Length || destOff + length > dest.Length)
            {
                throw new ArrayIndexOutOfBoundsException();
            }
            short index = 0;
            int byteCompareResult;
            do
            {
                byteCompareResult = src[srcOff + index] - dest[destOff + index];
                index++;
            } while (index < length && byteCompareResult == 0);

            return (byte)byteCompareResult;
        }

        /// <summary>
        /// Fills the byte array (non-atomically) beginning at the specified position, for the specified length with the specified byte value.
        /// </summary>
        /// <param name="array">Byte array.</param>
        /// <param name="offset">Offset within byte array to start filling bValue into.</param>
        /// <param name="length">Number of bytes to be filled.</param>
        /// <param name="value">Value to fill the byte array with.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public static void arrayFillNonAtomic(byte[] array, short offset, short length, byte value)
        {
            for (int i = offset; i < offset + length; i++)
            {
                array[i] = value;
            }
        }

        /// <summary>
        /// Copies an array from the specified source array, beginning at the specified position, to the specified position of the destination array (non-atomically).
        /// </summary>
        /// <param name="source">Source byte array.</param>
        /// <param name="sourceOffset">Offset within source byte array to start copy from.</param>
        /// <param name="output">Destination byte array.</param>
        /// <param name="outputOffset">Offset within destination byte array to start copy into.</param>
        /// <param name="writtenLength">Byte length to be copied</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public static void arrayCopyNonAtomic(byte[] source, short sourceOffset, byte[] output, short outputOffset, short writtenLength)
        {
            Array.Copy(source, sourceOffset, output, outputOffset, writtenLength);
        }

        /// <summary>
        /// Copies an array from the specified source array, beginning at the specified position, to the specified position of the destination array (atomicity is not honored by this implementation).
        /// </summary>
        /// <param name="source">Source byte array.</param>
        /// <param name="sourceOffset">Offset within source byte array to start copy from.</param>
        /// <param name="output">Destination byte array.</param>
        /// <param name="outputOffset">Offset within destination byte array to start copy into.</param>
        /// <param name="writtenLength">Byte length to be copied</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public static void arrayCopy(byte[] source, short sourceOffset, byte[] output, short outputOffset, short writtenLength)
        {
            Array.Copy(source, sourceOffset, output, outputOffset, writtenLength);
        }

        /// <summary>
        /// Concatenates the two parameter bytes to form a short value.
        /// </summary>
        /// <param name="b1">first byte (high order byte).</param>
        /// <param name="b2">second byte (low order byte).</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public static short makeShort(byte b1, byte b2)
        {
            return (short)(((b1 & 0xFF) << 8) | (b2 & 0xFF));
        }

        /// <summary>
        /// Concatenates two bytes in a byte array to form a short value.
        /// </summary>
        /// <param name="bArray">Byte array.</param>
        /// <param name="bOff">Offset within byte array containing first byte (the high order byte).</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public static short getShort(byte[] bArray, short bOff)
        {
            return makeShort(bArray[bOff], bArray[bOff + 1]);
        }

        /// <summary>
        /// Deposits the short value as two successive bytes at the specified offset in the byte array.
        /// </summary>
        /// <param name="bArray">Byte array.</param>
        /// <param name="bOff">Offset within byte array to deposit the first byte (the high order byte)</param>
        /// <param name="sValue">Value to set into array</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public static short setShort(byte[] bArray, short bOff, short sValue)
        {
            bArray[bOff] = (byte)((sValue >> 8) & 0xFF);
            bOff++;
            bArray[bOff] = (byte)(sValue & 0xFF);
            bOff++;
            return bOff;
        }
    }
}
