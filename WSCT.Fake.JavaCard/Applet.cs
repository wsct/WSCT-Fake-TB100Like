namespace WSCT.Fake.JavaCard
{
    public abstract class Applet
    {
        public abstract void Process(APDU apdu);

        public virtual bool Select()
        {
            return false;
        }

        public virtual void Deselect()
        {
        }

        /// <summary>
        /// javacard.framework.Applet#install(byte[], short, byte).
        /// </summary>
        /// <param name="bArray"></param>
        /// <param name="bOffset"></param>
        /// <param name="bLength"></param>
        public abstract void Install(byte[] bArray, short bOffset, byte bLength);

        protected void Register(byte[] _1 /* bArray*/, short _2 /*bOffset*/, byte _3 /*bLength*/)
        {
            // TODO ?
        }

        protected bool SelectingApplet() => IsSelectingAppletState;

        internal bool IsSelectingAppletState { private get; set; }
    }
}
