using System.Security.Cryptography;

namespace WSCT.Fake.JavaCard.Security
{
    internal class DESECBNoPadCipher : Cipher
    {
        private byte _algorithm;
        private SymmetricAlgorithm _cipher;
        private ICryptoTransform _transform;

        public DESECBNoPadCipher(byte algorithm, bool _ /* externalAccess */)
        {
            _algorithm = algorithm;

            _cipher = TripleDES.Create();
            _cipher.Mode = CipherMode.ECB;
            _cipher.Padding = PaddingMode.None;
        }

        #region >> Cipher

        public override void DoFinal(byte[] inBuffer, short inOffset, short inLength, byte[] outBuffer, short outOffset)
        {
            var result = _transform.TransformFinalBlock(inBuffer, inOffset, inLength);
            Util.ArrayCopyNonAtomic(result, 0, outBuffer, outOffset, inLength);
        }

        public override byte GetAlgorithm() => _algorithm;

        public override void Init(Key key, byte mode)
        {
            ((DESKey)key).GetKey(_cipher.Key, 0);

            _transform = mode switch
            {
                Cipher.MODE_DECRYPT => _cipher.CreateDecryptor(),
                Cipher.MODE_ENCRYPT => _cipher.CreateEncryptor(),
                _ => throw new ISOException(ISO7816.SW_UNKNOWN)
            };
        }

        #endregion
    }
}