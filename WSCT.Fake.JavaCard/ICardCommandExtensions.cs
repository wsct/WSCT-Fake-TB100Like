using System;
using WSCT.Core.APDU;
using WSCT.Fake.Core;
using WSCT.ISO7816;

namespace WSCT.Fake.JavaCard
{
    /// <summary>
    /// Fake implementation of JavaCard APDU methods as C# extension methods.
    /// </summary>
    public static class ICardCommandExtensions
    {
        private static byte[] _buffer = new byte[300];
        private static short _offset = 0;
        private static short _length = 0;

        /// <summary>
        /// Returns the APDU buffer byte array.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static byte[] GetBuffer(this ICardCommand command)
        {
            _buffer = new byte[300];

            Array.Copy(command.BinaryCommand, 0, _buffer, 0, command.BinaryCommand.Length);

            return _buffer;
        }

        /// <summary>
        /// This is the primary receive method.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static short SetIncomingAndReceive(this ICardCommand command)
        {
            return 0;
        }

        /// <summary>
        /// This method is used to set the data transfer direction to outbound and to obtain the expected length of response (Ne).
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static short SetOutgoing(this ICardCommand command)
        {
            var cApdu = (CommandAPDU)command;

            return (short)cApdu.Le;
        }

        /// <summary>
        /// Sends len more bytes from outData byte array starting at specified offset bOff.
        /// </summary>
        /// <param name="_"></param>
        /// <param name="buffer">Source data byte array.</param>
        /// <param name="offset">Offset into OutData array.</param>
        /// <param name="length">Byte length of the data to send.</param>
        /// <returns></returns>
        public static IFakeCardFeedback SendBytesLong(this ICardCommand _, byte[] buffer, short offset, short length)
        {
            _offset = offset;
            _length = length;

            Array.Copy(buffer, offset, _buffer, 0, _length);

            Util.SetShort(_buffer, bOff: (short)(_offset + _length), unchecked((short)0x9000));
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
        public static IFakeCardFeedback SetOutgoingAndSend(this ICardCommand _, short offset, short length)
        {
            _offset = offset;
            _length = length;

            Util.SetShort(_buffer, bOff: (short)(_offset + _length), unchecked((short)0x9000));
            _length += 2;

            var responseBytes = new byte[_length];
            Array.Copy(_buffer, _offset, responseBytes, 0, _length);

            return FakeCardFeedback.FromSuccess(responseBytes);
        }

        /// <summary>
        /// Sets the actual length of response data. If a length of 0 is specified, no data will be output.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="le">Length of response data.</param>
        /// <returns></returns>
        public static void SetOutgoingLength(this ICardCommand command, short le)
        {
            // TODO
        }
    }
}
