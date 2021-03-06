﻿using System.Security.Cryptography;

namespace WSCT.Fake.JavaCard.Security
{
    internal class SHAMessageDigest : MessageDigest
    {
        private byte _algorithm;
        private bool _externalAccess;
        private HashAlgorithm _hash;

        public SHAMessageDigest(byte algorithm, bool externalAccess)
        {
            _algorithm = algorithm;
            _externalAccess = externalAccess;

            _hash = SHA1.Create();
        }

        #region >> MessageDigest

        public override void doFinal(byte[] inBuffer, short inOffset, short inLength, byte[] outBuffer, short outOffset)
        {
            _hash.ComputeHash(inBuffer, inOffset, inLength);
            Util.arrayCopyNonAtomic(_hash.Hash, 0, outBuffer, outOffset, (short)(_hash.HashSize / 8));
        }

        public override byte getAlgorithm() => _algorithm;

        public override byte getLength() => (byte)_hash.HashSize;

        public override void reset() => _hash.Initialize();

        #endregion
    }
}