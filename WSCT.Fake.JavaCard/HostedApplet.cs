namespace WSCT.Fake.JavaCard
{
    public record HostedApplet(
        byte[] Aid,
        Applet Applet
    );
}
