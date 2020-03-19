using System;
using WSCT.Fake.Core;
using WSCT.Fake.JavaCard;
using WSCT.ISO7816;
using WSCT.Wrapper;

namespace WSCT.Fake.TB100Like.Core
{
    /// <summary>
    /// Very light implementation of a javacard runtime environment allowing to mimic the behaviour of a T=0 only TB100 smart card.
    /// </summary>
    public class JavaCardRuntime : IFakeCard
    {
        private readonly IFakeCard _fakeCard;
        private byte[] _udrWaitingToBeRetrieved;
        private byte _lastInsProcessed = 0x00;

        /// <summary>
        /// Creates a new instance wrapping an existing <see cref="IFakeCard"/> implementation.<br/>
        /// </summary>
        /// <param name="fakeCard">Existing <see cref="IFakeCard"/> implementation, notably an APDU level implementation.</param>
        public JavaCardRuntime(IFakeCard fakeCard)
        {
            _fakeCard = fakeCard;
        }

        #region >> IFakeCard

        /// <inheritdoc />
        public byte[] GetAtr()
        {
            return _fakeCard.GetAtr().Clone() as byte[];
        }

        /// <inheritdoc />
        public bool ColdReset()
        {
            return _fakeCard.ColdReset();
        }

        /// <inheritdoc />
        public bool Connect()
        {
            return _fakeCard.Connect();
        }

        /// <inheritdoc />
        public IFakeCardFeedback ExecuteCommand(CommandAPDU cApdu)
        {
            if (cApdu.BinaryCommand.Length < 5)
            {
                return FakeCardFeedback.FromError(ErrorCode.CommDataLost);
            }

            if (cApdu.IsCc2 && cApdu.Cla == 0x00 && cApdu.Ins == 0xC0)
            {
                return ProcessGetResponse(cApdu);
            }

            ClearUdrToBeRetrieved();

            IFakeCardFeedback response;
            try
            {
                response = _fakeCard.ExecuteCommand(cApdu);
            }
            catch (ISOException isoException)
            {
                return FakeCardFeedback.FromSuccess(isoException.StatusWord);
            }

            if (response.RApdu.Udr.Length == 0 || cApdu.IsCc2)
            {
                return response;
            }

            StoreUdrToBeRetrieved(response.RApdu.Udr, cApdu.Ins);

            return FakeCardFeedback.FromSuccess((short)response.RApdu.StatusWord);
        }

        /// <inheritdoc />
        public bool Unpower()
        {
            return _fakeCard.Unpower();
        }

        /// <inheritdoc />
        public bool WarmReset()
        {
            return _fakeCard.WarmReset();
        }

        #endregion

        private void ClearUdrToBeRetrieved()
        {
            _udrWaitingToBeRetrieved = null;
            _lastInsProcessed = 0x00;
        }

        private byte[] GetRApduBytesWithRetrievedUdr(short le)
        {
            var responseBytes = new byte[le + 2];

            var lengthToCopy = Math.Min(le, _udrWaitingToBeRetrieved.Length);
            var lengthToFillWithFF = Math.Max(le, _udrWaitingToBeRetrieved.Length) - lengthToCopy;

            Util.ArrayCopyNonAtomic(_udrWaitingToBeRetrieved, 0, responseBytes, 0, (short)lengthToCopy);

            Util.ArrayFillNonAtomic(responseBytes, (short)lengthToCopy, (short)lengthToFillWithFF, 0xFF);

            Util.SetShort(responseBytes, le, unchecked((short)0x9000));

            return responseBytes;
        }

        /// <summary>
        /// Process a GET RESPONSE command.
        /// </summary>
        /// <param name="cApdu"></param>
        /// <returns></returns>
        private IFakeCardFeedback ProcessGetResponse(CommandAPDU cApdu)
        {
            if (cApdu.P1 != 0x00 || cApdu.P2 != 0x00)
            {
                return FakeCardFeedback.FromSuccess(JavaCard.ISO7816.SW_WRONG_P1P2);
            }

            if (_udrWaitingToBeRetrieved == null)
            {
                return FakeCardFeedback.FromSuccess(unchecked((short)0x9010));
            }

            switch (_lastInsProcessed)
            {
                case JavaCard.ISO7816.INS_SELECT:
                    return ProcessGetResponseAfterSelect(cApdu);
                case Constants.INS_FSEARCH:
                    return ProcessGetResponseAfterFSearch(cApdu);
                default:
                    return FakeCardFeedback.FromSuccess(unchecked((short)0x9010));
            }
        }

        /// <summary>
        /// Process a GET RESPONSE command after a successful SELECT.
        /// </summary>
        /// <param name="cApdu"></param>
        /// <returns></returns>
        private IFakeCardFeedback ProcessGetResponseAfterSelect(CommandAPDU cApdu)
        {
            if (cApdu.Le != 0x08 && cApdu.Le != 0x0C)
            {
                return FakeCardFeedback.FromSuccess(JavaCard.ISO7816.SW_WRONG_LENGTH);
            }

            var rApduBytes = GetRApduBytesWithRetrievedUdr((short)cApdu.Le);

            ClearUdrToBeRetrieved();

            return FakeCardFeedback.FromSuccess(rApduBytes);
        }

        /// <summary>
        /// process a GET RESPONSE command after a successful FSEARCH.
        /// </summary>
        /// <param name="cApdu"></param>
        /// <returns></returns>
        private IFakeCardFeedback ProcessGetResponseAfterFSearch(CommandAPDU cApdu)
        {
            if (cApdu.Le != 0x08)
            {
                return FakeCardFeedback.FromSuccess(JavaCard.ISO7816.SW_WRONG_LENGTH);
            }

            var rApduBytes = GetRApduBytesWithRetrievedUdr((short)cApdu.Le);

            ClearUdrToBeRetrieved();

            return FakeCardFeedback.FromSuccess(rApduBytes);
        }

        private void StoreUdrToBeRetrieved(byte[] udr, byte ins)
        {
            _udrWaitingToBeRetrieved = new byte[udr.Length];
            Array.Copy(udr, _udrWaitingToBeRetrieved, udr.Length);

            _lastInsProcessed = ins;
        }
    }
}
