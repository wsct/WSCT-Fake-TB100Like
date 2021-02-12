using System;

namespace WSCT.Fake.JavaCard.Security
{
    public class KeyBuilder
    {
        #region >> Constants 

        public const byte TYPE_DES_TRANSIENT_RESET = 1;

        public const byte TYPE_DES_TRANSIENT_DESELECT = 2;

        public const byte TYPE_DES = 3;

        public const byte TYPE_RSA_PUBLIC = 4;

        public const byte TYPE_RSA_PRIVATE = 5;

        public const byte TYPE_RSA_CRT_PRIVATE = 6;

        public const byte TYPE_DSA_PUBLIC = 7;

        public const byte TYPE_DSA_PRIVATE = 8;

        public const byte TYPE_EC_F2M_PUBLIC = 9;

        public const byte TYPE_EC_F2M_PRIVATE = 10;

        public const byte TYPE_EC_FP_PUBLIC = 11;

        public const byte TYPE_EC_FP_PRIVATE = 12;

        public const byte TYPE_AES_TRANSIENT_RESET = 13;

        public const byte TYPE_AES_TRANSIENT_DESELECT = 14;

        public const byte TYPE_AES = 15;

        public const byte TYPE_KOREAN_SEED_TRANSIENT_RESET = 16;

        public const byte TYPE_KOREAN_SEED_TRANSIENT_DESELECT = 17;

        public const byte TYPE_KOREAN_SEED = 18;

        public const byte TYPE_HMAC_TRANSIENT_RESET = 19;

        public const byte TYPE_HMAC_TRANSIENT_DESELECT = 20;

        public const byte TYPE_HMAC = 21;

        public const short LENGTH_DES = 64;

        public const short LENGTH_DES3_2KEY = 128;

        public const short LENGTH_DES3_3KEY = 192;

        public const short LENGTH_RSA_512 = 512;

        public const short LENGTH_RSA_736 = 736;

        public const short LENGTH_RSA_768 = 768;

        public const short LENGTH_RSA_896 = 896;

        public const short LENGTH_RSA_1024 = 1024;

        public const short LENGTH_RSA_1280 = 1280;

        public const short LENGTH_RSA_1536 = 1536;

        public const short LENGTH_RSA_1984 = 1984;

        public const short LENGTH_RSA_2048 = 2048;

        public const short LENGTH_DSA_512 = 512;

        public const short LENGTH_DSA_768 = 768;

        public const short LENGTH_DSA_1024 = 1024;

        public const short LENGTH_EC_FP_112 = 112;

        public const short LENGTH_EC_F2M_113 = 113;

        public const short LENGTH_EC_FP_128 = 128;

        public const short LENGTH_EC_F2M_131 = 131;

        public const short LENGTH_EC_FP_160 = 160;

        public const short LENGTH_EC_F2M_163 = 163;

        public const short LENGTH_EC_FP_192 = 192;

        public const short LENGTH_EC_F2M_193 = 193;

        public const short LENGTH_AES_128 = 128;

        public const short LENGTH_AES_192 = 192;

        public const short LENGTH_AES_256 = 256;

        public const short LENGTH_KOREAN_SEED_128 = 128;

        public const short LENGTH_HMAC_SHA_1_BLOCK_64 = 64;

        public const short LENGTH_HMAC_SHA_256_BLOCK_64 = 64;

        public const short LENGTH_HMAC_SHA_384_BLOCK_128 = 128;

        public const short LENGTH_HMAC_SHA_512_BLOCK_128 = 128;

        #endregion

        public static Key BuildKey(byte keyType, short keyLength, bool keyEncryption)
        {
            switch (keyType)
            {
                case TYPE_RSA_PRIVATE:
                    return new RSAPrivateKey(keyType, (short)(keyLength / 8), keyEncryption);
                case TYPE_DES:
                    return new DESKey(keyType, 128, keyEncryption);
                default:
                    throw new ISOException(ISO7816.SW_UNKNOWN);
            }
        }
    }
}
