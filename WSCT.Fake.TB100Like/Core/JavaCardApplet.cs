using System.Security.Cryptography;
using WSCT.Fake.Core;
using WSCT.Fake.JavaCard;
using WSCT.ISO7816;

namespace WSCT.Fake.TB100Like.Core
{
    /// <summary>
    /// This <see cref="IFakeCard"/> implementation is an almost 1 to 1 port of the javacard TB100 applet from https://github.com/wsct/ENSICAEN-Card-Applet/commit/f20e5659b6a6b5003de4feec2084bb545fc522c2 .<br/>
    /// It works at ADPU level.
    /// </summary>
    public class JavaCardApplet : IFakeCard
    {
        private static readonly byte[] DefaultAtr = new byte[] { 0x3F, 0x67, 0x25, 0x00, 0x21, 0x20, 0x00, 0x0F, 0x78, 0x90, 0x00 };

        private bool _isActive;
        private readonly DedicatedFile _masterFile;
        private readonly HeaderParser _headerParser;
        private DedicatedFile _currentDF;
        private ElementaryFile _currentEF;

        public JavaCardApplet()
        {
            _masterFile = new DedicatedFile(new FileSystem(Constants.FILESYSTEM_SIZE, Constants.DF_MAX, Constants.EF_MAX));
            _masterFile.Setup(null, 0, Constants.FILESYSTEM_SIZE, Constants.MF_HEADER, 0, (short)Constants.MF_HEADER.Length);

            _headerParser = new HeaderParser();
            _currentDF = _masterFile;
            _currentEF = null;

            // Initialize structure for TB100
            _masterFile.CreateElementaryFile(0x0000, 0x0006, new byte[] { 0x17, 0xFF, 0x06, 0xE4 }, 0x0000, 0x0004);
            _masterFile.CreateElementaryFile(0x0006, 0x0005, new byte[] { 0x1F, 0x6C, 0x05, 0x70 }, 0x0000, 0x0004);
            _masterFile.CreateElementaryFile(0x000B, 0x0005, new byte[] { 0x0E, 0x2F, 0xF5, 0xCE }, 0x0000, 0x0004);
            _masterFile.CreateElementaryFile(0x0010, 0x0003, new byte[] { 0x0E, 0x10, 0x03, 0xDF }, 0x0000, 0x0004);
            _masterFile.CreateDedicatedFile(0x0013, 0x0022, new byte[] { 0x7F, 0x00, 0x00, 0x22, 0xFF, 0xFF, 0xFE, 0x62 }, 0x0000, 0x0008);
            ElementaryFile ef0E2F = (ElementaryFile)_masterFile.FindFileByFileId(0x0E2F);
            ef0E2F.Write(new byte[] { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 }, 0, 0x0002, 2);
            ElementaryFile ef0E10 = (ElementaryFile)_masterFile.FindFileByFileId(0x0E10);
            ef0E10.Write(new byte[] { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 }, 0, 0x0000, 2);
            DedicatedFile df7F00 = (DedicatedFile)_masterFile.FindFileByFileId(0x7F00);
            df7F00.CreateElementaryFile(0x0000, 0x0010, new byte[] { 0x6F, 0x01, 0x00, 0x10, 0xFF, 0xFF, 0xFF, 0x83 }, 0, 0x0008);
        }

        private readonly byte[] atr = DefaultAtr;

        #region >> IFakeCard

        public byte[] GetAtr()
        {
            return atr;
        }

        public bool ColdReset()
        {
            return Unpower() && Connect();
        }

        public bool Connect()
        {
            _isActive = true;

            return true;
        }

