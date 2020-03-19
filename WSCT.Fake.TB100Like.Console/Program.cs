using System;
using System.Linq;
using WSCT.Core;
using WSCT.Core.APDU;
using WSCT.Fake.Core;
using WSCT.Fake.TB100Like;
using WSCT.Helpers;
using WSCT.ISO7816;
using WSCT.ISO7816.Commands;
using WSCT.Wrapper;

namespace WSCT.Fake.ConsoleTests
{
    class Program
    {
        private static ICardChannel channel;

        static void Main(string[] args)
        {
            Func<ICardContext, ICardChannel> createCardChannel;
            Action processCommands;
            if (args.Length == 2)
            {
                switch (args[1])
                {
                    case "applet":
                        createCardChannel = (context) => new CardChannel(context, context.Readers.Last(), new JavaCardApplet());
                        processCommands = ProcessTb100FakeCardCommands;
                        break;
                    case "javacard":
                    default:
                        createCardChannel = (context) => new CardChannel(context, context.Readers.Last(), new JavaCardRuntime(new JavaCardApplet()));
                        processCommands = ProcessJavaCardTb100FakeCardCommands;
                        break;
                }
            }
            else
            {
                createCardChannel = (context) => new CardChannel(context, context.Readers.Last(), new JavaCardRuntime(new JavaCardApplet()));
                processCommands = ProcessJavaCardTb100FakeCardCommands;
            }

            // Connect to Fake PC/SC
            var context = new CardContext();
            context.Establish();

            // Get installed readers
            context.ListReaderGroups();
            context.ListReaders("");

            PrintReadersAndReaderGroups(context);

            // Connect to an ISO7816-4 card in the last reader found
            var rawChannel = createCardChannel(context);
            channel = new CardChannelIso7816(rawChannel);
            channel.Connect(ShareMode.Exclusive, Protocol.Any);

            PrintAttribute(Attrib.AtrString);
            PrintAttribute(Attrib.DeviceFriendlyName);

            processCommands();

            // Unpower the card
            channel.Disconnect(Disposition.UnpowerCard);

            // Disconnect from Fake PC/SC
            context.Release();
        }

        private static CommandResponsePair ExecuteCrp(CommandAPDU cApdu)
        {
            PrintCommand(cApdu);

            var crp = new CommandResponsePair(cApdu);

            var errorCode = crp.Transmit(channel);

            if (errorCode != ErrorCode.Success)
            {
                Console.WriteLine($"ErrorCode: {errorCode}");

                return crp;
            }

            PrintResponse(crp.RApdu);

            return crp;
        }

        private static void ExecuteCrpSelect(string fileIdentifier)
        {
            PrintHeader($"SELECT {fileIdentifier}");

            // Build a SELECT command
            var cApdu = new SelectCommand(
                SelectCommand.SelectionMode.SelectDFName,
                SelectCommand.FileOccurrence.FirstOrOnly,
                SelectCommand.FileControlInformation.ReturnFci,
                fileIdentifier.FromHexa()
                );

            ExecuteCrp(cApdu);
        }

        private static void ExecuteCrpCreateFile(string fileIdentifier, string fileControlInformation)
        {
            PrintHeader($"CREATE FILE {fileIdentifier}");

            var lc = $"{(fileIdentifier.Length + fileControlInformation.Length) / 2:X2}";
            var cApdu = new CommandAPDU($"00E00040{lc}{fileIdentifier}{fileControlInformation}");

            ExecuteCrp(cApdu);
        }

        private static void ExecuteCrpDeleteFile(string fileIdentifier)
        {
            PrintHeader($"DELETE FILE {fileIdentifier}");

            var lc = $"{fileIdentifier.Length / 2:X2}";
            var cApdu = new CommandAPDU($"00E40040{lc}{fileIdentifier}");

            ExecuteCrp(cApdu);
        }

        private static void ExecuteCrpErase(short offset, short length)
        {
            PrintHeader($"ERASE {length:X4} word(s) @{offset:X4}");

            var cApdu = new CommandAPDU($"000E{offset:X4}02{length:X4}");

            ExecuteCrp(cApdu);
        }

