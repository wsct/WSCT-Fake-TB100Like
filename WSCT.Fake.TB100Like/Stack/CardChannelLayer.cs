using WSCT.Core;
using WSCT.Stack;
using WSCT.Fake.TB100Like.Core;
using WSCT.Fake.Core;

namespace WSCT.Fake.TB100Like.Stack
{
    /// <summary>
    /// Implements <see cref="CardChannel"/> as a <see cref="CardChannelLayer"/>.
    /// </summary>
    /// <remarks>
    /// This layer is the terminal (top) layer by design.
    /// </remarks>
    public class CardChannelLayer : CardChannel, ICardChannelLayer
    {
        #region >> Constructors

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public CardChannelLayer()
            : base(new JavaCardRuntime(new JavaCardApplet()))
        {
        }

        /// <inheritdoc cref="CardChannel(ICardContext,string)"/>
        public CardChannelLayer(ICardContext context, string readerName)
            : base(context, readerName, new JavaCardRuntime(new JavaCardApplet()))
        {
        }

        #endregion

        #region >> ICardChannelLayer Membres

        /// <inheritdoc />
        public void SetStack(ICardChannelStack stack)
        {
            // Nothing to do here.
        }

        /// <inheritdoc />
        public string LayerId
        {
            get { return "Fake TB100"; }
        }

        #endregion
    }
}