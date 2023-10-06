using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.WriteLine("\r\n\r\n\r\n1███████11█████  █    ██   ██████      ▄████▄   ██▀███ 1██   ██1 ██████  ▄▄▄█████1\r\n0 0 1 ▄▀01█   ▀  ██  1██00██    0     1██▀ ▀█ 11██ 0 ██01██  ██00██0  ██01  ██1 10\r\n1 1 ▄▀01 1███   0██  0██01 ███▄       1██    ▄ 1██ 1▄█ 0 1██ ██01██1 ██011 1██1 01\r\n  ▄▀1   011█  ▄ 11█  1██1  1   ██    10██▄ ▄██00██▀▀█▄   0 ▐██100██▄█01 10 1██0 0 \r\n0███████001████011█████1 1██████0    11 ████▀ 01██0 0██0 1 ██1011██0 0  0  0██1 1 \r\n111 0101101 11 10101 1 0 1 011 1     11 01 1  11 11 1011  ██011 0010 1  1  1 01   \r\n011 0 1 1 0 1  11001 1 0 1 10  0     1  0  1     00 1 101██ 100 11 1         1    \r\n1 0 1 1 0   1    101 0 0 0  1  1      1          11   1 0 1 10  01         0      \r\n  1 0       1  1  01           0      1 0         1     1 0                       \r\n1                                     0                 1 1                      \r\n\r\n");
        Console.WriteLine("\n\n\nHello, welcome to zeus-crypt please select 1 or 2 to get started or 3 to quit");
        Console.WriteLine("\n[ 1 ] Encrypt\n[ 2 ] Decrypt\n[ 3 ] quit");
        string option1or2 = Console.ReadLine();

        while (true)
        {
            if (option1or2 == "1")
            {
                Encryptiptions();
                break;
            }
            else if (option1or2 == "2")
            {
                Decryptiptions();
                break;
            }
            else if (option1or2 == "3")
            {
                Console.WriteLine("quitting...");
                Thread.Sleep(2000);
                return;
            }
            else
            {
                Console.WriteLine("Invalid option. Please enter 1 or 2.");
                option1or2 = Console.ReadLine();
            }
        }
    }




    static void Encryptiptions()
    {
        Console.WriteLine("Please enter the path of the file you want to encrypt");
        string EncInput = Console.ReadLine().Trim('"'); // Remove leading and trailing quotes
        Console.WriteLine("Please enter the output path of the file");
        string EncOutput = Console.ReadLine().Trim('"'); // Remove leading and trailing quotes
        Console.WriteLine("Please enter the password for encryption");
        string EncPassword = Console.ReadLine();

        // Ensure the key size is appropriate for AES (128, 192, or 256 bits)
        byte[] enckeyBytes = AdjustKeySize(Encoding.UTF8.GetBytes(EncPassword), 256 / 8);

        Console.WriteLine("Encrypting file...");
        EncryptFile(EncInput, EncOutput, enckeyBytes);
        Console.WriteLine("Encryption complete.");
    }




    static void Decryptiptions()
    {
        Console.WriteLine("Please enter the path of the file you want to decrypt");
        string DncInput = Console.ReadLine().Trim('"'); // Remove leading and trailing quotes
        Console.WriteLine("Please enter the output path of the file");
        string DncOutput = Console.ReadLine().Trim('"'); // Remove leading and trailing quotes
        Console.WriteLine("Please enter the password for decryption");
        string DncPassword = Console.ReadLine();

        // Ensure the key size is appropriate for AES (128, 192, or 256 bits)
        byte[] dnckeyBytes = AdjustKeySize(Encoding.UTF8.GetBytes(DncPassword), 256 / 8);

        Console.WriteLine("Decrypting file...");
        DecryptFile(DncInput, DncOutput, dnckeyBytes);
        Console.WriteLine("Decryption complete.");
    }




    static byte[] AdjustKeySize(byte[] key, int desiredSize)
    {

        byte[] adjustedKey = new byte[desiredSize];
        Array.Copy(key, adjustedKey, Math.Min(key.Length, adjustedKey.Length));
        return adjustedKey;
    }




    static void EncryptFile(string inputFile, string outputFile, byte[] key)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.GenerateIV();

            using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
            using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
            using (ICryptoTransform encryptor = aesAlg.CreateEncryptor())
            using (CryptoStream cryptoStream = new CryptoStream(fsOutput, encryptor, CryptoStreamMode.Write))
            {
                
                fsOutput.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                byte[] buffer = new byte[4096];
                int bytesRead;

                while ((bytesRead = fsInput.Read(buffer, 0, buffer.Length)) > 0)
                    cryptoStream.Write(buffer, 0, bytesRead);
            }
        }
        File.Delete(inputFile);
        Console.WriteLine("Ecrypton done");
        Console.WriteLine("going back to main menu...\n\n");
        Thread.Sleep(3000);
        Main();


    }




    static void DecryptFile(string inputFile, string outputFile, byte[] key)
    {
        try
        {
            using (Aes aesAlg = Aes.Create())
            {
                using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
                {
                    byte[] iv = new byte[16];
                    fsInput.Read(iv, 0, iv.Length);

                    aesAlg.Key = key;
                    aesAlg.IV = iv;

                    using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
                    using (ICryptoTransform decryptor = aesAlg.CreateDecryptor())
                    using (CryptoStream cryptoStream = new CryptoStream(fsInput, decryptor, CryptoStreamMode.Read))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;

                        while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                            fsOutput.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }
        catch (Exception error)
        {
            Console.WriteLine(error);
            if (error.ToString() == "System.Security.Cryptography.CryptographicException: Padding is invalid and cannot be removed.\r\n   at System.Security.Cryptography.CapiSymmetricAlgorithm.DepadBlock(Byte[] block, Int32 offset, Int32 count)\r\n   at System.Security.Cryptography.CapiSymmetricAlgorithm.TransformFinalBlock(Byte[] inputBuffer, Int32 inputOffset, Int32 inputCount)\r\n   at System.Security.Cryptography.CryptoStream.FlushFinalBlock()\r\n   at System.Security.Cryptography.CryptoStream.Dispose(Boolean disposing)\r\n   at System.IO.Stream.Close()\r\n   at System.IO.Stream.Dispose()\r\n   at Program.DecryptFile(String inputFile, String outputFile, Byte[] key) in C:\\Users\\jaysc\\source\\repos\\EncryptFile\\EncryptFile\\Program.cs:line 152")
            {
                File.Delete(outputFile);
                Console.WriteLine("\n\nincorrect passoword\ntry again?\n[ 1 ] yes\n[ 2 ] no");
                string IncorrectPassInput = Console.ReadLine();
                while (true)
                {
                    if (IncorrectPassInput == "1")
                    {
                        Console.WriteLine("Please enter the password for decryption");
                        string DncPasswordwrongpass = Console.ReadLine();
                        byte[] dnckeyBytes = AdjustKeySize(Encoding.UTF8.GetBytes(DncPasswordwrongpass), 256 / 8);
                        DecryptFile(inputFile, outputFile, dnckeyBytes);
                    }
                    else if (IncorrectPassInput == "2")
                    {
                        Console.WriteLine("going back to main menu\n\n");


                        Main();
                    }
                    else
                    {
                        Console.WriteLine("invaild input");
                    }
                }

            }
            else
            {
                Console.WriteLine("the encrypton failed\noutput:\n\n" + error);
            }
        }
        
        File.Delete(inputFile);
        Console.WriteLine("Decrypton done");
        Console.WriteLine("going back to main menu...\n\n");
        Thread.Sleep(3000);
        Main();
    }

}
