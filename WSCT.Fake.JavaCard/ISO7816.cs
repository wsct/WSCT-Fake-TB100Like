namespace WSCT.Fake.JavaCard
{
    public class ISO7816
    {
        public const byte INS_SELECT = (byte)0xA4;
        public const short OFFSET_INS = 1;
        public const short OFFSET_P1 = 2;
        public const short OFFSET_P2 = 3;
        public const short OFFSET_LC = 4;
        public const short OFFSET_CDATA = 5;

        public const short CLA_ISO7816 = 0;

        public const short SW_CONDITIONS_NOT_SATISFIED = 0x6985;
        public const short SW_DATA_INVALID = 0x6983;
        public const short SW_FILE_FULL = 0x6A84;
        public const short SW_FILE_NOT_FOUND = 0x6A82;
        public const short SW_INS_NOT_SUPPORTED = 0x6D00;
        public const short SW_WRONG_DATA = 0x6A80;
        public const short SW_WRONG_LENGTH = 0x6700;
        public const short SW_WRONG_P1P2 = 0x6B00;
    }
}
