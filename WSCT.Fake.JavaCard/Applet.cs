namespace WSCT.Fake.JavaCard
{
    public abstract class Applet
    {
        public abstract void process(APDU apdu);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public virtual bool select()
        {
            return false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public virtual void deselect()
        {
        }

        /// <summary>
        /// javacard.framework.Applet#install(byte[], short, byte).
        /// </summary>
        /// <param name="bArray"></param>
        /// <param name="bOffset"></param>
        /// <param name="bLength"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public abstract void install(byte[] bArray, short bOffset, byte bLength);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        protected void register(byte[] _1 /* bArray*/, short _2 /*bOffset*/, byte _3 /*bLength*/)
        {
            // TODO ?
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        protected bool selectingApplet() => IsSelectingAppletState;

        internal bool IsSelectingAppletState { private get; set; }
    }
}
