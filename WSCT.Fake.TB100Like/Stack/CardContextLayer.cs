using WSCT.Stack;
using WSCT.Fake.Core;

namespace WSCT.Fake.TB100Like.Stack
{
    /// <summary>
    /// Implements <see cref="CardContext"/> as a <see cref="CardContextLayer"/>.
    /// </summary>
    /// <remarks>
    /// This layer is the terminal (top) layer by design.
    /// </remarks>
    public class CardContextLayer : CardContext, ICardContextLayer
    {
        #region >> ICardContextLayer Membres

        /// <inheritdoc />
        public void SetStack(ICardContextStack stack)
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