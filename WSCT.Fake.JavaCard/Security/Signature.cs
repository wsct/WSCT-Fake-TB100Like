using System;

namespace WSCT.Fake.JavaCard.Security
{
    public abstract class Signature
    {
        #region >> Constants 

        public const byte ALG_DES_MAC4_NOPAD = 1;

        public const byte ALG_DES_MAC8_NOPAD = 2;

        public const byte ALG_DES_MAC4_ISO9797_M1 = 3;

        public const byte ALG_DES_MAC8_ISO9797_M1 = 4;

        public const byte ALG_DES_MAC4_ISO9797_M2 = 5;

        public const byte ALG_DES_MAC8_ISO9797_M2 = 6;

        public const byte ALG_DES_MAC4_PKCS5 = 7;

        public const byte ALG_DES_MAC8_PKCS5 = 8;

        public const byte ALG_RSA_SHA_ISO9796 = 9;

        public const byte ALG_RSA_SHA_PKCS1 = 10;

        public const byte ALG_RSA_MD5_PKCS1 = 11;

        public const byte ALG_RSA_RIPEMD160_ISO9796 = 12;

        public const byte ALG_RSA_RIPEMD160_PKCS1 = 13;

        public const byte ALG_DSA_SHA = 14;

        public const byte ALG_RSA_SHA_RFC2409 = 15;

        public const byte ALG_RSA_MD5_RFC2409 = 16;

        public const byte ALG_ECDSA_SHA = 17;

        public const byte ALG_AES_MAC_128_NOPAD = 18;

        public const byte ALG_DES_MAC4_ISO9797_1_M2_ALG3 = 19;

        public const byte ALG_DES_MAC8_ISO9797_1_M2_ALG3 = 20;

        public const byte ALG_RSA_SHA_PKCS1_PSS = 21;

        public const byte ALG_RSA_MD5_PKCS1_PSS = 22;

        public const byte ALG_RSA_RIPEMD160_PKCS1_PSS = 23;

        public const byte ALG_HMAC_SHA1 = 24;

        public const byte ALG_HMAC_SHA_256 = 25;

        public const byte ALG_HMAC_SHA_384 = 26;

        public const byte ALG_HMAC_SHA_512 = 27;

        public const byte ALG_HMAC_MD5 = 28;

        public const byte ALG_HMAC_RIPEMD160 = 29;

        public const byte ALG_RSA_SHA_ISO9796_MR = 30;

        public const byte ALG_RSA_RIPEMD160_ISO9796_MR = 31;

        public const byte ALG_KOREAN_SEED_MAC_NOPAD = 32;

        public const byte MODE_SIGN = 1;

        public const byte MODE_VERIFY = 2;

        #endregion

        public static Signature GetInstance(byte algorithm, bool externalAccess)
        {
            switch (algorithm)
            {
                case ALG_DES_MAC8_ISO9797_M1:
                    return new MAC97971Signature(algorithm, externalAccess);
                default:
                    throw new ISOException(ISO7816.SW_UNKNOWN);
            }

        }

        public abstract byte GetAlgorithm();

        public abstract void Init(Key key, byte mode);

        public abstract void Sign(byte[] inBuffer, short inOffset, short inLength, byte[] outBuffer, short outOffset);
    }
}
