namespace WSCT.Fake.TB100Like.Core
{
    internal class Constants
    {
        public const byte INS_GENERATE_RANDOM = 0xC4;
        public const byte INS_CREATE_FILE = 0xE0;
        public const byte INS_DELETE_FILE = 0xE4;
        public const byte INS_SELECT = 0xA4;
        public const byte INS_FSEARCH = 0xA0;
        public const byte INS_READ_BINARY = 0xB0;
        public const byte INS_WRITE_BINARY = 0xD0;
        public const byte INS_ERASE = 0x0E;
        public const byte INS_GET_DATA = 0xCA;
        public const byte INS_PUT_DATA = 0xDA;

        // TODO: Remove these constants.
        public static byte P1_CREATE_FILE_DF = 0x01;
        public static byte P1_CREATE_FILE_EF = 0x02;

        /// <summary>
        /// Response status: Not enough memory space in the file = 6A84
        /// </summary>
        public const short SW_NOT_ENOUGH_SPACE_IN_FILE = 0x6A84;
        public const short SW_DATA_NOT_FOUND = 0x6A88;

        internal static readonly byte[] MF_HEADER = new byte[] { 0x3F, 0x00, 0x02, 0xA1, 0xFF, 0xFF, 0x9E, 0x81 };

        public static readonly byte[] FCI_APPLET = new byte[] { (byte)'T', (byte)'B', (byte)'1', (byte)'0', (byte)'0', (byte)'L', (byte)'I', (byte)'K', (byte)'E' };

        internal const short FILESYSTEM_SIZE = 0x02A1;
        internal const byte DF_MAX = 0x08;
        internal const byte EF_MAX = 0x10;
    }
}