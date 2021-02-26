using System;
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

        /// <inheritdoc cref="CardChannelCore(ICardContext,string,IFakeCard)" />
        public CardChannel(ICardContext context, string readerName, IFakeCard fakeCard)
            : base(new CardChannelCore(context, readerName, fakeCard))
        {
        }

        /// <inheritdoc cref="CardChannelCore(Func&lt;IFakeCard&gt;)" />
        public CardChannel(Func<IFakeCard> fakeCardFactory)
            : base(new CardChannelCore(fakeCardFactory))
        {
        }

        /// <inheritdoc cref="CardChannelCore(ICardContext,string,Func&lt;IFakeCard&gt;)" />
        public CardChannel(ICardContext context, string readerName, Func<IFakeCard> fakeCardFactory)
            : base(new CardChannelCore(context, readerName, fakeCardFactory))
        {
        }

        #endregion
    }
}
