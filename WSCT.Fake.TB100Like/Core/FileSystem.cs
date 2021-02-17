using WSCT.Fake.JavaCard;

namespace WSCT.Fake.TB100Like.Core
{
    internal class FileSystem
    {
        private readonly byte _dfMax;
        private readonly byte _efMax;

        readonly ElementaryFile[] elementaryFiles;
        readonly DedicatedFile[] dedicatedFiles;

        /// <summary>
        /// Raw data storage.
        /// </summary>
        public readonly byte[] memory;
        /// <summary>
        /// Storage for attributes of data words in <see cref="memory"/>.
        /// 1 attribute byte per data word.
        /// </summary>
        public readonly byte[] attributes;

        public FileSystem(short size, byte dfMax, byte efMax)
        {
            _dfMax = dfMax;
            _efMax = efMax;

            // Allocation of data memory
            memory = new byte[(short)(size << 2)];
            attributes = new byte[size];
            Erase(0, (short)(size << 2));

            // Allocation of DF files structures
            dedicatedFiles = new DedicatedFile[_dfMax];
            byte i;
            for (i = 0; i < _dfMax; i++)
            {
                dedicatedFiles[i] = new DedicatedFile(this);
            }

            // Allocation of EF files structures
            elementaryFiles = new ElementaryFile[_efMax];
            for (i = 0; i < _efMax; i++)
            {
                elementaryFiles[i] = new ElementaryFile(this);
            }
        }

        /// <summary>
        /// Erases data from memory.
        /// </summary>
        /// <param name="offset">Offset from where to start erasing (<em>should be a multiple of 4</em>) (BYTES).</param>
        /// <param name="length">Number of bytes to erase (<em>should be a multiple of 4</em>) (BYTES).</param>
        /// <returns>offset + length</returns>
        public short Erase(short offset, short length)
        {
            // Erase data
            Util.arrayFillNonAtomic(memory, offset, length, (byte)MemoryState.Free);

            // Update "written" attributes of erased words
            Util.arrayFillNonAtomic(attributes, (short)(offset / 4), (short)(length / 4), (byte)AttributeState.Free);

            return (short)(offset + length);
        }

        /// <summary>
        /// Returns a "not yet used" DF. 
        /// <p>
        /// Warning: 2 successive calls to this method will return the same instance.
        /// </summary>
        /// <returns></returns>
        public DedicatedFile GetFreeDF()
        {
            byte index = 0;
            while (index < _dfMax && dedicatedFiles[index].GetLength() != 0)
            {
                index++;
            }
            return index == _dfMax ? null : dedicatedFiles[index];
        }

        /// <summary>
        /// Returns a "not yet used" EF.
        /// <p>
        /// Warning: 2 successive calls to this method will return the same instance.
        /// </summary>
        /// <returns></returns>
        public ElementaryFile GetFreeEF()
        {
            byte index = 0;
            while (index < _efMax && elementaryFiles[index].GetLength() != 0)
            {
                index++;
            }
            return index == _efMax ? null : elementaryFiles[index];
        }

        /// <summary>
        /// Returns the number of consecutive free words (WORDS).
        /// </summary>
        /// <param name="from">Offset in the body from where to look for free words (WORDS).</param>
        /// <param name="to">Maximum offset to look for (WORDS).</param>
        /// <returns></returns>
        public short GetFreeLength(short from, short to)
        {
            short i = from;

            while (i <= to && attributes[i] != (byte)AttributeState.Written)
            {
                i++;
            }

            if (i == (short)(to + 1))
            {
                i--;
            }

            return (short)(i - from);
        }

        /// <summary>
        /// Returns the number of consecutive written words.
        /// </summary>
        /// <param name="from">Offset in the body from where to look for written words (WORDS).</param>
        /// <param name="to">Maximum offset to look for (WORDS).</param>
        /// <returns></returns>
        public short GetWrittenLength(short from, short to)
        {
            short i = from;

            while (i <= to && attributes[i] == (byte)AttributeState.Written)
            {
                i++;
            }

            return (short)(i - from);
        }

