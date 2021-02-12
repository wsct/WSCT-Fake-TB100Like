using System;
using WSCT.Core.APDU;
using WSCT.Fake.Core;
using WSCT.ISO7816;

namespace WSCT.Fake.JavaCard
{
    /// <summary>
    /// <see cref="ICardCommand"/> wrapper used as a fake <c>avacard.framework.APDU</c>.
    /// </summary>
    public class APDU
    {
        readonly ICardCommand _command;

        private readonly byte[] _buffer = new byte[300];
        private short _offset = 0;
        private short _length = 0;

        public APDU(ICardCommand command)
        {
            _command = command;
        }

        internal short ResponseLength => _length;

        internal byte[] ResponseBuffer => _buffer;

        /// <summary>
        /// Returns the APDU buffer byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBuffer()
        {
            Array.Copy(_command.BinaryCommand, 0, _buffer, 0, _command.BinaryCommand.Length);
            Array.Clear(_buffer, _command.BinaryCommand.Length, _buffer.Length - _command.BinaryCommand.Length);

            return _buffer;
        }

        /// <summary>
        /// This is the primary receive method.
        /// </summary>
        /// <returns></returns>
        public short SetIncomingAndReceive()
        {
            var cApdu = (CommandAPDU)_command;

            return (short)cApdu.Udc.Length;
        }

        /// <summary>
        /// This method is used to set the data transfer direction to outbound and to obtain the expected length of response (Ne).
        /// </summary>
        /// <returns></returns>
        public short SetOutgoing()
        {
            var cApdu = (CommandAPDU)_command;

            return (short)cApdu.Le;
        }

        /// <summary>
        /// Sends len more bytes from outData byte array starting at specified offset bOff.
        /// </summary>
        /// <param name="buffer">Source data byte array.</param>
        /// <param name="offset">Offset into OutData array.</param>
        /// <param name="length">Byte length of the data to send.</param>
        /// <returns></returns>
        public IFakeCardFeedback SendBytesLong(byte[] buffer, short offset, short length)
        {
            _offset = offset;
            _length = length;

            Array.Copy(buffer, offset, _buffer, 0, _length);

            Util.SetShort(_buffer, (short)(_offset + _length), unchecked((short)0x9000));
            _length += 2;

            var responseBytes = new byte[_length];
            Array.Copy(_buffer, _offset, responseBytes, 0, _length);

            return FakeCardFeedback.FromSuccess(responseBytes);
        }

        /// <summary>
        /// This is the "convenience" send method.
        /// </summary>
        /// <param name="offset">Offset into APDU buffer.</param>
        /// <param name="length">Bytelength of the data to send.</param>
        public IFakeCardFeedback SetOutgoingAndSend(short offset, short length)
        {
            _offset = offset;
            _length = length;

            Util.SetShort(_buffer, (short)(_offset + _length), unchecked((short)0x9000));
            _length += 2;

            var responseBytes = new byte[_length];
            Array.Copy(_buffer, _offset, responseBytes, 0, _length);

            return FakeCardFeedback.FromSuccess(responseBytes);
        }

        /// <summary>
        /// Sets the actual length of response data. If a length of 0 is specified, no data will be output.
        /// </summary>
        /// <param name="le">Length of response data.</param>
        /// <returns></returns>
        public void SetOutgoingLength(short le)
        {
            _length = le;
        }
    }

}
