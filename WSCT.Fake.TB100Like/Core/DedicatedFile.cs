namespace WSCT.Fake.TB100Like.Core
{
    /// <summary>
    /// Dedicated File implementation inspired by TB100.
    /// </summary>
    internal class DedicatedFile : File
    {
        private const byte MaxChildren = 10;
        private readonly File[] _children = new File[MaxChildren];
        private byte _childrenCount = 0;

        /// <summary>
        /// Creates a new empty DF.
        /// </summary>
        /// <param name="fileSystem">The file system instance this DF belongs.</param>
        public DedicatedFile(FileSystem fileSystem) : base(fileSystem)
        { }

        #region >> File

        public override bool IsAvailable(short localWordOffset, short length)
        {
            for (byte i = 0; i < _childrenCount; i++)
            {
                bool neverOverlap = localWordOffset > (short)((short)(_children[i].GetOffset() >> 2) + _children[i].GetLength() - 1)
                                      || localWordOffset + length - 1 < (short)(_children[i].GetOffset() >> 2);
                if (!neverOverlap)
                {
                    return false;
                }
            }

            return true;
        }

        public override bool IsDF() => true;

        public override bool IsEF() => false;

        protected override void ClearInternals()
        {
            for (byte i = 0; i < _childrenCount; i++)
            {
                DeleteFile(i);
            }
        }

        #endregion

        /// <summary>
        /// Creates a new DF starting at offset and using size bytes.
        /// </summary>
        /// <param name="offset">Offset of the start of the file in DF body (WORDS).</param>
        /// <param name="size">Size used by the file (header + body) (WORDS).</param>
        /// <param name="header">Buffer containing the header of the file.</param>
        /// <param name="headerOffset">Offset of the header in previous buffer (BYTES).</param>
        /// <param name="headerLength">Length of the header (BYTES).</param>
        /// <returns></returns>
        public DedicatedFile CreateDedicatedFile(short offset, short size, byte[] header, short headerOffset, short headerLength)
        {
            if (_childrenCount == MaxChildren)
            {
                return null;
            }

            DedicatedFile file = _fileSystem.GetFreeDF();
            if (file != null)
            {
                file.Setup(this, offset, size, header, headerOffset, headerLength);

                byte fileIndex = 0;
                while (fileIndex < _childrenCount && _children[fileIndex]._inParentBodyOffset < file._inParentBodyOffset)
                {
                    fileIndex++;
                }

                for (byte i = _childrenCount; i > fileIndex; i--)
                {
                    _children[i] = _children[(byte)(i - 1)];
                }
                _children[fileIndex] = file;
                _childrenCount++;
            }

            return file;
        }

        /// <summary>
        /// Creates a new EF starting at offset and using size bytes.
        /// </summary>
        /// <param name="offset">Offset of the start of the file in DF body (WORDS).</param>
        /// <param name="size">Size used by the file (header + body) (WORDS).</param>
        /// <param name="header">Buffer containing the header of the file.</param>
        /// <param name="headerOffset">Offset of the header in previous buffer (BYTES).</param>
        /// <param name="headerLength">Length of the header (BYTES).</param>
        /// <returns>Index of the new file.</returns>
        public ElementaryFile CreateElementaryFile(short offset, short size, byte[] header, short headerOffset, short headerLength)
        {
            if (_childrenCount == MaxChildren)
            {
                return null;
            }

            ElementaryFile file = _fileSystem.GetFreeEF();
            if (file != null)
            {
                file.Setup(this, offset, size, header, headerOffset, headerLength);

                byte fileIndex = 0;
                while (fileIndex < _childrenCount && _children[fileIndex]._inParentBodyOffset < file._inParentBodyOffset)
                {
                    fileIndex++;
                }

                for (byte i = _childrenCount; i > fileIndex; i--)
                {
                    _children[i] = _children[(byte)(i - 1)];
                }
                _children[fileIndex] = file;
                _childrenCount++;
            }

            return file;
        }

        /// <summary>
        /// Deletes a file by its FID.
        /// </summary>
        /// <param name="fid">FID of the file.</param>
        /// <returns><c>false</c> is the file is not found.</returns>
        public bool DeleteFile(short fid)
        {
            File file = FindFileByFileId(fid);
            if (file == null)
            {
                return false;
            }

            file.Release();

            // Update the children
            byte fileIndex = 0;
            while (fileIndex < _childrenCount && _children[fileIndex] != file)
            {
                fileIndex++;
            }

            for (byte i = (byte)(fileIndex + 1); i < _childrenCount; i++)
            {
                _children[(byte)(i - 1)] = _children[i];
            }
            _childrenCount--;
            _children[_childrenCount] = null;

            return true;
        }

        /// <summary>
        /// Find the file in DF having a given file identifier.
        /// </summary>
        /// <param name="fid"></param>
        /// <returns><c>null</c> if not found or the found <see cref="File"/> instance.</returns>
        public File FindFileByFileId(short fid)
        {
            byte index = 0;
            while (index < _childrenCount)
            {
                if (_children[index].GetFileId() == fid)
                {
                    return _children[index];
                }
                index++;
            }

            return null;
        }

        /// <summary>
        /// Returns the nth child file. A call to hasChild should be done to verify the existence.
        /// </summary>
        /// <param name="nth">Index of the file in the DF.</param>
        /// <returns></returns>
        public File GetChild(byte nth)
        {
            return _children[nth];
        }

        /// <summary>
        /// Returns true if the nth child file exists.
        /// </summary>
        /// <param name="nth">Index of the file in the DF.</param>
        /// <returns></returns>
        public bool HasChild(byte nth)
        {
            return nth >= 0 && nth < _childrenCount;
        }

        /// <summary>
        /// Returns the number of children.
        /// </summary>
        /// <returns></returns>
        public byte GetChildCount()
        {
            return _childrenCount;
        }
    }
}