        private static void ExecuteCrpFSearch(short offset)
        {
            PrintHeader($"FSEARCH @{offset:X4}");

            var cApdu = new CommandAPDU($"00A0{offset:X4}04");

            ExecuteCrp(cApdu);
        }

        private static void ExecuteCrpGenerateRandom()
        {
            PrintHeader("GENERATE RANDOM");

            var cApdu = new CommandAPDU(0x00, 0xC4, 0x00, 0x00, 0x08);

            ExecuteCrp(cApdu);
        }

        private static void ExecuteCrpGetResponse(byte le)
        {
            PrintHeader($"GET RESPONSE {le:X4} bytes");

            var cApdu = new CommandAPDU($"00C00000{le:X2}");

            ExecuteCrp(cApdu);
        }

        private static void ExecuteCrpRead(short offset, byte le)
        {
            PrintHeader($"READ {le:X2} word(s) @{offset:X4}");

            var cApdu = new CommandAPDU($"00B0{offset:X4}{le:X2}");

            ExecuteCrp(cApdu);
        }

        private static void ExecuteCrpWrite(short offset, string udc)
        {
            var lc = udc.Length / 2;

            PrintHeader($"WRITE {lc:X2} word(s) @{offset:X4}");

            var cApdu = new CommandAPDU($"00D0{offset:X4}{lc:X2}{udc}");

            ExecuteCrp(cApdu);
        }

        private static void PrintAttribute(Attrib atrString)
        {
            byte[] bytes = null;
            channel.GetAttrib(atrString, ref bytes);

            Console.WriteLine($"{atrString}: {bytes.ToHexa()}");
        }

        private static void PrintCommand(ICardCommand command)
        {
            Console.WriteLine($"C-CAPDU: {command}");
        }

        private static void PrintHeader(string header)
        {
            Console.WriteLine($"==== {header} ====");
        }

        private static void PrintReadersAndReaderGroups(ICardContext context)
        {
            Console.WriteLine($"Groups: {context.Groups.Aggregate("", (a, g) => $"{a} {g};")}");
            Console.WriteLine($"Readers: {context.Readers.Aggregate("", (a, r) => $"{a} {r};")}");
        }

        private static void PrintResponse(ResponseAPDU response)
        {
            Console.WriteLine($"R-APDU: {response}");
        }

        private static void ProcessTb100FakeCardCommands()
        {
            // Generate Random
            ExecuteCrpGenerateRandom();
            ExecuteCrpGenerateRandom(); // (must be different thant previous GENERATE RANDOM)

            // Create & Delete file
            ExecuteCrpSelect("3F00");
            ExecuteCrpSelect("7F00"); // (expected to fail)
            ExecuteCrpSelect("3F00");
            ExecuteCrpDeleteFile("2F01");
            ExecuteCrpSelect("2F01"); // (expected to fail)
            ExecuteCrpCreateFile("2F01", "0010FFFFFF83"); // (EF)
            ExecuteCrpSelect("2F01");
            ExecuteCrpDeleteFile("2F01");
            ExecuteCrpSelect("2F01");

            // Create file structure
            ExecuteCrpCreateFile("7F00", "0022FFFFFE62");
            ExecuteCrpSelect("7F00");
            ExecuteCrpCreateFile("6F01", "0010FFFFFF83");
            ExecuteCrpFSearch(0x0000);
            ExecuteCrpSelect("6F01");

            // Read/Write/Erase/FSearch
            ExecuteCrpRead(0x0100, 0x10); // (expected to fail - offset too long)
            ExecuteCrpRead(0x0000, 0x39); // (expected to fail - length too long)
            ExecuteCrpRead(0x0000, 0x38);
            ExecuteCrpWrite(0x0100, "01020304050607"); // (expected to fail - offset too long)
            ExecuteCrpWrite(0x000D, "01020304050607"); // ((expected to fail - offset+length too long)
            ExecuteCrpWrite(0x0000, "01020304050607"); // (expected to fail - offset too long)
            ExecuteCrpRead(0x0000, 0x08);
            ExecuteCrpErase(0x0100, 0x0001); // (expected to fail - offset too long)
            ExecuteCrpErase(0x0000, 0x0100); // (expected to fail - length too long)
            ExecuteCrpErase(0x0000, 0x0001);
            ExecuteCrpRead(0x0000, 0x08);
            ExecuteCrpWrite(0x0000, "01020304050607"); // (expected to fail - already written)
            ExecuteCrpErase(0x0001, 0x0001);
            ExecuteCrpRead(0x0000, 0x08);
            ExecuteCrpFSearch(0x0000);
            ExecuteCrpWrite(0x0000, "0102030405060708");
            ExecuteCrpFSearch(0x0000);
            ExecuteCrpWrite(0x000C, "11121314");
            ExecuteCrpFSearch(0x000C);
            ExecuteCrpWrite(0x000C, "15161718");
            ExecuteCrpFSearch(0x000C);
            ExecuteCrpErase(0x0000, 0x000E);
            ExecuteCrpRead(0x0000, 0x08);
        }

