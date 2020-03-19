using WSCT.ISO7816;
using WSCT.Wrapper;

namespace WSCT.Fake.Core
{
    public class FakeCardFeedback : IFakeCardFeedback
    {
        /// <summary>
        /// Creates a new instance from given <paramref name="errorCode"/> and <paramref name="response"/>.
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="response"></param>
        public FakeCardFeedback(ErrorCode errorCode, ResponseAPDU response)
        {
            ErrorCode = errorCode;
            RApdu = response;
        }

        /// <summary>
        /// Creates a new instance with the given <paramref name="errorCode"/> and an empty response.
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static IFakeCardFeedback FromError(ErrorCode errorCode)
        {
            return new FakeCardFeedback(errorCode, new ResponseAPDU());
        }

        /// <summary>
        /// Creates a new instance with a <see cref="ErrorCode.Success"/> status and the given bytes sequence as the response (UDR | SW).
        /// </summary>
        /// <param name="responseBytes"></param>
        /// <returns></returns>
        public static IFakeCardFeedback FromSuccess(byte[] responseBytes)
        {
            return new FakeCardFeedback(ErrorCode.Success, new ResponseAPDU(responseBytes));
        }

        /// <summary>
        /// Creates a new instance with a <see cref="ErrorCode.Success"/> status and the given status word (no UDR).
        /// </summary>
        /// <param name="statusWord"></param>
        /// <returns></returns>
        public static IFakeCardFeedback FromSuccess(short statusWord)
        {
            return FromSuccess(new byte[] { (byte)((statusWord & 0xFF00) >> 8), (byte)(statusWord & 0x00FF) });
        }

        #region >> IFakeCardResponse

        public ErrorCode ErrorCode { get; }

        public ResponseAPDU RApdu { get; }

        #endregion
    }
}
