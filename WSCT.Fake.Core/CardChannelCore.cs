﻿using System;
using System.Linq;
using System.Text;
using WSCT.Core;
using WSCT.Core.APDU;
using WSCT.ISO7816;
using WSCT.Wrapper;

namespace WSCT.Fake.Core
{
    public class CardChannelCore : ICardChannel
    {
        #region >> Fields

        private ICardContext _context;
        private bool _connected;

        private IFakeCard _fakeCard;
        private readonly Func<IFakeCard> _fakeCardFactory;

        #endregion

        #region >> Constructors

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public CardChannelCore(IFakeCard fakeCard)
        {
            _connected = false;
            _fakeCardFactory = () => fakeCard;
        }

        /// <summary>
        /// Constructor (<seealso cref="Attach"/>).
        /// </summary>
        /// <param name="context">Resource manager context to attach.</param>
        /// <param name="readerName">Name of the reader to use.</param>
        /// <param name="fakeCard"></param>
        public CardChannelCore(ICardContext context, string readerName, IFakeCard fakeCard)
            : this(fakeCard)
        {
            Attach(context, readerName);
        }

        /// <summary>
        /// Initializes a new instance using a factory to obtain the <see cref="IFakeCard"/> instance on reset.
        /// </summary>
        public CardChannelCore(Func<IFakeCard> fakeCardFactory)
        {
            _fakeCardFactory = fakeCardFactory;
        }

        /// <summary>
        /// Initializes a new instance using a factory to obtain the <see cref="IFakeCard"/> instance on reset (<seealso cref="Attach"/>).
        /// </summary>
        /// <param name="context">Resource manager context to attach.</param>
        /// <param name="readerName">Name of the reader to use.</param>
        /// <param name="fakeCardFactory"></param>
        public CardChannelCore(ICardContext context, string readerName, Func<IFakeCard> fakeCardFactory)
            : this(fakeCardFactory)
        {
            Attach(context, readerName);
        }

        #endregion

        #region >> ICardChannel Members

        /// <inheritdoc />
        public Protocol Protocol { get; private set; }

        /// <inheritdoc />
        public string ReaderName { get; private set; }

        /// <inheritdoc />
        public virtual void Attach(ICardContext context, string readerName)
        {
            _context = context;
            ReaderName = readerName;
            Protocol = Protocol.T0;
        }

        /// <inheritdoc />
        public virtual ErrorCode Connect(ShareMode shareMode, Protocol preferredProtocol)
        {
            Protocol = preferredProtocol;
            _connected = true;

            _fakeCard = _fakeCardFactory();

            return ErrorCode.Success;
        }

        /// <inheritdoc />
        public virtual ErrorCode Disconnect(Disposition disposition)
        {
            if (!_connected)
            {
                return ErrorCode.ErrorInvalidHandle;
            }

            _connected = false;

            return ErrorCode.Success;
        }

        /// <inheritdoc />
        public virtual ErrorCode GetAttrib(Attrib attrib, ref byte[] buffer)
        {
            switch (attrib)
            {
                case Attrib.AtrString:
                    buffer = _fakeCard.GetAtr();
                    break;
                case Attrib.DeviceFriendlyName:
                    buffer = Encoding.Default.GetBytes(ReaderName);
                    break;
                default:
                    buffer = Array.Empty<byte>();
                    break;
            }

            return ErrorCode.Success;
        }

        /// <inheritdoc />
        public virtual State GetStatus()
        {
            // TODO

            return State.Present;
        }

        /// <inheritdoc />
        public virtual ErrorCode Reconnect(ShareMode shareMode, Protocol preferedProtocol, Disposition initialization)
        {
            // TODO

            Protocol = preferedProtocol;

            return ErrorCode.Success;
        }

        /// <inheritdoc />
        public virtual ErrorCode Transmit(ICardCommand command, ICardResponse response)
        {
            var result = _fakeCard.ExecuteCommand(new CommandAPDU(command.BinaryCommand));

            if (result.ErrorCode == ErrorCode.Success)
            {
                var rApdu = result.RApdu;
                response.Parse(rApdu.Udr.Concat(new byte[] { rApdu.Sw1, rApdu.Sw2 }).ToArray());
            }

            return result.ErrorCode;
        }

        #endregion
    }
}