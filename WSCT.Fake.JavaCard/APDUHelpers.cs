using WSCT.ISO7816;

namespace WSCT.Fake.JavaCard
{
    public static class APDUHelpers
    {
        public static short GetOffsetCdata(CommandAPDU cApdu)
        {
            var lcLength = cApdu.HasLc ? (cApdu.Lc < 0x0100 ? 1 : 2) : 0;

            return (short)(4 + lcLength);
        }

        public static short GetIncomingLength(CommandAPDU cApdu)
        {
            return (short)cApdu.Lc;
        }
    }
}