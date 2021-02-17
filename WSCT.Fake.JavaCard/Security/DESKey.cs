namespace WSCT.Fake.JavaCard.Security
{
    public class DESKey : SecretKey
    {
        private byte[] _keyStore;

        private readonly byte _keyType;
        private readonly bool _keyEncryption;
        private bool _isInitialized = false;

        internal DESKey(byte keyType, short keyLength, bool keyEncryption)
        {
            _keyType = keyType;
            _keyEncryption = keyEncryption;
            _keyStore = new byte[keyLength / 8];
        }

        #region >> SecretKey

        public void clearKey()
        {
            _keyStore = null;
            _isInitialized = false;
        }

        public short getSize() => (short)_keyStore.Length;

        public bool isInitialized() => _isInitialized;

        byte Key.getType() => _keyType;

        #endregion

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public byte getKey(byte[] buffer, short offset)
        {
            Util.arrayCopyNonAtomic(_keyStore, 0, buffer, offset, (short)_keyStore.Length);
            return (byte)_keyStore.Length;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public void setKey(byte[] buffer, short offset)
        {
            Util.arrayCopyNonAtomic(buffer, offset, _keyStore, 0, (short)_keyStore.Length);
            _isInitialized = true;
        }
    }
}
