using WSCT.ISO7816;
using WSCT.Wrapper;

namespace WSCT.Fake.Core
{
    public interface IFakeCardFeedback
    {
        /// <summary>
        /// Status of the CRP transmission.
        /// </summary>
        ErrorCode ErrorCode { get; }

        /// <summary>
        /// Response sent by the card.
        /// </summary>
        ResponseAPDU RApdu { get; }
    }
}