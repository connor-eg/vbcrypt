using vbcrypt;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Intrinsics.Arm;
using System.Buffers.Text;
using System.Reflection.Emit;

internal class Program
{
    private static SHA256 sha = SHA256.Create();
    private static System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create();
    private static void Main(string[] args)
    {
        Dictionary<string, Argument>? parsedArgs = ArgumentParser.Parse(args);
        if(parsedArgs == null)
        {
            Console.WriteLine("An incorrect number of arguments was passed in or an argument was invalid.");
            Console.WriteLine("Run this program with no arguments to see proper usage..");
            return;
        }

        // If we're here, we should now have a valid map of arguments and can actually start the program part of the program.

        /**
         *  The process
         *   1. Generate an AES key from the key phrase.
         *   2. Use that key to encrypt or decrypt all of the files which the user has specified.
         *   3. There is no step 3.
         */

        // Step 1 - SECURE THE KEYS
        string phrase = parsedArgs["key"].GetValue();
        // Now that we're through that reference, let's get the rest of the arguments ready for use.
        string runMode = parsedArgs["mode"].GetValue();
        string[] fileNames = parsedArgs["files"].GetValues();

        // Generate an AES key (256 bits because I'm cool btw) from the key phrase/file
        // No need to run the hash more than once because the key can't be obtained from the finished files anyways. The hash just normalizes the input key to a fixed size.
        aes.KeySize = 256;
        aes.Key = sha.ComputeHash(Encoding.ASCII.GetBytes(phrase));

        // Now it's time to do the thing with the stuff.
        if (runMode == "e") Encrypt(fileNames);
        if (runMode == "d") Decrypt(fileNames);

        aes.Clear();

        Console.WriteLine("Done.");
        
    }
    
    private static void Encrypt(string[] files)
    {
        foreach(string file in files)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine($"Cannot find file {file}");
                continue;
            }

            Console.Write($"Processing {file}... ");

            aes.GenerateIV();

            using (FileStream inStream = File.OpenRead(file))
            {
                using FileStream outStream = File.OpenWrite($"{file}.vbcrypt");
                using CryptoStream cStream = new CryptoStream(outStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
                outStream.Write(aes.IV, 0, 16);
                outStream.Flush();
                inStream.CopyTo(cStream);
                cStream.FlushFinalBlock();
                cStream.Clear();
            }
            Console.WriteLine("done.");
        }
    }

    private static void Decrypt(string[] files)
    {
        foreach (string file in files)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine($"Cannot find file {file}");
                continue;
            }

            Console.Write($"Processing {file}... ");
            string outFileName = file.EndsWith(".vbcrypt") ? $"{file[..^8]}" : $"{file}.decrypted";
            if (outFileName.EndsWith(".decrypted")) Console.Write("remember to remove '.decrypted' from the resulting file... ");

            using (FileStream inStream = File.OpenRead(file))
            {
                if(inStream.Read(aes.IV, 0, 16) < 16) throw new EndOfStreamException("File is too small to contain an AES IV.");
                using FileStream outStream = File.OpenWrite(outFileName);
                using CryptoStream cStream = new CryptoStream(inStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
                cStream.CopyTo(outStream);
                outStream.Flush();
                cStream.Clear();
            }
            Console.WriteLine("done.");
        }
    }
}