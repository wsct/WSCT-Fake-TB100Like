using NUnit.Framework;
using WSCT.Helpers;
using WSCT.ISO7816;
using WSCT.Wrapper;

namespace WSCT.Fake.TB100Like.Core
{
    [TestFixture]
    public class JavaCardAppletUnitTests
    {
        [Test]
        public void FSearchInMasterFileShouldFindFreeWordAt0035()
        {
            var fakeCard = new JavaCardApplet();
            var cApdu = new CommandAPDU("00 A0 00 00 00");

            var feedback = fakeCard.ExecuteCommand(cApdu);

            Assert.AreEqual(ErrorCode.Success, feedback.ErrorCode);
            Assert.AreEqual(0x9000, feedback.RApdu.StatusWord);
            Assert.AreEqual(8, feedback.RApdu.Udr.Length);
            Assert.AreEqual("00 35 02 9F 00 00 00 00", feedback.RApdu.Udr.ToHexa());
        }
    }
}