        private static void ProcessJavaCardTb100FakeCardCommands()
        {
            // Generate Random
            ExecuteCrpGenerateRandom();
            ExecuteCrpGenerateRandom(); // (must be different thant previous GENERATE RANDOM)

            // Create & Delete file
            ExecuteCrpSelect("3F00");
            ExecuteCrpGetResponse(0x07); // (expected to fail: Le expected to be 0x08 or 0x0C)
            ExecuteCrpGetResponse(0x0C);
            ExecuteCrpGetResponse(0x0C);
            ExecuteCrpSelect("7F00"); // (expected to fail)
            ExecuteCrpSelect("3F00");
            ExecuteCrpDeleteFile("2F01");
            ExecuteCrpSelect("2F01"); // (expected to fail)
            ExecuteCrpCreateFile("2F01", "0010FFFFFF83"); // (EF)
            ExecuteCrpSelect("2F01");
            ExecuteCrpGetResponse(0x0C);
            ExecuteCrpDeleteFile("2F01");
            ExecuteCrpSelect("2F01");

            //// Create file structure
            ExecuteCrpCreateFile("7F00", "0022FFFFFE62");
            ExecuteCrpSelect("7F00");
            ExecuteCrpCreateFile("6F01", "0010FFFFFF83");
            ExecuteCrpFSearch(0x0000);
            ExecuteCrpGetResponse(0x08);
            ExecuteCrpSelect("6F01");

            //// Read/Write/Erase/FSearch
            //ExecuteCrpRead(0x0100, 0x10); // (expected to fail - offset too long)
            //ExecuteCrpRead(0x0000, 0x39); // (expected to fail - length too long)
            //ExecuteCrpRead(0x0000, 0x38);
            //ExecuteCrpWrite(0x0100, "01020304050607"); // (expected to fail - offset too long)
            //ExecuteCrpWrite(0x000D, "01020304050607"); // ((expected to fail - offset+length too long)
            //ExecuteCrpWrite(0x0000, "01020304050607"); // (expected to fail - offset too long)
            //ExecuteCrpRead(0x0000, 0x08);
            //ExecuteCrpErase(0x0100, 0x0001); // (expected to fail - offset too long)
            //ExecuteCrpErase(0x0000, 0x0100); // (expected to fail - length too long)
            //ExecuteCrpErase(0x0000, 0x0001);
            //ExecuteCrpRead(0x0000, 0x08);
            //ExecuteCrpWrite(0x0000, "01020304050607"); // (expected to fail - already written)
            //ExecuteCrpErase(0x0001, 0x0001);
            //ExecuteCrpRead(0x0000, 0x08);
            //ExecuteCrpFSearch(0x0000);
            //ExecuteCrpWrite(0x0000, "0102030405060708");
            //ExecuteCrpFSearch(0x0000);
            //ExecuteCrpWrite(0x000C, "11121314");
            //ExecuteCrpFSearch(0x000C);
            //ExecuteCrpWrite(0x000C, "15161718");
            //ExecuteCrpFSearch(0x000C);
            //ExecuteCrpErase(0x0000, 0x000E);
            //ExecuteCrpRead(0x0000, 0x08);
        }
    }
}
