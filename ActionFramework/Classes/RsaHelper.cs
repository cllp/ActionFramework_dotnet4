using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Classes
{
    public class RsaHelper
    {
        //initializing the RSA object taking the option of a 1024 key size
        readonly RSACryptoServiceProvider _rsa = new RSACryptoServiceProvider(1024);
        readonly string _privateKey;
        readonly string _publicKey;

        public RsaHelper()
        {
            _privateKey = _rsa.ToXmlString(true);
            _publicKey = _rsa.ToXmlString(false);
        }

        public SecureString Decrypt(string ciphertext, string privateKey_ = null)
        {
            if (ciphertext.Length <= 0) throw new ArgumentNullException("ciphertext");

            string key = String.IsNullOrEmpty(privateKey_) ? _privateKey : privateKey_;
            return DecryptToBytes(ciphertext, key);
        }
        private SecureString DecryptToBytes(string ciphertext, string privateKey)
        {
            if (String.IsNullOrEmpty(privateKey)) throw new ArgumentNullException("privateKey");

            byte[] ciphertext_Bytes = Convert.FromBase64String(ciphertext);
            _rsa.FromXmlString(privateKey);

            byte[] plainbytes = _rsa.Decrypt(ciphertext_Bytes, false);
            char[] plain = Encoding.Unicode.GetChars(plainbytes);
            var retval = new SecureString();
            FromArray(retval, ref plain);
            return retval;
        }

        public string Encrypt(SecureString plaintext, string publicKey_ = null)
        {
            if (plaintext == null) throw new ArgumentNullException("plaintext");
            if (plaintext.Length <= 0) throw new ArgumentOutOfRangeException("plaintext");

            string key = String.IsNullOrEmpty(publicKey_) ? _publicKey : publicKey_;
            return EncryptToBytes(plaintext, key);
        }
        private string EncryptToBytes(SecureString plaintext, string publicKey)
        {
            if (String.IsNullOrEmpty(publicKey)) throw new ArgumentNullException("publicKey");

            byte[] plaintext_Bytes = SecureStringToByteArray(plaintext);
            _rsa.FromXmlString(publicKey);

            byte[] ciphertext = _rsa.Encrypt(plaintext_Bytes, false);
            return Convert.ToBase64String(ciphertext);
        }

        // YIKES! Copies the SecureString to a byte[] array; use with caution!
        static byte[] SecureStringToByteArray(SecureString secureString)
        {
            if (secureString == null) throw new ArgumentNullException(/*MSG0*/"secureString");

            // copy plaintext from unmanaged memory into managed byte[]s
            int stringSize = secureString.Length * sizeof(char); // http://blogs.msdn.com/shawnfa/archive/2005/02/25/380592.aspx
            byte[] bytes = new byte[stringSize];
            IntPtr pString = Marshal.SecureStringToGlobalAllocUnicode(secureString); // do this last to minimize the availability of "pString"
            try // free "pString" even if Copy() throws
            {
                Marshal.Copy(pString, bytes, 0, stringSize);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(pString); // clear plaintext from unmanaged memory ASAP
            }

            return bytes;
        }

        static void ArrayZero<T>(ref T[] array)
        {
            Array.Clear(array, 0, array.Length); // clear from managed memory
            Array.Resize(ref array, 0); // set Length=0 ASAP; even the length can be valuable
            array = null; // just to "help" callers know there's nothing in the array anymore
        }

        static void FromArray(SecureString secureString, ref char[] array)
        {
            try // clear "array" even if AppendChar() throws
            {
                for (int i = 0; i < array.Length; i++)
                {
                    secureString.AppendChar(array[i]);
                    array[i] = default(char); // clear plaintext ASAP
                }
            }
            finally
            {
                ArrayZero(ref array); // clear plaintext from managed memory
            }
        }
    }
}
