using System;

namespace WSCT.Fake.JavaCard.Security
{
    public abstract class MessageDigest
    {
        #region >> Constants

        public const byte ALG_SHA = 1;

        public const byte ALG_MD5 = 2;

        public const byte ALG_RIPEMD160 = 3;

        public const byte ALG_SHA_256 = 4;

        public const byte ALG_SHA_384 = 5;

        public const byte ALG_SHA_512 = 6;

        public const byte LENGTH_SHA = 20;

        public const byte LENGTH_MD5 = 16;

        public const byte LENGTH_RIPEMD160 = 20;

        public const byte LENGTH_SHA_256 = 32;

        public const byte LENGTH_SHA_384 = 48;

        public const byte LENGTH_SHA_512 = 64;

        #endregion

        public static MessageDigest GetInstance(byte algorithm, bool externalAccess)
        {
            switch (algorithm)
            {
                case ALG_SHA:
                    return new SHAMessageDigest(algorithm, externalAccess);
                default:
                    throw new ISOException(ISO7816.SW_UNKNOWN);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public abstract byte getAlgorithm();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public abstract byte getLength();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public abstract void reset();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public abstract void doFinal(byte[] inBuffer, short inOffset, short inLength, byte[] outBuffer, short outOffset);
    }
}
