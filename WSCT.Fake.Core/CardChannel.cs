using WSCT.Core;

namespace WSCT.Fake.Core
{
    public class CardChannel : CardChannelObservable
    {
        #region >> Constructors

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public CardChannel(IFakeCard fakeCard)
            : base(new CardChannelCore(fakeCard))
        {
        }

        /// <inheritdoc cref="CardChannelCore(ICardContext,string)" />
        public CardChannel(ICardContext context, string readerName, IFakeCard fakeCard)
            : base(new CardChannelCore(context, readerName, fakeCard))
        {
        }

        #endregion
    }
}
