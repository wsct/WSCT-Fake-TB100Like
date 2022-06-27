using System;
using WSCT.Fake.JavaCard;

namespace WSCT.Fake.GlobalPlatform
{
    public class SecureChannel
    {
        public const byte ANY_AUTHENTICATED = 0b_0100_0000;
        public const byte AUTHENTICATED = 0b_1000_0000;
        public const byte C_DECRYPTION = 0b_0000_0010;
        public const byte C_MAC = 0b_0000_0001;
        public const byte NO_SECURITY_LEVEL = 0b_0000_0000;
        public const byte R_ENCRYPTION = 0b_0010_0000;
        public const byte R_MAC = 0b_0001_0000;

        /// <summary>
        /// Processes security related APDU commands.<br/>
        /// This method is used by an applet to process APDU commands that possibly relate to the security mechanism used by the Security Domain. As the intention is to allow an applet to be associated with a Security Domain without having any knowledge of the security mechanisms used by the Security Domain, the applet assumes that APDU commands that it does not recognize are part of the security mechanism and will be recognized by the Security Domain. The applet can either invoke this method prior to determining if it recognizes the instruction or only invoke this method for instructions it does not recognize.
        /// </summary>
        /// <param name="apdu"></param>
        /// <returns>the number of bytes to be output.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public short processSecurity(APDU apdu)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is used to apply additional security processing to outgoing response data and Status Words according to the security level.<br/>
        /// Notes:
        /// <list type="bullet">
        ///   <item>The applet is able to ensure that the security level it requires (R_MAC, R_ENCRYPTION) will be applied by invoking the getSecurityLevel() method.</item>
        ///   <item>The getSecurityLevel() method invocation may also indicate that entity authentication (AUTHENTICATED) or (ANY_AUTHENTICATED) has previously occurred.</item>
        ///   <item>If NO_SECURITY_LEVEL is indicated, this method will do no processing.</item>
        /// </list>
        /// </summary>
        /// <param name="baBuffer"></param>
        /// <param name="sOffset"></param>
        /// <param name="sLength"></param>
        /// <returns>the length of the wrapped data.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public short wrap(byte[] baBuffer, short sOffset, short sLength)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is used to process and verify the secure messaging of an incoming command according to the security level.<br/>
        /// Notes:
        /// <list type="bullet">
        ///   <item>The applet is able to query what level of security will be assumed (C_MAC, C_DECRYPTION) to be present by the Security Domain by invoking the getSecurityLevel() method.</item>
        ///   <item>The getSecurityLevel() method invocation may also indicate that entity authentication (AUTHENTICATED) or (ANY_AUTHENTICATED) has previously occurred.</item>
        ///   <item>If NO_SECURITY_LEVEL is indicated, this method will do no processing.</item>
        ///   <item>If the class byte does not indicate secure messaging (ISO 7816-4), this method will do no processing.</item>
        ///   <item>The applet is responsible for receiving the data field of the command.</item>
        ///   <item>Correct processing of the unwrap method will result in the incoming command being reformatted within the incoming APDU object with all data relating to the Secure Messaging removed.</item>
        ///   <item>Incorrect processing of the unwrap method will result in the information relating to the current Secure Channel being reset.</item>
        /// </list>
        /// </summary>
        /// <param name="baBuffer"></param>
        /// <param name="sOffset"></param>
        /// <param name="sLength"></param>
        /// <returns>the length of the unwrapped data.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public short unwrap(byte[] baBuffer, short sOffset, short sLength)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is used to decrypt data located in the input buffer.<br/>
        /// Notes:
        /// <list type="bullet">
        ///   <item>The Security Domain implicitly knows the key to be used for decryption.</item>
        ///   <item>The Security Domain is implicitly aware of any padding that may be present in the decrypted data and this is discarded.</item>
        ///   <item>The clear text data replaces the ciphered data within the byte array. The removal of padding may cause the length of the clear text data to be shorter than the length of the ciphered data.</item>
        ///   <item>The applet is responsible for checking the integrity of the decrypted data.</item>
        /// </list>
        /// </summary>
        /// <param name="baBuffer"></param>
        /// <param name="sOffset"></param>
        /// <param name="sLength"></param>
        /// <returns>The length of the clear text data.</returns>
        public short decryptData(byte[] baBuffer, short sOffset, short sLength)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is used to encrypt data located in the input buffer.<br/>
        /// Notes:
        /// <list type="bullet">
        ///   <item>The Security Domain is implicitly aware of any padding that must be applied to the clear text data prior to encryption.</item>
        ///   <item>The Security Domain implicitly knows the key to be used for encryption.</item>
        ///   <item>The ciphered data replaces the clear text data within the byte array.</item>
        ///   <item>Due to padding, the length of the ciphered data may be longer than the length of the clear text data and the applet must make allowances for this.</item>
        /// </list>
        /// </summary>
        /// <param name="baBuffer"></param>
        /// <param name="sOffset"></param>
        /// <param name="sLength"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public short encryptData(byte[] baBuffer, short sOffset, short sLength)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is used to reset information relating to the current Secure Channel.<br/>
        /// Notes:
        /// <list type="bullet">
        ///   <item>Applets using the services of a Security Domain should invoke this method in the Applet.deselect() method.</item>
        ///   <item>The Security Domain will reset all information relating to the current Secure Channel, i.e. all Secure Channel session keys, state information and security level information will be erased.</item>
        ///   <item>This method shall not fail if no Secure Channel has been initiated.</item>
        /// </list>
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public void resetSecurity()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is used to determine whether the Security Domain has performed authentication and to determine what level of security will be applied by the wrap and unwrap methods.<br/>
        /// Notes:
        /// <list type="bullet">
        ///   <item>Applets must invoke this method to ensure that application specific security requirements have been previously met or will be enforced by the Security Domain.</item>
        ///   <item>More than one level of security may be active and these may change during a Secure Channel, e.g. an R_MAC session may be initiated during a C_MAC session.</item>
        /// </list>
        /// </summary>
        /// <returns>NO_SECURITY_LEVEL (0x00) indicating that Entity Authentication has not occurred and that the wrap and unwrap methods will not apply any cryptographic processing to command or response data, or a bitmap of the security level as follows:
        /// <list type="bullet">
        ///   <item>AUTHENTICATED (0x80): Entity authentication has occurred as Application Provider.</item>
        ///   <item>ANY_AUTHENTICATED (0x40): Entity authentication has occurred but not as Application Provider.</item>
        ///   <item>C_MAC (0x01): The unwrap method will verify the MAC on the incoming command.</item>
        ///   <item>R_MAC (0x10): The wrap method will generate a MAC for the outgoing response data.</item>
        ///   <item>C_DECRYPTION (0x02): The unwrap method will decrypt the incoming command data.</item>
        ///   <item>R_ENCRYPTION (0x20): The wrap method will encrypt the outgoing response data.</item>
        /// </list>
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard method")]
        public byte getSecurityLevel()
        {
            throw new NotImplementedException();
        }
    }
}
