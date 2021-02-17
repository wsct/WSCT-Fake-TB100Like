using System.Security.Cryptography;

namespace WSCT.Fake.JavaCard.Security
{
    internal class SecureRandom : RandomData
    {
        private RandomNumberGenerator _random;

        public SecureRandom()
        {
            _random = RandomNumberGenerator.Create();
        }

        #region >> RandomData

        public override void generateData(byte[] buffer, short offset, short length)
        {
            _random.GetBytes(buffer, offset, length);
        }

        public override void setSeed(byte[] buffer, short offset, short length)
        {
            // Nothing to do
        }

        #endregion
    }
}