namespace WSCT.Fake.JavaCard
{
    public class ISO7816
    {
        /// <summary>
        /// APDU header offset : CLA = 0
        /// </summary>
        public const byte OFFSET_CLA = 0;

        /// <summary>
        /// APDU command INS : SELECT = 0xA4
        /// </summary>
        public const byte INS_SELECT = (byte)0xA4;

        /// <summary>
        /// APDU header offset : INS = 1
        /// </summary>
        public const short OFFSET_INS = 1;

        /// <summary>
        /// APDU header offset : P1 = 2
        /// </summary>
        public const short OFFSET_P1 = 2;

        /// <summary>
        /// APDU header offset : P2 = 3
        /// </summary>
        public const short OFFSET_P2 = 3;

        /// <summary>
        /// APDU header offset : LC = 4
        /// </summary>
        public const short OFFSET_LC = 4;

        /// <summary>
        /// APDU command data offset : CDATA = 5
        /// </summary>
        public const short OFFSET_CDATA = 5;

        /// <summary>
        /// APDU command CLA : ISO 7816 = 0x00
        /// </summary>
        public const short CLA_ISO7816 = 0;

        /// <summary>
        /// Response status : Wrong length = 0x6700
        /// </summary>
        public const short SW_WRONG_LENGTH = 0x6700;

        /// <summary>
        /// Response status : Security condition not satisfied = 0x6982
        /// </summary>
        public const short SW_SECURITY_STATUS_NOT_SATISFIED = 0x6982;

        /// <summary>
        /// Response status : File invalid = 0x6983
        /// </summary>
        public const short SW_FILE_INVALID = 0x6983;

        /// <summary>
        /// Response status : Data invalid = 0x6984
        /// </summary>
        public const short SW_DATA_INVALID = 0x6984;

        /// <summary>
        /// Response status : Conditions of use not satisfied = 0x6985
        /// </summary>
        public const short SW_CONDITIONS_NOT_SATISFIED = 0x6985;

        /// <summary>
        /// Response status : Wrong data = 0x6A80
        /// </summary>
        public const short SW_WRONG_DATA = 0x6A80;

        /// <summary>
        /// Response status : Function not supported = 0x6A81
        /// </summary>
        public const short SW_FUNC_NOT_SUPPORTED = 0x6A81;

        /// <summary>
        /// Response status : File not found = 0x6A82
        /// </summary>
        public const short SW_FILE_NOT_FOUND = 0x6A82;

        /// <summary>
        /// Response status : Record not found = 0x6A83
        /// </summary>
        public const short SW_RECORD_NOT_FOUND = 0x6A83;

        /// <summary>
        /// Response status : Not enough memory space in the file  = 0x6A84
        /// </summary>
        public const short SW_FILE_FULL = 0x6A84;

        /// <summary>
        /// Response status : Incorrect parameters (P1,P2) = 0x6A86
        /// </summary>
        public const short SW_INCORRECT_P1P2 = 0x6A86;

        /// <summary>
        /// Response status : Incorrect parameters (P1,P2) = 0x6B00
        /// </summary>
        public const short SW_WRONG_P1P2 = 0x6B00;

        /// <summary>
        /// Response status : INS value not supported = 0x6D00
        /// </summary>
        public const short SW_INS_NOT_SUPPORTED = 0x6D00;

        /// <summary>
        /// Response status : CLA value not supported = 0x6E00
        /// </summary>
        public const short SW_CLA_NOT_SUPPORTED = 0x6E00;

        /// <summary>
        /// Response status : No precise diagnosis = 0x6F00
        /// </summary>
        public const short SW_UNKNOWN = 0x6F00;

        /// <summary>
        /// Response status : No Error = (short)0x9000
        /// </summary>
        public const short SW_NO_ERROR = unchecked((short)0x9000);
    }
}
