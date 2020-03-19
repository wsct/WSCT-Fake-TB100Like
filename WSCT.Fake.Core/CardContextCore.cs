using System;
using WSCT.Core;
using WSCT.Wrapper;

namespace WSCT.Fake.Core
{
    /// <summary>
    /// Represents a basic object capable of managing smartcard resources.
    /// </summary>
    public class CardContextCore : ICardContext
    {
        #region >> Fields

        private bool _established = false;

        #endregion

        #region >> Constructor

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public CardContextCore()
        {
            _established = false;

            Context = IntPtr.Zero;
        }

        #endregion

        #region >> ICardContext Members

        /// <inheritdoc />
        public IntPtr Context { get; }

        /// <inheritdoc />
        public string[] Groups { get; private set; }

        /// <inheritdoc />
        public int GroupsCount
        {
            get { return Groups.Length; }
        }

        /// <inheritdoc />
        public string[] Readers { get; private set; }

        /// <inheritdoc />
        public int ReadersCount
        {
            get { return Readers.Length; }
        }

        /// <inheritdoc />
        public ErrorCode Cancel()
        {
            if (!_established)
            {
                return ErrorCode.ErrorInvalidHandle;
            }

            return ErrorCode.Success;
        }

        /// <inheritdoc />
        public virtual ErrorCode Establish()
        {
            if (_established)
            {
                return ErrorCode.Unexpected;
            }

            _established = true;

            return ErrorCode.Success;
        }

        /// <inheritdoc />
        public ErrorCode GetStatusChange(uint timeout, AbstractReaderState[] readerStates)
        {
            if (!_established)
            {
                return ErrorCode.ErrorInvalidHandle;
            }

            // TODO

            return ErrorCode.Success;
        }

        /// <inheritdoc />
        public ErrorCode IsValid()
        {
            return _established ? ErrorCode.Success : ErrorCode.InvalidHandle;
        }

        /// <inheritdoc />
        public virtual ErrorCode ListReaders(string group)
        {
            if (!_established)
            {
                return ErrorCode.ErrorInvalidHandle;
            }

            Readers = new[] { "WSCT.Fake Reader" };

            return ErrorCode.Success;
        }

        /// <inheritdoc />
        public virtual ErrorCode ListReaderGroups()
        {
            if (!_established)
            {
                return ErrorCode.ErrorInvalidHandle;
            }

            Groups = new[] { "WSCT.Fake Reader Group" };

            return ErrorCode.Success;
        }

        /// <inheritdoc />
        public virtual ErrorCode Release()
        {
            if (!_established)
            {
                return ErrorCode.ErrorInvalidHandle;
            }

            _established = false;

            return ErrorCode.Success;
        }

        #endregion
    }
}