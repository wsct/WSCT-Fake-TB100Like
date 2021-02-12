using System;
using System.Linq;
using WSCT.Fake.Core;
using WSCT.ISO7816;

namespace WSCT.Fake.JavaCard
{
    public class FakeJavaCard : IFakeCard
    {
        private Applet _applet;
        private byte[] _appletAid;

        private static readonly byte[] DefaultAtr = new byte[] { 0x3F, 0x67, 0x25, 0x00, 0x21, 0x20, 0x00, 0x0F, 0x78, 0x90, 0x00 };

        private byte[] _atr = DefaultAtr;

        private bool _isActive;

        public FakeJavaCard()
        {
        }

        public FakeJavaCard(Applet applet, byte[] aid)
        {
            Install(applet, aid);
        }

        #region >> IFakeCard

        /// <inheritdoc />
        public byte[] GetAtr() => _atr;

        /// <inheritdoc />
        public bool ColdReset() => Unpower() && Connect();

        /// <inheritdoc />
        public bool Connect()
        {
            _isActive = true;

            return true;
        }

        /// <inheritdoc />
        public IFakeCardFeedback ExecuteCommand(CommandAPDU commandApdu)
        {
            try
            {
                _applet.IsSelectingAppletState = commandApdu.Cla == 0x00
                    && commandApdu.Ins == 0xA4
                    && _appletAid.SequenceEqual(commandApdu.Udc);

                var apdu = new APDU(commandApdu);

                _applet.Process(apdu);

                byte[] responseBytes = apdu.ResponseBuffer.Take(apdu.ResponseLength).ToArray();

                if (apdu.ResponseLength == 0)
                {
                    responseBytes = apdu.ResponseBuffer.Take(apdu.ResponseLength).Concat(new byte[] { 0x90, 0x00 }).ToArray();
                }
                else
                {
                    responseBytes = apdu.ResponseBuffer.Take(apdu.ResponseLength).ToArray();
                }

                return FakeCardFeedback.FromSuccess(responseBytes);
            }
            catch (ISOException exception)
            {
                return FakeCardFeedback.FromSuccess(exception.StatusWord);
            }
        }

        /// <inheritdoc />
        public bool Unpower()
        {
            if (!_isActive)
            {
                return false;
            }

            _isActive = false;

            return true;

        }

        /// <inheritdoc />
        public bool WarmReset()
        {
            return _isActive;
        }

        #endregion

        public void Install(Applet applet, byte[] aid)
        {
            _applet = applet;
            _appletAid = aid;

            // buffer : [aidLength] (aid)
            var buffer = new byte[1 + aid.Length];
            buffer[0] = (byte)aid.Length;
            Array.Copy(aid, 0, buffer, 1, aid.Length);

            _applet.Install(buffer, 0, (byte)buffer.Length);
        }

        public void SetAtr(byte[] atr)
        {
            _atr = atr;
        }
    }
}
