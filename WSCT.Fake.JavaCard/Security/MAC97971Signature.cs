using System.Security.Cryptography;

namespace WSCT.Fake.JavaCard.Security
{
    internal class MAC97971Signature : Signature
    {
        private byte _algorithm;
        private bool _externalAccess;
        private SymmetricAlgorithm _cipher;
        private ICryptoTransform _transform;

        public MAC97971Signature(byte algorithm, bool externalAccess)
        {
            _algorithm = algorithm;
            _externalAccess = externalAccess;

            _cipher = TripleDES.Create();
            _cipher.Mode = CipherMode.ECB;
            _cipher.Padding = PaddingMode.Zeros;
        }

        #region >> Signature

        public override byte GetAlgorithm() => _algorithm;

        public override void Init(Key key, byte mode)
        {
            ((DESKey)key).GetKey(_cipher.Key, 0);

            switch (mode)
            {
                case Signature.MODE_SIGN:
                    _transform = _cipher.CreateEncryptor();
                    break;
                default:
                    throw new ISOException(ISO7816.SW_UNKNOWN);
            }
        }

        public override void Sign(byte[] inBuffer, short inOffset, short inLength, byte[] outBuffer, short outOffset)
        {
            var result = _transform.TransformFinalBlock(inBuffer, inOffset, inLength);
            Util.ArrayCopyNonAtomic(result, 0, outBuffer, outOffset, 8);
        }

        #endregion
    }
}