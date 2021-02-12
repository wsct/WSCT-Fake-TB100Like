using System;

namespace WSCT.Fake.JavaCard.Security
{
    public abstract class Cipher
    {
        public const byte ALG_DES_CBC_NOPAD = 1;

        public const byte ALG_DES_CBC_ISO9797_M1 = 2;

        public const byte ALG_DES_CBC_ISO9797_M2 = 3;

        public const byte ALG_DES_CBC_PKCS5 = 4;

        public const byte ALG_DES_ECB_NOPAD = 5;

        public const byte ALG_DES_ECB_ISO9797_M1 = 6;

        public const byte ALG_DES_ECB_ISO9797_M2 = 7;

        public const byte ALG_DES_ECB_PKCS5 = 8;

        public const byte ALG_RSA_ISO14888 = 9;

        public const byte ALG_RSA_PKCS1 = 10;

        public const byte ALG_RSA_ISO9796 = 11;

        public const byte ALG_RSA_NOPAD = 12;

        public const byte ALG_AES_BLOCK_128_CBC_NOPAD = 13;

        public const byte ALG_AES_BLOCK_128_ECB_NOPAD = 14;

        public const byte ALG_RSA_PKCS1_OAEP = 15;

        public const byte ALG_KOREAN_SEED_ECB_NOPAD = 16;

        public const byte ALG_KOREAN_SEED_CBC_NOPAD = 17;

        public const byte MODE_DECRYPT = 1;

        public const byte MODE_ENCRYPT = 2;

        public static Cipher GetInstance(byte algorithm, bool externalAccess)
        {
            switch (algorithm)
            {
                case ALG_RSA_NOPAD:
                    return new RSANoPadCipher(algorithm, externalAccess);
                case ALG_DES_ECB_NOPAD:
                    return new DESECBNoPadCipher(algorithm, externalAccess);
                default:
                    throw new ISOException(ISO7816.SW_UNKNOWN);
            }
        }

        public abstract byte GetAlgorithm();

        public abstract void Init(Key key, byte mode);

        public abstract void DoFinal(byte[] inBuffer, short inOffset, short inLength, byte[] outBuffer, short outOffset);
    }
}
