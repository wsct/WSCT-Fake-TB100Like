namespace WSCT.Fake.JavaCard
{
    public class JCSystem
    {
        public const byte CLEAR_ON_DESELECT = 2;
        public const byte CLEAR_ON_RESET = 1;


        /// <summary>
        /// Creates a transient byte array with the specified array length.
        /// </summary>
        /// <param name="length">Length of the byte array</param>
        /// <param name="">The CLEAR_ON... event which causes the array elements to be cleared</param>
        /// <returns>The new transient byte array</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public static byte[] makeTransientByteArray(short length, byte eventByte)
        {
            return new byte[length];
        }
    }
}
