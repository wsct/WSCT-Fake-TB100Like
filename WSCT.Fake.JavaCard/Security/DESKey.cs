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

        public void ClearKey()
        {
            _keyStore = null;
            _isInitialized = false;
        }

        public short GetSize() => (short)_keyStore.Length;

        public bool IsInitialized() => _isInitialized;

        byte Key.GetType() => _keyType;

        #endregion

        public byte GetKey(byte[] buffer, short offset)
        {
            Util.ArrayCopyNonAtomic(_keyStore, 0, buffer, offset, (short)_keyStore.Length);
            return (byte)_keyStore.Length;
        }

        public void SetKey(byte[] buffer, short offset)
        {
            Util.ArrayCopyNonAtomic(buffer, offset, _keyStore, 0, (short)_keyStore.Length);
            _isInitialized = true;
        }
    }
}
