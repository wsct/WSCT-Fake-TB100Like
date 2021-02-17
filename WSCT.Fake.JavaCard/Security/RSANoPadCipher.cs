using System;
using Org.BouncyCastle;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using WSCT.Helpers;

namespace WSCT.Fake.JavaCard.Security
{
    internal class RSANoPadCipher : Cipher
    {
        private byte _algorithm;
        private bool _externalAccess;
        private RsaEngine rsa;

        public RSANoPadCipher(byte algorithm, bool externalAccess)
        {
            _algorithm = algorithm;
            _externalAccess = externalAccess;

            rsa = new RsaEngine();
        }

        #region >> Cipher

        public override void doFinal(byte[] inBuffer, short inOffset, short inLength, byte[] outBuffer, short outOffset)
        {
            var data = new Span<byte>(inBuffer, inOffset, inLength).ToArray();
            var encrypted = rsa.ProcessBlock(data, 0, inLength);

            Util.arrayCopyNonAtomic(encrypted, 0, outBuffer, outOffset, inLength);
        }

        public override byte getAlgorithm() => _algorithm;

        public override void init(Key key, byte mode)
        {
            var rsaKey = (RSAPrivateKey)key;
            var rsaParameters = new RsaKeyParameters(true, new BigInteger(rsaKey.Modulus.ToHexa('\0'), 16), new BigInteger(rsaKey.Exponent.ToHexa('\0'), 16));

            rsa.Init(true, rsaParameters);
        }

        #endregion
    }
}