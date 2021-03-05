using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WSCT.Fake.Core;
using WSCT.ISO7816;

namespace WSCT.Fake.JavaCard
{
    public class FakeJavaCard : IFakeCard
    {
        private readonly List<HostedApplet> _hostedApplets = new();

        private Applet _applet;
        private byte[] _appletAid;

        private static readonly byte[] DefaultAtr = new byte[] { 0x3F, 0x67, 0x25, 0x00, 0x21, 0x20, 0x00, 0x0F, 0x78, 0x90, 0x00 };

        private byte[] _atr = DefaultAtr;

        private bool _isActive;

        public FakeJavaCard(Applet applet, byte[] aid)
        {
            Install(applet, aid);
        }

        public FakeJavaCard(IEnumerable<HostedApplet> hostedApplets)
        {
            foreach (var (aid, applet) in hostedApplets)
            {
                Install(applet, aid);
            }
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
            switch (commandApdu)
            {
                case { Cla: 0x00, Ins: 0xA4, P1: 0x04, P2: 0x00 }: // JavaCard SELECT of an applet
                    var selectedApplet = _hostedApplets.FirstOrDefault(a => a.Aid.SequenceEqual(commandApdu.Udc));
                    if (selectedApplet != null)
                    {
                        _applet?.deselect();

                        var hostedApplet = selectedApplet;
                        (_appletAid, _applet) = hostedApplet;
                    }
                    else
                    {
                        if (_applet == null)
                        {
                            return FakeCardFeedback.FromSuccess(0x6A82); // file or application not found
                        }
                    }
                    break;
            }

            return ForwardCommandToCurrentApplet(commandApdu);
        }

        public IFakeCardFeedback ForwardCommandToCurrentApplet(CommandAPDU commandApdu)
        {
            try
            {
                _applet.IsSelectingAppletState = commandApdu.Cla == 0x00
                                                 && commandApdu.Ins == 0xA4
                                                 && _appletAid.SequenceEqual(commandApdu.Udc);

                var apdu = new APDU(commandApdu);

                _applet.process(apdu);

                var responseBytes = apdu.ResponseLength switch
                {
                    0 => apdu.ResponseBuffer.Take(apdu.ResponseLength).Concat(new byte[] { 0x90, 0x00 }).ToArray(),
                    _ => apdu.ResponseBuffer.Take(apdu.ResponseLength).ToArray()
                };

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

            JCSystem.ClearTransientOnDeselect();
            JCSystem.ClearTransientOnReset();

            _isActive = false;

            return true;

        }

        /// <inheritdoc />
        public bool WarmReset()
        {
            JCSystem.ClearTransientOnDeselect();
            JCSystem.ClearTransientOnReset();

            return _isActive;
        }

        #endregion

        public void Install(Applet applet, byte[] aid)
        {
            _hostedApplets.Add(new HostedApplet(aid, applet));

            // buffer : [aidLength] (aid)
            var buffer = new byte[1 + aid.Length];
            buffer[0] = (byte)aid.Length;
            Array.Copy(aid, 0, buffer, 1, aid.Length);

            applet.install(buffer, 0, (byte)buffer.Length);
        }

        public void SetAtr(byte[] atr)
        {
            _atr = atr;
        }
    }
}
