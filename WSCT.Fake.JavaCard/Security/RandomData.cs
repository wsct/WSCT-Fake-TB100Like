namespace WSCT.Fake.JavaCard.Security
{
    public abstract class RandomData
    {
        #region >> Constants

        public const byte ALG_PSEUDO_RANDOM = 1;

        public const byte ALG_SECURE_RANDOM = 2;

        #endregion

        public static RandomData GetInstance(byte algorithm)
        {
            switch (algorithm)
            {
                case ALG_SECURE_RANDOM:
                    return new SecureRandom();
                default:
                    throw new ISOException(ISO7816.SW_UNKNOWN);
            }
        }

        public abstract void GenerateData(byte[] buffer, short offset, short outLength);

        public abstract void SetSeed(byte[] buffer, short offset, short length);
    }
}
