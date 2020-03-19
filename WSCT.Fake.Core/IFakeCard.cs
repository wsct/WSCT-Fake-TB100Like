using WSCT.ISO7816;

namespace WSCT.Fake.Core
{
    public interface IFakeCard
    {

        /// <summary>
        /// Realize a cold reset of the card.
        /// </summary>
        /// <returns></returns>
        bool ColdReset();

        /// <summary>
        /// Connect to the card.
        /// </summary>
        /// <returns></returns>
        bool Connect();

        /// <summary>
        /// Set a input command.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        IFakeCardFeedback ExecuteCommand(CommandAPDU command);

        /// <summary>
        /// Returns  the ATR of the card.
        /// </summary>
        byte[] GetAtr();

        /// <summary>
        /// Unpower the card.
        /// </summary>
        /// <returns></returns>
        bool Unpower();

        /// <summary>
        /// Realize a warm reset of the card.
        /// </summary>
        /// <returns></returns>
        bool WarmReset();
    }
}