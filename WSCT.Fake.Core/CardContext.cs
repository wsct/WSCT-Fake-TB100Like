﻿using WSCT.Core;

namespace WSCT.Fake.Core
{
    /// <summary>
    /// Represents an enhanced <see cref="CardContextCore"/> allowing to observe activity by using delegates.
    /// </summary>
    public class CardContext : CardContextObservable
    {
        #region >> Constructors

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public CardContext()
            : base(new CardContextCore())
        {
        }

        #endregion
    }
}