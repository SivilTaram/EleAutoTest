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
        public const string PrivateKey = "NDA5NiE8UlNBS2V5VmFsdWU+PE1vZHVsdXM+akFSRXV6MjJPSTZ5bmVLWnN0aWtQb2pzVlBnRUtOSnhYR1BUYkFQZEk1K0Fja2R6MzVqREVISnJDNjRXNkdlQUNQcldJckpkN3pLQ1BWd1V6TCtrTnZhL3pGd3RpUGRMczdzUUJCRzFKNmVpc0ZCY1REOTFLUk9WOXU2czdSdzRLNVpGWlZ2bm1qV3ZlQnQyQWhqbzFVRFFwQ0RXRXFOdW1BVFdyR3IwWHo0Q2lLMlh3VjVkMExRZWZSZnpWcnNJWWI0bEJnYkcxc3VDOWxyTE1JL2w2SE9ZSnZ2bFFTWXhCcFRrUVJkMi9yUERhS1hIU2ZvUE1PUlJaU1VuK3FIMWxYelJGWFh3VzRERVlubWFPT21xQkJSajJTSEFpY25FRWVER2Z5QnJQSE5sYURjTTdRK201U0ZFeDVzeUJEU3lkc3dOdHJqWGhHSVBRNkUzZ050cGJ1U0pqbmNuOGJzNlllT1d5a2tza1JKdkhpMVZxdElMQTN0dElwWVpNNksrTzhmSURHZ0Z1S1R5UmM0TmJIZ1g1UGRvanhId1dlQU9Vc0dRV2NBQkVhNmlyTERpR3ZCblAyQ2JuMUVXUmxjRGpwVGx5QW01NExtWk5nUm5uaHBZQURubDFmT0RON3lGSmd4NWgwWVdzcXhNaW0zRHRUMDFhUU53cm1NdHM0UGVjK3gvUU9NRnJnM1RGelZJY0thQWg2WHFGWE9vWjBhS3ZwT3JrOFluVHY1bTNaMVhvZHU3ZHEvVExOdVkyNWFFaG13c1ErcjlQZS9wbytpY2c0aWJNd3VYUlhWVVpEV3o2dmNYUEpmUXFYdFc4V1N0dHBwUktRcXJ3MkxmK1FvU1ZKeEZ3ZU1meXJDbE9aZElwM0lYOU81MjF5ZUE4N3RsT2czYXlDTWltTlU9PC9Nb2R1bHVzPjxFeHBvbmVudD5BUUFCPC9FeHBvbmVudD48UD54Y0t4K1I3UkwvSnYvVGhwQk5YWS8zLzhqb3FodjdlQW5LQUxFMVBSQ0c4YlNxQmwyYkhQNGwvT0k0bmUvTVl5UnlTS3NOQlhWZUZHd3NKcnF0anZiNlNMOU1ZMVkyNVU3N0drK2ovQm9QbkwrT2dtMXdkY2Rsc21zakdHa1FXSUJWU0YyVzZXbndSZVhUMERhVUcvYk82cStxbFNiSHFhRk9LZkZzMFpNVmVZbVRoazFURUFjd3U1UmlONHF4Y3lqb3FIMGhPakZSYm9xOTdXV1M4bjMraGthME03YkRwcDlCWlVnYVZjUTFuWTF5U1Y1aFJWUit2TWZLTlJ6QVpwNVViMWxHRkY1aVh3cXZ5emlFZFJ3ZlZsWTZBRU1xSVVjdGd5ayt5enREazFaWVdwZ3p5SG9jNFVsdjVlU3J0R2R6QkFoZzV6NDgyR0c5K1NJTHFwb3c9PTwvUD48UT50VUE1UDhUbGZwQXRkbEViYVVkMC9QczUvUHRSMVlycU9qVEc0VU5nbWpYcWkvZWMya1VGaWtzajVWOVRJTU5PbGh6MFNpbTR4cHVtOVEyY1l5TEpOZDRhRHN0bGk4L21oMlBoVlh4TlBoNkwzaEd0Y1B2aFJVODRTdytmOEFJSS9ndnZ0TFV1VDcxZnk0N0U5NGNVZzlDb1dORGd1NE5zbjRWRmdZNk54WWdTeGJTbDZJTVRnM2dPWkl0ZzN4OTh4R3lVNHFaczV5SENyT3p0dWNvemhIdFBCcnRHOHBOM3dIQm55eGVLeFJNbDhwZXdPTXZycG9BV2ovT3lOdVRsdDZrcTE1YUo3L0ludXhqV2hxbmZ4bU5ZcUxNVUQwMjA3alRDOE1yK1ZLUjRiR0h1NU9BaFBsVlhUUE92YmxTRCt0WUcrdWM4T2ZjTTVWZmRIUUJMSnc9PTwvUT48RFA+SDExRGtMa2NnRGc1SHJZRm9laXdvOFVPcnlTNWhvaDU4MTFHcEdnVXJQdkNIbEhXRlhLbi84VW1FbWpaWHNtb1M3UTE4dGo0SVB5N2xrUHpnNE1vWmxKNGgvZXZtL2R4clJwRmloTDJpS3RWZUVLSmRtMEtjeEw5emlFZ3NIdHFHVDVRaEMyUDRlUjFldUFrWEZzN0d2RzNXRTNtYnFIN01makhObnlNMUZDS09UTENlelZzVnRLekxiYU9lc1FSRnA1TjhEbUJFZVV5UFovMzZ1a1pyOTlSbmRscDRUNWFjNFhmSFRpMnEwL2luaXlGUVpwVDlEWW5sbFRvc2kwS09ueklraUhqVGg4U1Rtd0pwVUY2YkFLWmMvVjY5UFRUdldzZ2ltbHVEdFZMN0xBSU5ISXpLS3hnZzVvUUpsWVJWUEhvS2RxODhwSTBCTk91ZjFaYmRRPT08L0RQPjxEUT5pMnNDWGwzckZhWjdnZFl6bXBDL21WVDVydkgyYWFiNE1wdWtHMlJEYTN0cmxneDZWNTdkK3hsL1hCRXVGVGFHUFl1NkVVMzBkcmtGL050aVh4TkQ2SzZXSnFrbnpCcTRQWTZEcHNqOWRYbnpwbmE4amZzQnRkbVErSlBsTjYycEV4bEZZaUEzcGxpSjFqOVVuUDRIdHJrS3RYNitUeDV4c0diSUp6YnJNSFZSMDBmNGtsZzQ2MWl1NFlJZ21xQlN6WmlrRVk5b2w0VnRoS3llMDltSU5URFFnZUlPV2NzN1F3aHpubEhXaVc4SU1vbWhvSjZpL2lqcXNnSUhKSmgvWnYyMkdhMFBIMFdLY0MxVDNyUE9rSUNja3U5a3o3eTdIcnZ3ODEyQnNzUHNhWm9QTTNoL3ViM1ZjRGpGeWIweTVBQTcwRVdNR3g1QWlpMUN4T3NUNHc9PTwvRFE+PEludmVyc2VRPnZrbWh6Ym85MEFhOXczZDJMTkI2RXNxQmhZdUtha3FhVHRIUW5lRG9RTG91UUI4bXIvRmdDTklRU3FnZnV4dlpqWXF3Tk9lWDBGREErRG1UZ3pyNmVRcWpBWkVRYUdyWFVuVkdHZVBDUUp4ZTlKOStmODV6eVdVcnVvdGw0eWZzR2RtenlYQ1hpVnI2d01HaDEzQjdMU1NieXQxOVRzNUhQQkFGK1F6dWNyOGVoakhKMVVMMkpOdmZXYWJ5VFBVd21nb2kySnR4U0dINERISXdONGIxY1dVdzNpYWcyWDFDbmJ4MU5TTHVaMklMWFZJZ2NKS3NvWTc2Z2ZjVkNKUndxRUlzQTBWUExkT0h6bWUzcmtlejNFdWF5R2FSZ3VsbHc5Qm43ZmRJWVFiMWRIYzB5aWdqMURaMHdkN3hvSGRFRzNoSE5DeG9UMUJudkdOR0N1VUdQQT09PC9JbnZlcnNlUT48RD5Ba1QxY1JWNUMvZ04xbHlPcHNDQWxQNEkveVJyWDdKTE0rM0tqOUJEN3MwNzhZTE5yZzhEU3U4RmlHMEFqK2ZqMUpPM0VjckJCQUZvZjJ1c0FjWm9pbmFwZ25LQ0JpVEpHTks1L2w3ajlkZW1YYjlaK2VkQkhUdlMwYUtPaldQYXhYODV6Z0N3c1RoaFpIbWJ3UGZ5MFA4c0p4dTN0YXdPQmdzc1VaRHlnT0Jsb2tKN0t0WVp5WmZTNmRNaVZwM2pTbVA4TnV2Y3hMOHdaSkVXQkxmV0VaMkNCSFRoY1FDRjl3cHFVcXllQzN0L3NNNU9lNnRsUUtMVDNGVGp4S01hcVlpc2xFejNqMm5UTytMTFkrYm1OTUlCMkpOelpqcWJkQU1EYmI4Tis1eWhNc2R1eUk3WkxBQ05IREVjWmpUb3U5OTVPdHR2R2p2NkVoS2RnTUU3WEFCRHlKa2craDh0M01BajVjb3F5cWpIcEpyMlMyaGRjV1I4YWhVQW9UaXJuYVpyVVdjNngyWnllUEFLam40ajl0SmpIdnNqdVRURm5ibU1GOHQwZ1hMYk14bWw0dDVoOGdQdUowTjFJanU3L0UzTDRFNHVHNDducHB1UElkWm04ck1UbURKT3RvVWpyVXUrNTlEUTZvQ2VzMlJ2ZDBqSW12TE4vSmE3d1Q4Sk5kZzVrcWs4VTlwbFMxUStzRlE5SG5OQktDLzFMa1VuVWh6bDlWUHF4NjFZM01qQXBvc1N2d0ZaSVZJd2xYY1NIUDFRZ2hVMW4rMllNQjZKOC8wWnBPeGtSdjZjb3BrQjBzSHlNMCtwQ20zZ0xDaHhxVVYvM3hvaFY1UGNKSXd3aHNhdEMrS2ptUC9zRlBRMlBjS1l4aEplc1BaL1NjYkVudVVWbll1WW1xYz08L0Q+PC9SU0FLZXlWYWx1ZT4=";
    }

    [Serializable]
    public class EncryptorRSAKeys
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }

    public static class EncryptorRSA
    {
        private static bool _optimalAsymmetricEncryptionPadding = false;

        public static EncryptorRSAKeys GenerateKeys(int keySize)
        {
            if (keySize % 2 != 0 || keySize < 512)
                throw new Exception("Key should be multiple of two and greater than 512.");

            var response = new EncryptorRSAKeys();

            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                var publicKey = provider.ToXmlString(false);
                var privateKey = provider.ToXmlString(true);

                var publicKeyWithSize = IncludeKeyInEncryptionString(publicKey, keySize);
                var privateKeyWithSize = IncludeKeyInEncryptionString(privateKey, keySize);

                response.PublicKey = publicKeyWithSize;
                response.PrivateKey = privateKeyWithSize;
            }

            return response;
        }

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
            int maxLength = GetMaxDataLength(keySize);
            if (data.Length > maxLength) throw new ArgumentException(String.Format("Maximum data length is {0}", maxLength), "data");
            if (!IsKeySizeValid(keySize)) throw new ArgumentException("Key size is not valid", "keySize");
            if (String.IsNullOrEmpty(publicKeyXml)) throw new ArgumentException("Key is null or empty", "publicKeyXml");

            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                provider.FromXmlString(publicKeyXml);
                return provider.Encrypt(data, _optimalAsymmetricEncryptionPadding);
            }
        }

        public static string DecryptText(string text, string privateKey)
        {
            int keySize = 0;
            string publicAndPrivateKeyXml = "";

            GetKeyFromEncryptionString(privateKey, out keySize, out publicAndPrivateKeyXml);

            var decrypted = Decrypt(Convert.FromBase64String(text), keySize, publicAndPrivateKeyXml);
            return Encoding.UTF8.GetString(decrypted);
        }

        private static byte[] Decrypt(byte[] data, int keySize, string publicAndPrivateKeyXml)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Data are empty", "data");
            if (!IsKeySizeValid(keySize)) throw new ArgumentException("Key size is not valid", "keySize");
            if (String.IsNullOrEmpty(publicAndPrivateKeyXml)) throw new ArgumentException("Key is null or empty", "publicAndPrivateKeyXml");

            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                provider.FromXmlString(publicAndPrivateKeyXml);
                return provider.Decrypt(data, _optimalAsymmetricEncryptionPadding);
            }
        }

        public static int GetMaxDataLength(int keySize)
        {
            if (_optimalAsymmetricEncryptionPadding)
            {
                return ((keySize - 384) / 8) + 7;
            }
            return ((keySize - 384) / 8) + 37;
        }

        public static bool IsKeySizeValid(int keySize)
        {
            return keySize >= 384 &&
                    keySize <= 16384 &&
                    keySize % 8 == 0;
        }

        private static string IncludeKeyInEncryptionString(string publicKey, int keySize)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(keySize.ToString() + "!" + publicKey));
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
