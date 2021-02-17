using WSCT.ISO7816;

namespace WSCT.Fake.JavaCard
{
    public static class APDUHelpers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public static short getOffsetCdata(CommandAPDU cApdu)
        {
            var lcLength = cApdu.HasLc ? (cApdu.Lc < 0x0100 ? 1 : 2) : 0;

            return (short)(4 + lcLength);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public static short getIncomingLength(CommandAPDU cApdu)
        {
            return (short)cApdu.Lc;
        }
    }
}