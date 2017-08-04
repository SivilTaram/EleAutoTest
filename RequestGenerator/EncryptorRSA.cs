using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RequestGenerator.Encrypt
{
    public static class GenratedKeys
    {
        public const string PublicKey =
            "NDA5NiE8UlNBS2V5VmFsdWU+PE1vZHVsdXM+akFSRXV6MjJPSTZ5bmVLWnN0aWtQb2pzVlBnRUtOSnhYR1BUYkFQZEk1K0Fja2R6MzVqREVISnJDNjRXNkdlQUNQcldJckpkN3pLQ1BWd1V6TCtrTnZhL3pGd3RpUGRMczdzUUJCRzFKNmVpc0ZCY1REOTFLUk9WOXU2czdSdzRLNVpGWlZ2bm1qV3ZlQnQyQWhqbzFVRFFwQ0RXRXFOdW1BVFdyR3IwWHo0Q2lLMlh3VjVkMExRZWZSZnpWcnNJWWI0bEJnYkcxc3VDOWxyTE1JL2w2SE9ZSnZ2bFFTWXhCcFRrUVJkMi9yUERhS1hIU2ZvUE1PUlJaU1VuK3FIMWxYelJGWFh3VzRERVlubWFPT21xQkJSajJTSEFpY25FRWVER2Z5QnJQSE5sYURjTTdRK201U0ZFeDVzeUJEU3lkc3dOdHJqWGhHSVBRNkUzZ050cGJ1U0pqbmNuOGJzNlllT1d5a2tza1JKdkhpMVZxdElMQTN0dElwWVpNNksrTzhmSURHZ0Z1S1R5UmM0TmJIZ1g1UGRvanhId1dlQU9Vc0dRV2NBQkVhNmlyTERpR3ZCblAyQ2JuMUVXUmxjRGpwVGx5QW01NExtWk5nUm5uaHBZQURubDFmT0RON3lGSmd4NWgwWVdzcXhNaW0zRHRUMDFhUU53cm1NdHM0UGVjK3gvUU9NRnJnM1RGelZJY0thQWg2WHFGWE9vWjBhS3ZwT3JrOFluVHY1bTNaMVhvZHU3ZHEvVExOdVkyNWFFaG13c1ErcjlQZS9wbytpY2c0aWJNd3VYUlhWVVpEV3o2dmNYUEpmUXFYdFc4V1N0dHBwUktRcXJ3MkxmK1FvU1ZKeEZ3ZU1meXJDbE9aZElwM0lYOU81MjF5ZUE4N3RsT2czYXlDTWltTlU9PC9Nb2R1bHVzPjxFeHBvbmVudD5BUUFCPC9FeHBvbmVudD48L1JTQUtleVZhbHVlPg==";
    }

    public static class EncryptorRSA
    {
        private static bool _optimalAsymmetricEncryptionPadding = false;

        public static string EncryptText(string text, string publicKey)
        {
            int keySize = 0;
            string publicKeyXml = "";

            GetKeyFromEncryptionString(publicKey, out keySize, out publicKeyXml);

            var encrypted = Encrypt(Encoding.UTF8.GetBytes(text), keySize, publicKeyXml);
            return Convert.ToBase64String(encrypted);
        }

        private static byte[] Encrypt(byte[] data, int keySize, string publicKeyXml)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Data are empty", "data");
            if (String.IsNullOrEmpty(publicKeyXml)) throw new ArgumentException("Key is null or empty", "publicKeyXml");

            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                provider.FromXmlString(publicKeyXml);
                return provider.Encrypt(data, _optimalAsymmetricEncryptionPadding);
            }
        }

        private static void GetKeyFromEncryptionString(string rawkey, out int keySize, out string xmlKey)
        {
            keySize = 0;
            xmlKey = "";

            if (!string.IsNullOrEmpty(rawkey))
            {
                byte[] keyBytes = Convert.FromBase64String(rawkey);
                var stringKey = Encoding.UTF8.GetString(keyBytes);

                if (stringKey.Contains("!"))
                {
                    var splittedValues = stringKey.Split(new char[] { '!' }, 2);

                    try
                    {
                        keySize = int.Parse(splittedValues[0]);
                        xmlKey = splittedValues[1];
                    }
                    catch (Exception e) { }
                }
            }
        }
    }
}
