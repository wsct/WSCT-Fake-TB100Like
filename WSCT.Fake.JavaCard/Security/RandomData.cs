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
            return algorithm switch
            {
                ALG_SECURE_RANDOM => new SecureRandom(),
                _ => throw new ISOException(ISO7816.SW_UNKNOWN),
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public abstract void generateData(byte[] buffer, short offset, short outLength);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public abstract void setSeed(byte[] buffer, short offset, short length);
    }
}
