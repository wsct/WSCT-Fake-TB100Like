namespace WSCT.Fake.Core
{
    public static class StatusWord
    {
        /// <summary>
        /// Status word for "Normal processing".
        /// </summary>
        public static byte[] Normal
        {
            get => new byte[] { 0x90, 0x00 };
        }

        /// <summary>
        /// Status word for "Class not supported".
        /// </summary>
        public static byte[] ClaNotSupported
        {
            get => new byte[] { 0x6E, 0x00 };
        }

        /// <summary>
        /// Status word for "Instruction code not supported or invalid".
        /// </summary>
        public static byte[] InsNotSupported
        {
            get => new byte[] { 0x6D, 0x00 };
        }
    }
}