        public IFakeCardFeedback ExecuteCommand(CommandAPDU cApdu)
        {
            if (cApdu.Cla != 0x00)
            {
                return FakeCardFeedback.FromSuccess(StatusWord.ClaNotSupported);
            }

            switch (cApdu.Ins)
            {
                case Constants.INS_CREATE_FILE:
                    return ProcessCreateFile(cApdu);
                case Constants.INS_DELETE_FILE:
                    return ProcessDeleteFile(cApdu);
                case Constants.INS_ERASE:
                    return ProcessErase(cApdu);
                case Constants.INS_FSEARCH:
                    return ProcessFSearch(cApdu);
                case Constants.INS_GENERATE_RANDOM:
                    return ProcessGenerateRandom(cApdu);
                case Constants.INS_READ_BINARY:
                    return ProcessRead(cApdu);
                case Constants.INS_SELECT:
                    return ProcessSelect(cApdu);
                case Constants.INS_WRITE_BINARY:
                    return ProcessWrite(cApdu);
                default:
                    return FakeCardFeedback.FromSuccess(StatusWord.InsNotSupported);
            }
        }

        public bool Unpower()
        {
            if (!_isActive)
            {
                return false;
            }

            _isActive = false;

            return true;

        }

        public bool WarmReset()
        {
            return _isActive;
        }

        #endregion

        /// <summary>
        /// Process CREATE FILE instruction (E0)
        /// <para>
        /// C-APDU: <code>00 E0 {offset} {Lc} {header}</code>
        /// </para>
        /// <list type="table">
        ///     <item>
        ///         <term>offset</term>
        ///         <description>offset of first word of the file coded by P1 P2 (WORDS).</description>
        ///     </item>
        ///     <item>
        ///         <term>header</term>
        ///         <description>header of the new file, must be word aligned.</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="apdu"></param>
        /// <returns></returns>
        private IFakeCardFeedback ProcessCreateFile(CommandAPDU apdu)
        {
            byte[] buffer = apdu.GetBuffer();
            apdu.SetIncomingAndReceive();

            short headerOffset = APDUHelpers.GetOffsetCdata(apdu);
            short headerLength = APDUHelpers.GetIncomingLength(apdu);

            if (headerLength < 4)
            {
                ISOException.ThrowIt(JavaCard.ISO7816.SW_DATA_INVALID);
            }

            short offset = Util.GetShort(buffer, JavaCard.ISO7816.OFFSET_P1);

            if (!_headerParser.Parse(buffer, headerOffset, (short)(headerOffset + headerLength)))
            {
                ISOException.ThrowIt(JavaCard.ISO7816.SW_DATA_INVALID);
            }
            short size = (short)(_headerParser.bodyLength + (short)(_headerParser.headerLength >> 2));

            File file = null;

            switch (_headerParser.fileType)
            {
                case HeaderParser.FILETYPE_DF:
                    file = _currentDF.CreateDedicatedFile(offset, size, buffer, headerOffset, headerLength);
                    break;
                case HeaderParser.FILETYPE_EFSZ:
                case HeaderParser.FILETYPE_EFWZ:
                    file = _currentDF.CreateElementaryFile(offset, size, buffer, headerOffset, headerLength);
                    break;
                default:
                    ISOException.ThrowIt(JavaCard.ISO7816.SW_CONDITIONS_NOT_SATISFIED);
                    break;
            }

            if (file == null)
            {
                ISOException.ThrowIt(JavaCard.ISO7816.SW_DATA_INVALID);
            }

            return apdu.SetOutgoingAndSend(0, 0);
        }

        /// <summary>
        /// Process DELETE FILE instruction (E4)
        /// <para>
        /// C-APDU: <code>00 E4 00 00 02 {fid}</code>
        /// </para>
        /// </summary>
        /// <param name="apdu"></param>
        /// <returns></returns>
        private IFakeCardFeedback ProcessDeleteFile(CommandAPDU apdu)
        {
            byte[] buffer = apdu.GetBuffer();
            _ = apdu.SetIncomingAndReceive();

            short udcOffset = APDUHelpers.GetOffsetCdata(apdu);
            short lc = APDUHelpers.GetIncomingLength(apdu);

            if (lc != 2)
            {
                ISOException.ThrowIt(JavaCard.ISO7816.SW_WRONG_P1P2);
            }

            short fid = Util.GetShort(buffer, udcOffset);

            if (_currentDF.DeleteFile(fid) == false)
            {
                ISOException.ThrowIt(JavaCard.ISO7816.SW_FILE_NOT_FOUND);
            }

            if (_currentEF != null && fid == _currentEF.GetFileId())
            {
                _currentEF = null;
            }

            return apdu.SetOutgoingAndSend(0, 0);
        }

