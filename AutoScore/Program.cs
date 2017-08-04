using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequestGenerator.Encrypt;

namespace AutoScore
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args[0] == "Gen")
                {
                    GenerateKeys();
                }
                else if (args[0] == "Test")
                {
                    AutoTest(args[1]);
                }
            }
            catch (Exception) {
                Console.WriteLine("Usage: ~ Gen/Test");
            }
        }

        public static void GenerateKeys()
        {
            int keySize = 4096;
            var keys = EncryptorRSA.GenerateKeys(keySize);
            StreamWriter publicKey = new StreamWriter("publickey");
            publicKey.Write(keys.PublicKey);
            StreamWriter privateKey = new StreamWriter("privatekey");
            privateKey.Write(keys.PrivateKey);
            publicKey.Close();
            privateKey.Close();
        }

        public static void AutoTest(string file)
        {
            StreamReader reader = new StreamReader(file);
            var content = reader.ReadToEnd();
            var origin = EncryptorRSA.DecryptText(content, GenratedKeys.PrivateKey);
            Console.WriteLine(origin);
        }
    }
}
