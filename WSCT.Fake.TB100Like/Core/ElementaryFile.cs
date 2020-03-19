using WSCT.Fake.JavaCard;

namespace WSCT.Fake.TB100Like.Core
{
    /// <summary>
    /// Elementary File implementation inspired by TB100.
    /// </summary>
    internal class ElementaryFile : File
    {
        /// <summary>
        /// Creates a new empty EF.
        /// </summary>
        /// <param name="fileSystem">The file system instance this DF belongs.</param>
        public ElementaryFile(FileSystem fileSystem) : base(fileSystem)
        {
        }

        #region >> File

        public override bool IsAvailable(short localWordOffset, short length)
        {
            short offset = (short)(GetInMemoryOffset((short)(localWordOffset << 2)) >> 2);

            return _fileSystem.GetFreeLength(offset, (short)(offset + length)) == length;
        }

        public override bool IsDF() => false;

        public override bool IsEF() => true;

        protected override void ClearInternals()
        {
            // TODO Clear used space.
        }

        #endregion

        /// <summary>
        /// Erases <paramref name="length"/> words of data starting from <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset of the first word of data to erase (WORDS).</param>
        /// <param name="length">Number of words to erase (WORDS).</param>
        /// <returns>offset + length</returns>
        public short Erase(short offset, short length)
        {
            _fileSystem.Erase(GetInMemoryOffset((short)(offset << 2)), (short)(length << 2));

            return (short)(offset + length);
        }

        /// <summary>
        /// Reads a sequence of words from the file.
        /// </summary>
        /// <param name="offset">Offset in the body where to start the reading (WORD).</param>
        /// <param name="output">Output buffer.</param>
        /// <param name="outputOffset">Offset in the output buffer where to write the data (BYTES).</param>
        /// <param name="length">Length of the data to read (WORDS).</param>
        /// <param name="secureRead"></param>
        /// <returns>offset + length.</returns>
        public short Read(short offset, byte[] output, short outputOffset, short length, bool secureRead)
        {
            _fileSystem.Read(GetInMemoryOffset((short)(offset << 2)), output, outputOffset, (short)(length << 2), secureRead);

            return (short)(offset + length);
        }

        /// <summary>
        /// Search consecutive bytes in file body.
        /// </summary>
        /// <param name="offset">Offset of the first word to test in file body (WORDS).</param>
        /// <param name="value">Value to search in memory range.</param>
        /// <param name="valueOffset">Offset of the first byte to test in value (BYTES).</param>
        /// <param name="valueLength">Length of the searched value (BYTES).</param>
        /// <returns>Offset of the first occurrence of value in file body (WORDS).</returns>
        public short Search(short offset, byte[] value, short valueOffset, short valueLength)
        {
            if ((short)(GetHeaderSize() + offset) > _length || offset < 0)
            {
                throw new ArrayIndexOutOfBoundsException();
            }

            short searchResult = _fileSystem.Search(
                    (short)(GetInMemoryOffset((short)(offset << 2)) >> 2),
                    (short)(_length - offset),
                    value, valueOffset, valueLength);

            if (searchResult == -1)
            {
                return searchResult;
            }

            return (short)(searchResult - (GetInMemoryOffset(0) >> 2));
        }

        /// <summary>
        /// Writes a sequence of words to the file.
        /// </summary>
        /// <param name="source">Buffer containing the data to write.</param>
        /// <param name="sourceOffset">Offset of the data in previous buffer (BYTES).</param>
        /// <param name="offset">Offset in the body where to write the data (WORDS).</param>
        /// <param name="length">Length of the data to write (WORDS).</param>
        /// <returns>offset + length</returns>
        public short Write(byte[] source, short sourceOffset, short offset, short length)
        {
            _fileSystem.Write(source, sourceOffset, GetInMemoryOffset((short)(offset << 2)), (short)(length << 2));

            return (short)(offset + length);
        }

    }
}