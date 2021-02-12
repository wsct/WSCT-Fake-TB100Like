namespace WSCT.Fake.JavaCard.Security
{
    public interface Key
    {
        void ClearKey();

        short GetSize();

        byte GetType();

        bool IsInitialized();
    }
}