        /// <summary>
        /// Process ERASE instruction (CC3)
        /// <para>
        /// C-APDU: <code>00 0E {offset} {Lc} {length} </code>
        /// </para>
        /// <list type="table">
        ///     <item>
        ///         <term>offset</term>
        ///         <description>offset of first word of the file coded by P1 P2 (WORDS) to be erased.</description>
        ///     </item>
        ///     <item>
        ///         <term>length</term>
        ///         <description>number of words to be erased.</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="apdu"></param>
        /// <returns></returns>
        private IFakeCardFeedback ProcessErase(CommandAPDU apdu)
        {
            // TODO: check ==> security of current EF

            byte[] apduBuffer = apdu.GetBuffer();
            apdu.SetIncomingAndReceive();

            // Check if Lc ==2
            short lc = APDUHelpers.GetIncomingLength(apdu);
            if (lc != 2)
            {
                ISOException.ThrowIt(JavaCard.ISO7816.SW_WRONG_LENGTH);
            }

            short offset = Util.GetShort(apduBuffer, JavaCard.ISO7816.OFFSET_P1); // in WORDS
            short udcOffset = APDUHelpers.GetOffsetCdata(apdu);
            short length = Util.GetShort(apduBuffer, udcOffset); // in WORDS

            VerifyOutOfFile(offset, length);

            _currentEF.Erase(offset, length);

            return FakeCardFeedback.FromSuccess(unchecked((short)0x9000));
        }

        /// <summary>
        /// Process FSEARCH instruction (CC2).
        /// <para>
        /// C-APDU: <code>00 B0 {P1-P2: offset} 04</code>
        /// </para>
        /// <para>
        /// R-APDU when an EF is selected: <code>{offset} {number of word in current EF} {looked-up word}</code>
        /// </para>
        /// <para>
        /// R-APDU when no EF is selected: <code>{offset} {number of word in current DF} {looked-up word}</code>
        /// </para>
        /// </summary>
        /// <param name="apdu"></param>
        /// <returns></returns>
        private IFakeCardFeedback ProcessFSearch(CommandAPDU apdu)
        {
            // get offset
            byte[] buffer = apdu.GetBuffer();
            short offset = Util.GetShort(buffer, JavaCard.ISO7816.OFFSET_P1); // in WORDS

            // get le
            short le = apdu.SetOutgoing(); // in BYTES
            if (le != 0)
            {
                ISOException.ThrowIt(JavaCard.ISO7816.SW_WRONG_LENGTH);
            }

            short wordCount;
            short offsetFound = offset;

            // check that there is a current EF
            if (_currentEF != null)
            {
                wordCount = (short)(_currentEF.GetLength() - _currentEF.GetHeaderSize());
                while (!(_currentEF.IsAvailable(offsetFound, 1) || offsetFound == wordCount))
                {
                    offsetFound++;
                }
            }
            else
            {
                wordCount = (short)(_currentDF.GetLength() - _currentDF.GetHeaderSize());
                while (!(_currentDF.IsAvailable(offsetFound, 1) || offsetFound == wordCount))
                {
                    offsetFound++;
                }
            }

            // check that there is still some empty space
            if (offsetFound == wordCount)
            {
                ISOException.ThrowIt(Constants.SW_DATA_NOT_FOUND);
            }
            // copy answer in buffer
            Util.SetShort(buffer, 0, offsetFound);
            Util.SetShort(buffer, 2, wordCount);
            Util.ArrayFillNonAtomic(buffer, 4, 4, (byte)MemoryState.Free);

            // and send it!
            return apdu.SetOutgoingAndSend(0, 8);
        }

