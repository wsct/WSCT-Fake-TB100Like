using System;
using NUnit.Framework;
using WSCT.Fake.Core;
using WSCT.Helpers;
using WSCT.ISO7816;
using WSCT.Wrapper;

namespace WSCT.Fake.TB100Like.Core
{
    /// <summary>
    /// Unit tests based on questions asked to the students.
    /// </summary>
    [TestFixture]
    public class JavaCardRuntimeUnitTests
    {
        private static string ToHexa4(int value)
        {
            return new[] { (byte)(value / 0x100), (byte)(value % 0x100) }.ToHexa();
        }

        private IFakeCard CreateFakeCard() => new JavaCardT0ForTB100Adapter(new JavaCardApplet());

        [TestCase(0x0000, "00 35")]
        [TestCase(0x0034, "00 35")]
        [TestCase(0x0038, "00 38")]
        public void FSearchInMasterFileShouldFindFreeWordAt0035(int searchOffset, string expectedFoundOffset)
        {
            var fakeCard = CreateFakeCard();

            var fSearchApdu = new CommandAPDU($"00 A0 {searchOffset:X4} 00");
            var feedbackToFSearch = fakeCard.ExecuteCommand(fSearchApdu);

            Assert.AreEqual(ErrorCode.Success, feedbackToFSearch.ErrorCode);
            Assert.AreEqual(0x9000, feedbackToFSearch.RApdu.StatusWord);
            Assert.AreEqual(0, feedbackToFSearch.RApdu.Udr.Length);

            var getResponseApdu = new CommandAPDU("00 C0 00 00 08");
            var feedbackToGetResponse = fakeCard.ExecuteCommand(getResponseApdu);

            Assert.AreEqual(ErrorCode.Success, feedbackToGetResponse.ErrorCode);
            Assert.AreEqual(0x9000, feedbackToGetResponse.RApdu.StatusWord);
            Assert.AreEqual($"{expectedFoundOffset} 02 9F FF FF FF FF", feedbackToGetResponse.RApdu.Udr.ToHexa());
        }

        [Test]
        public void GenerateRandomWithRightLeShouldSucceed()
        {
            var fakeCard = CreateFakeCard();

            var generateRandomApdu = new CommandAPDU($"00 C4 00 00 08");
            var feedbackToGenerateRandom = fakeCard.ExecuteCommand(generateRandomApdu);

            Assert.AreEqual(ErrorCode.Success, feedbackToGenerateRandom.ErrorCode);
            Assert.AreEqual(0x9000, feedbackToGenerateRandom.RApdu.StatusWord);
            Assert.AreEqual(8, feedbackToGenerateRandom.RApdu.Udr.Length);
        }

        [TestCase(0x06)]
        [TestCase(0x12)]
        public void GenerateRandomWithWrongLeShouldFail(int le)
        {
            var fakeCard = CreateFakeCard();

            var generateRandomApdu = new CommandAPDU($"00 C4 00 00 {le:X2}");
            var feedbackToGenerateRandom = fakeCard.ExecuteCommand(generateRandomApdu);

            Assert.AreEqual(ErrorCode.Success, feedbackToGenerateRandom.ErrorCode);
            Assert.AreEqual(0x6700, feedbackToGenerateRandom.RApdu.StatusWord);
            Assert.AreEqual(0, feedbackToGenerateRandom.RApdu.Udr.Length);
        }

        [Test]
        public void ReadMFShouldSucceed()
        {
            var fakeCard = CreateFakeCard();

            var readApdu = new CommandAPDU("00 B0 00 00 40");
            var feedbackToRead = fakeCard.ExecuteCommand(readApdu);

            Assert.AreEqual(ErrorCode.Success, feedbackToRead.ErrorCode);
            Assert.AreEqual(0x9000, feedbackToRead.RApdu.StatusWord);
            var filler = new byte[0x40 - 24];
            Array.Fill(filler, (byte)0xFF);
            Assert.AreEqual($"17 FF 06 E4 1F 6C 05 70 0E 2F F5 CE 0E 10 03 DF 7F 00 00 22 FF FF FE 62 {filler.ToHexa()}", feedbackToRead.RApdu.Udr.ToHexa());

        }

        [TestCase(0x08, "00 00 02 9F 3F 00 02 A1")]
        [TestCase(0x0C, "00 00 02 9F 3F 00 02 A1 FF FF 9E 81")]
        public void SelectMFWithRightLeShouldSucceed(int le, string expectedOnGetResponse)
        {
            var fakeCard = CreateFakeCard();

            var selectApdu = new CommandAPDU("00 A4 00 00 02 3F 00");
            var feedbackToSelect = fakeCard.ExecuteCommand(selectApdu);

            Assert.AreEqual(ErrorCode.Success, feedbackToSelect.ErrorCode);
            Assert.AreEqual(0x9000, feedbackToSelect.RApdu.StatusWord);
            Assert.AreEqual(0, feedbackToSelect.RApdu.Udr.Length);

            var getResponseApdu = new CommandAPDU($"00 C0 00 00 {le:X2}");
            var feedbackToGetResponse = fakeCard.ExecuteCommand(getResponseApdu);

            Assert.AreEqual(ErrorCode.Success, feedbackToGetResponse.ErrorCode);
            Assert.AreEqual(0x9000, feedbackToGetResponse.RApdu.StatusWord);
            Assert.AreEqual(le, feedbackToGetResponse.RApdu.Udr.Length);
            Assert.AreEqual(expectedOnGetResponse, feedbackToGetResponse.RApdu.Udr.ToHexa());
        }

        [TestCase(0x06)]
        [TestCase(0x12)]
        public void SelectMFWithWrongLeShouldFailOnGetResponse(int le)
        {
            var fakeCard = CreateFakeCard();

            var selectApdu = new CommandAPDU("00 A4 00 00 02 3F 00");
            var feedbackToSelect = fakeCard.ExecuteCommand(selectApdu);

            Assert.AreEqual(ErrorCode.Success, feedbackToSelect.ErrorCode);
            Assert.AreEqual(0x9000, feedbackToSelect.RApdu.StatusWord);
            Assert.AreEqual(0, feedbackToSelect.RApdu.Udr.Length);

            var getResponseApdu = new CommandAPDU($"00 C0 00 00 {le:X2}");
            var feedbackToGetResponse = fakeCard.ExecuteCommand(getResponseApdu);

            Assert.AreEqual(ErrorCode.Success, feedbackToGetResponse.ErrorCode);
            Assert.AreEqual(0x6700, feedbackToGetResponse.RApdu.StatusWord);
            Assert.AreEqual(0, feedbackToGetResponse.RApdu.Udr.Length);
        }
    }
}
