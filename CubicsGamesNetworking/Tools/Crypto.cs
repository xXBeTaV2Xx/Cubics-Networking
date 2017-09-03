using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CubicsGamesNetworking.Tools
{
    public static class Crypto
    {
        public static string DecryptXOR(string encripTedString, string passPhrase)
        {
            return EncryptXOR(encripTedString, passPhrase);
        }

        public static string EncryptXOR(string stringToEncript, string passPhrase)
        {
            var encriptedStringBuilder = new StringBuilder(stringToEncript.Length);
            int positionInPassword = 0;

            for (int i = 0; i < stringToEncript.Length; i++)
            {
                __corectPositionInPassWord(ref positionInPassword, passPhrase);
                encriptedStringBuilder.Append((char)((int)stringToEncript[i] ^ (int)passPhrase[positionInPassword]));
                ++positionInPassword;
            }
            return encriptedStringBuilder.ToString();
        }

        public static string EncryptSHA1(string text)
        {
            var sha1 = new SHA1CryptoServiceProvider();

            string result = null;

            var arrayData = Encoding.ASCII.GetBytes(text);
            var arrayResult = sha1.ComputeHash(arrayData);
            foreach (var t in arrayResult)
            {
                var temp = Convert.ToString(t, 16);
                if (temp.Length == 1)
                    temp = string.Format("0{0}", temp);
                result += temp;
            }
            return result;
        }

        private static void __corectPositionInPassWord(ref int positionInPassword, string passPhrase)
        {
            if (positionInPassword == passPhrase.Length)
            {
                positionInPassword = 0;
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
