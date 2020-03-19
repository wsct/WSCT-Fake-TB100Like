using WSCT.Fake.JavaCard;

namespace WSCT.Fake.TB100Like
{
    public class HeaderParser
    {
        public const byte FILETYPE_UNKNOWN = (byte)0xFF;
        public const byte FILETYPE_DF = (byte)0x3D;
        public const byte FILETYPE_EFWZ = (byte)0x2D;
        public const byte FILETYPE_EFSZ = (byte)0x0C;

        /**
         * File type: one of <c>Constants.FILETYPE_*</>.
         */
        public byte fileType;
        /**
         * Length of the header (BYTES).
         */
        public byte headerLength;
        /**
         * Length of file body (WORDS).
         */
        public short bodyLength;
        /**
         * FID of the file.
         */
        public short fileIdentifier;

        /**
         * Parses the header starting at offset in header byte array.
         *
         * @param buffer Buffer containing the header.
         * @param offset Offset of the first byte of the header in the buffer.
         * @param length Length of the buffer (BYTES).
         * 
         * @return true if successful
         */
        public bool Parse(byte[] buffer, short offset, short length)
        {
            fileType = FILETYPE_UNKNOWN;

            byte qualifier = buffer[offset];
            if ((qualifier & FILETYPE_DF) == FILETYPE_DF)
            {
                if ((short)(length - offset) < 8)
                {
                    return false;
                }
                fileType = FILETYPE_DF;
                headerLength = 8;
                bodyLength = (short)(Util.GetShort(buffer, (short)(offset + 2)) - 2);
            }
            else if ((qualifier & FILETYPE_EFWZ) == FILETYPE_EFWZ)
            {
                if ((short)(length - offset) < 8)
                {
                    return false;
                }
                fileType = FILETYPE_EFWZ;
                headerLength = 8;
                bodyLength = (short)(Util.GetShort(buffer, (short)(offset + 2)) - 2);
            }
            else if ((qualifier & FILETYPE_EFSZ) == FILETYPE_EFSZ)
            {
                if ((short)(length - offset) < 4)
                {
                    return false;
                }
                fileType = FILETYPE_EFSZ;
                headerLength = 4;
                bodyLength = (short)((buffer[(short)(offset + 2)] & 0x0F) - 1);
            }
            else
            {
                return false;
            }

            fileIdentifier = Util.GetShort(buffer, offset);

            return true;
        }
    }
}