        /// <summary>
        /// Reads a section of data in memory.
        /// </summary>
        /// <param name="offset">Offset in the body where to start the reading (should be a multiple of 4) (BYTES).</param>
        /// <param name="output">Output buffer.</param>
        /// <param name="outputOffset">Offset in the output buffer where to write the data (BYTES).</param>
        /// <param name="length">Length of the data to read (BYTES).</param>
        /// <param name="secureRead"></param>
        /// <returns>New in memory offset (outputOffset + read).</returns>
        public short Read(short offset, byte[] output, short outputOffset, short length, bool secureRead)
        {
            short iMax = (short)(offset + length);
            short i = offset;
            short writtenLength;
            short freeLength;
            while (i < iMax)
            {
                writtenLength = (short)(GetWrittenLength((short)(i >> 2), (short)((short)(iMax >> 2) - 1)) << 2);
                if (secureRead)
                {
                    Util.arrayFillNonAtomic(output, outputOffset, writtenLength, (byte)MemoryState.Written);
                }
                else
                {
                    Util.arrayCopyNonAtomic(memory, i, output, outputOffset, writtenLength);
                }
                i += writtenLength;
                outputOffset += writtenLength;
                freeLength = (short)(GetFreeLength((short)(i >> 2), (short)(iMax >> 2)) << 2);
                Util.arrayFillNonAtomic(output, outputOffset, freeLength, (byte)MemoryState.Free);
                i += freeLength;
                outputOffset += freeLength;
            }

            return outputOffset;
        }

        /// <summary>
        /// Search for up to 4 consecutive bytes in memory.
        /// </summary>
        /// <param name="offset">Offset of the first word to test in memory (WORDS).</param>
        /// <param name="length">Maximum number of consecutive words to test (WORDS).</param>
        /// <param name="value">Value to search in memory range.</param>
        /// <param name="valueOffset">Offset of the first byte to test in value (BYTES).</param>
        /// <param name="valueLength">Length of the searched value (BYTES).</param>
        /// <returns>Offset of the first occurrence of value in memory (WORDS).</returns>
        public short Search(short offset, short length, byte[] value, short valueOffset, short valueLength)
        {
            short lengthInBytes = (short)(length << 2);
            if (valueLength > lengthInBytes || (short)(offset + length) > memory.Length || (short)(valueOffset + valueLength) > value.Length)
            {
                throw new ArrayIndexOutOfBoundsException();
            }

            short valueLengthInWords = (short)((short)(valueLength + 3) / 4);

            short index = (short)(offset << 2);
            byte compareResult = 0;
            do
            {
                short x = GetWrittenLength((short)(index / 4), (short)((short)(index + valueLength - 1) / 4));
                if (x == valueLengthInWords)
                {
                    compareResult = Util.arrayCompare(value, valueOffset, memory, index, valueLength);
                }
                index += 4;
            } while (index < lengthInBytes && compareResult != 0);

            if (compareResult == 0)
            {
                return (short)((short)(index - 4) >> 2);
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Writes data in memory.
        /// </summary>
        /// <param name="source">Buffer containing the data to write.</param>
        /// <param name="sourceOffset">Offset of the data in previous buffer (BYTES).</param>
        /// <param name="offset"> Offset in memory where to write the data (should be a multiple of 4) (BYTES).</param>
        /// <param name="length">Length of the data to write (BYTES).</param>
        /// <returns>new in memory offset (offset + length)</returns>
        public short Write(byte[] source, short sourceOffset, short offset, short length)
        {
            // Store data
            Util.arrayCopyNonAtomic(source, sourceOffset, memory, offset, length);

            // Update "written" attributes of used words
            Util.arrayFillNonAtomic(attributes, (short)(offset / 4), (short)(length / 4), (byte)AttributeState.Written);

            return (short)(offset + length);
        }
    }
}
