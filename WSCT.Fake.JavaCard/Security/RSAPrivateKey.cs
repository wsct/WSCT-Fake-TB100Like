using System;

namespace WSCT.Fake.JavaCard.Security
{
    // TODO
    public class RSAPrivateKey : SecretKey
    {
        private byte[] _exponentStore;
        private byte[] _modulusStore;

        private byte _keyType;
        private short _keyLength;
        private bool _keyEncryption;
        private bool _hasModulus;
        private bool _hasExponent;

        public RSAPrivateKey(byte keyType, short keyLength, bool keyEncryption)
        {
            _keyType = keyType;
            _keyLength = keyLength;
            _keyEncryption = keyEncryption;

            _exponentStore = new byte[keyLength];
            _modulusStore = new byte[keyLength];

            _hasModulus = false;
        }

        #region >> SecretKey
        public void clearKey()
        {
            Util.arrayFillNonAtomic(_exponentStore, 0, _keyLength, 0x00);
            Util.arrayFillNonAtomic(_modulusStore, 0, _keyLength, 0x00);
            _hasModulus = false;
            _hasExponent = false;
        }

        public short getSize() => _keyLength;

        public bool isInitialized() => _hasExponent && _hasModulus;

        byte Key.getType() => _keyType;

        #endregion

        internal byte[] Modulus { get => (byte[])_modulusStore.Clone(); }

        internal byte[] Exponent { get => (byte[])_exponentStore.Clone(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public void setExponent(byte[] exponent, short offset, short length)
        {
            Util.arrayCopyNonAtomic(exponent, offset, _exponentStore, 0, length);
            _hasExponent = true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public void setModulus(byte[] modulus, short offset, short length)
        {
            Util.arrayCopyNonAtomic(modulus, offset, _modulusStore, 0, length);
            _hasModulus = true;
        }
    }
}