        /// <summary>
        /// Process GENERATE RANDOM instruction (CC2)
        /// <para>
        /// C-APDU: <code>00 C4 00 00 08</code>
        /// </para>
        /// </summary>
        /// <param name="apdu"></param>
        /// <returns></returns>
        private IFakeCardFeedback ProcessGenerateRandom(CommandAPDU apdu)
        {
            byte[] apduBuffer = apdu.GetBuffer();
            short le = apdu.SetOutgoing();

            // verify that Le='08'
            if (le != 8)
            {
                // if not => error
                ISOException.ThrowIt(JavaCard.ISO7816.SW_WRONG_LENGTH);
            }

            // generate 8 random bytes
            var rndGen = new RNGCryptoServiceProvider();
            rndGen.GetBytes(apduBuffer, 0, 8);
            // and send it!
            return apdu.SetOutgoingAndSend(0, 8);
        }

        /// <summary>
        /// Process READ BINARY instruction (CC2)
        /// <para>
        /// C-APDU: <code>00 B0 {P1-P2: offset} {Le}</code>
        /// </para>
        /// <para>
        /// R-APDU when an EF is selected: <code>{data in EF}</code>
        /// </para>
        /// <para>
        /// R-APDU when no EF is selected: <code>{headers of EF in current DF}</code>
        /// </para>
        /// </summary>
        /// <param name="apdu"></param>
        /// <returns></returns>
        private IFakeCardFeedback ProcessRead(CommandAPDU apdu)
        {
            short le = apdu.SetOutgoing(); // in BYTES
            short wordCount = (short)((short)(le + 3) / 4);
            byte[] buffer = JCSystem.MakeTransientByteArray((short)(wordCount * 4), JCSystem.CLEAR_ON_DESELECT);
            short offset;

            if (_currentEF != null)
            {
                // an EF is selected ==> read binary
                offset = Util.GetShort(apdu.GetBuffer(), JavaCard.ISO7816.OFFSET_P1); // in WORDS

                byte[] header = JCSystem.MakeTransientByteArray(8, JCSystem.CLEAR_ON_DESELECT);
                _currentEF.GetHeader(header, 0);
                _headerParser.Parse(header, 0, 8);
                offset += (short)(_currentEF.Read(offset, buffer, 0, wordCount, _headerParser.fileType == HeaderParser.FILETYPE_EFSZ) << 2);

            }
            else
            {
                // no EF selected ==> DIR
                offset = 0;
                byte currentChildNumber = 0;
                File fileChild = _currentDF.GetChild(currentChildNumber);
                // get header of each file in current DF
                while (fileChild != null && offset < le)
                {
                    fileChild.GetHeader(buffer, offset);
                    offset += (short)(fileChild.GetHeaderSize() << 2);
                    fileChild = _currentDF.GetChild(++currentChildNumber);
                }
            }

            // Pad with FF
            Util.ArrayFillNonAtomic(buffer, offset, (short)(le - offset), (byte)MemoryState.Free);

            // and send data!
            apdu.SetOutgoingLength(le);
            return apdu.SendBytesLong(buffer, 0, le);
        }

