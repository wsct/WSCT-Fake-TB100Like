using WSCT.Fake.JavaCard;

namespace WSCT.Fake.TB100Like.Core
{
    internal abstract class File
    {
        protected readonly FileSystem _fileSystem;

        protected DedicatedFile _parentDF;

        /// <summary>
        /// Offset of first byte of the file in the parent (BYTES, should be a multiple of 4).
        /// </summary>
        internal short _inParentBodyOffset;

        /// <summary>
        /// Length of the file (header + body) (WORDS).
        /// </summary>
        internal short _length;

        /// <summary>
        /// Length of the header (BYTES, should be a multiple of 4).
        /// </summary>
        protected short _headerLength;

        /// <summary>
        /// Creates a new file in the given <see cref="FileSystem"/>
        /// </summary>
        /// <param name="fileSystem">FileSystem instance in which the file lives.</param>
        public File(FileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        /// <summary>
        /// Reads the first 2 bytes of the header of the file.
        /// </summary>
        /// <returns>FID of the file.</returns>
        public short GetFileId()
        {
            short inMemoryOffset = GetInMemoryOffset((short)-_headerLength);
            return Util.getShort(_fileSystem.memory, inMemoryOffset);
        }

        /// <summary>
        /// Returns the length of the file (header + body) (WORDS).
        /// </summary>
        /// <returns></returns>
        public short GetLength()
        {
            return _length;
        }

        /// <summary>
        /// Offset the file in memory (start of header).
        /// </summary>
        /// <returns></returns>
        public short GetOffset()
        {
            return _inParentBodyOffset;
        }

        /// <summary>
        /// Informs if the memory from <paramref name="localWordOffset"/> and <paramref name="length"/> following words are free.
        /// </summary>
        /// <param name="localWordOffset">Offset of the first word to be tested (WORDS)</param>
        /// <param name="length">Number of words to be tested (WORDS)</param>
        /// <returns><c>true</c> if all the words in the file between localWordOffset and localWordOffset+length are not yet written</returns>
        public abstract bool IsAvailable(short localWordOffset, short length);

        /// <summary>
        /// Informs if the file is a Dedicated File.
        /// </summary>
        /// <returns><c>true</c> if the file is an Dedicated File.</returns>
        public abstract bool IsDF();

        /// <summary>
        /// Informs if the file is an Elementary File.
        /// </summary>
        /// <returns><c>true</c> if the file is an Elementary File.</returns>
        public abstract bool IsEF();

        /// <summary>
        /// Copies the header in the output buffer.
        /// </summary>
        /// <param name="output">Output buffer.</param>
        /// <param name="offset">Offset in previous buffer where to write the header.</param>
        public void GetHeader(byte[] output, short offset)
        {
            _fileSystem.Read(GetInMemoryOffset((short)-_headerLength), output, offset, _headerLength, false);
        }

        /// <summary>
        /// Returns the size of the header (WORDS).
        /// </summary>
        /// <returns></returns>
        public short GetHeaderSize()
        {
            return (short)(_headerLength >> 2);
        }

        /// <summary>
        /// Marks the file as released. Automatically calls <see cref="ClearInternals"/> to release file specific data.
        /// </summary>
        public void Release()
        {
            ClearInternals();

            // Erase all memory related reserved by the file (header + body)
            _fileSystem.Erase(GetInMemoryOffset((short)-_headerLength), _length);

            _headerLength = 0;
            _length = 0;
        }

        /// <summary>
        /// Sets up the file for use.
        /// </summary>
        /// <param name="parentDF">Parent DF.</param>
        /// <param name="offset">Offset of the file in parent DF body (WORDS).</param>
        /// <param name="size">Size used by the file (WORDS).</param>
        /// <param name="header">Buffer containing the header of the file (header should be 32 bits aligned).</param>
        /// <param name="headerOffset">Offset of the header in previous buffer (BYTES).</param>
        /// <param name="headerLength">Length of the header (BYTES, should be a multiple of 4).</param>
        public void Setup(DedicatedFile parentDF, short offset, short size, byte[] header, short headerOffset, short headerLength)
        {
            _parentDF = parentDF;
            _inParentBodyOffset = (short)(offset << 2);
            _length = size;
            _headerLength = headerLength;

            // Write the header
            _fileSystem.Write(header, headerOffset, GetInMemoryOffset((short)(-_headerLength)), _headerLength);
        }

        /// <summary>
        /// Must be overriden to clear file specific data before file release.
        /// </summary>
        protected abstract void ClearInternals();

        /// <summary>
        /// Converts local file body offset to file system memory offset.
        /// </summary>
        /// <param name="dataOffset">Offset in file body.</param>
        /// <returns></returns>
        protected short GetInMemoryOffset(short dataOffset)
        {
            File ancestor = _parentDF;
            short offset = (short)(_inParentBodyOffset + _headerLength + dataOffset);
            while (ancestor != null)
            {
                offset += (short)(ancestor._inParentBodyOffset + ancestor._headerLength);
                ancestor = ancestor._parentDF;
            }

            return offset;
        }
    }
}