        /// <summary>
        /// Process SELECT instruction (CC4).
        /// <para>
        /// C-APDU: <code>00 A4 00 00 02 {FID} {Le}</code>
        /// </para>
        /// <para>
        /// R-APDU: <code>{offset} {size} {header}</code>
        /// </para>
        /// <list type="table">
        ///     <item>
        ///         <term>offset</term>
        ///         <description>offset of first word of the file, 2 bytes (WORDS).</description>
        ///     </item>
        ///     <item>
        ///         <term>size</term>
        ///         <description>size of the body of the file (WORDS).</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="apdu"></param>
        /// <returns></returns>
        private IFakeCardFeedback ProcessSelect(CommandAPDU apdu)
        {
            byte[] buffer = apdu.GetBuffer();
            apdu.SetIncomingAndReceive();

            short udcOffset = APDUHelpers.GetOffsetCdata(apdu);
            short lc = (short)apdu.Lc;

            if (lc != 2)
            {
                ISOException.ThrowIt(JavaCard.ISO7816.SW_WRONG_LENGTH);
            }

            short fid = Util.GetShort(buffer, udcOffset);

            File file;
            if (fid == 0x3F00)
            {
                file = _masterFile;
            }
            else
            {
                file = _currentDF.FindFileByFileId(fid);
            }

            if (file == null)
            {
                ISOException.ThrowIt(JavaCard.ISO7816.SW_FILE_NOT_FOUND);
            }

            // Update current DF / EF
            if (file.IsDF())
            {
                _currentDF = (DedicatedFile)file;
                _currentEF = null;
            }
            else
            {
                _currentEF = (ElementaryFile)file;
            }

            // Build and send R-APDU
            short headerSize = file.GetHeaderSize();
            Util.SetShort(buffer, 0, (short)(file._inParentBodyOffset >> 2));
            Util.SetShort(buffer, 2, (short)(file.GetLength() - headerSize));
            file.GetHeader(buffer, 4);

            // TODO Automagically adds 9000

            return apdu.SetOutgoingAndSend(0, (short)(4 + (headerSize << 2)));
        }

        /// <summary>
        /// Process WRITE BINARY instruction (CC3)
        /// <para>
        /// C-APDU: <code>00 B0 {offset} {Lc} {data} </code>
        /// </para>
        /// <list type="table">
        ///     <item>
        ///         <term>offset</term>
        ///         <description>offset of first word of the file coded by P1 P2 (WORDS) to be written.</description>
        ///     </item>
        ///     <item>
        ///         <term>data</term>
        ///         <description>data to be written in file.</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="apdu"></param>
        /// <returns></returns>
        private IFakeCardFeedback ProcessWrite(CommandAPDU apdu)
        {
            // TODO: check ==> security of current EF

            byte[] apduBuffer = apdu.GetBuffer();
            apdu.SetIncomingAndReceive();

            short offset = Util.GetShort(apduBuffer, JavaCard.ISO7816.OFFSET_P1); // in WORDS
            short length = APDUHelpers.GetIncomingLength(apdu); // in BYTES
            short wordCount = (short)((short)(length + 3) / 4); // length in WORDS

            // availability check
            VerifyOutOfFile(offset, wordCount);
            if (!_currentEF.IsAvailable(offset, wordCount))
            {
                ISOException.ThrowIt(JavaCard.ISO7816.SW_WRONG_LENGTH);
            }

            // copy data in a buffer
            byte[] buffer = JCSystem.MakeTransientByteArray((short)(wordCount * 4), JCSystem.CLEAR_ON_DESELECT);
            short udcOffset = APDUHelpers.GetOffsetCdata(apdu);
            Util.ArrayCopyNonAtomic(apduBuffer, udcOffset, buffer, 0, length);

            // complete words with FF in buffer
            short iMax = (short)(wordCount * 4);
            for (short i = length; i < iMax; i++)
            {
                buffer[i] = 0xFF;
            }

            // and write data to file
            _currentEF.Write(buffer, 0, offset, wordCount);

            return FakeCardFeedback.FromSuccess(unchecked((short)0x9000));
        }

        private void VerifyOutOfFile(short offset, short length)
        {
            // Check if there is a current EF
            if (_currentEF == null)
            {
                ISOException.ThrowIt(JavaCard.ISO7816.SW_CONDITIONS_NOT_SATISFIED);
            }

            short bodyLength = (short)(_currentEF.GetLength() - _currentEF.GetHeaderSize()); // in WORDS

            // check if offset < bodyLength
            if (offset >= bodyLength)
            {
                ISOException.ThrowIt(JavaCard.ISO7816.SW_WRONG_P1P2);
            }

            // check if offset+length <= bodyLength
            if ((short)(offset + length) > bodyLength)
            {
                ISOException.ThrowIt(JavaCard.ISO7816.SW_WRONG_LENGTH);
            }

        }

    }
}
