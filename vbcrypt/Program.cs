using vbcrypt;
using System.Security.Cryptography;
using System.Text;

internal class Program
{
    private static SHA256 sha = SHA256.Create();
    private static Aes aes = Aes.Create();
    private static void Main(string[] args)
    {
        Dictionary<string, Argument> parsedArgs;
        try
        {
            parsedArgs = ArgumentParser.Parse(args); // Tautology is a vibe ngl.
        }
        catch (ParseException e)
        {
            Console.WriteLine(e.Message);
            return;
        }
        
        /**
         *  The process
         *   1. Generate an AES key from the key phrase.
         *   2. Use that key to encrypt or decrypt all of the files which the user has specified.
         *   3. There is no step 3.
         */

        // Extracting arguments from the ArgumentParser.
        string phrase = parsedArgs["key"].GetValue();
        string runMode = parsedArgs["mode"].GetValue();
        string[] fileNames = parsedArgs["files"].GetValues();

        // Generate an AES key (256 bits) from the password.
        // The hash isn't for security, it just normalizes the input key to a fixed size
        aes.KeySize = 256;
        aes.Key = sha.ComputeHash(Encoding.ASCII.GetBytes(phrase));

        // Now it's time to do the thing with the stuff.
        if (runMode == "e") Encrypt(fileNames, false);
        if (runMode == "ex") Encrypt(fileNames, true);
        if (runMode == "d") Decrypt(fileNames, false);
        if (runMode == "dx") Decrypt(fileNames, true);

        aes.Clear();

        Console.WriteLine("Done.");
    }
    
    private static void Encrypt(string[] files, bool deleteOnFinish)
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

            try
            {
                using (FileStream inStream = File.OpenRead(file))
                {
                    using FileStream outStream = File.OpenWrite($"{file}.vbcrypt");
                    using CryptoStream cStream = new(outStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
                    outStream.Write(aes.IV, 0, 16);
                    inStream.CopyTo(cStream);
                    cStream.FlushFinalBlock();
                    cStream.Clear();
                }
                Console.WriteLine("done.");
                if (deleteOnFinish) File.Delete(file);
                
            }
            catch (Exception e)
            {
                Console.Write("failed. Reason: ");
                Console.WriteLine(e.Message);
                File.Delete($"{file}.vbcrypt");
            }
        }
    }

    private static void Decrypt(string[] files, bool deleteOnFinish)
    {
        foreach (string file in files)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine($"Cannot find/access file {file}");
                continue;
            }

            Console.Write($"Processing {file}... ");

            bool terseName = file.EndsWith(".vbcrypt");
            string outFileName = terseName ? $"{file[..^8]}" : $"{file}.decrypted";
            try
            {
                using (FileStream inStream = File.OpenRead(file))
                {
                    byte[] recoveredIV = new byte[16];
                    if (inStream.Read(recoveredIV, 0, 16) < 16) throw new EndOfStreamException("failed. File is too small to contain any encrypted data.");
                    aes.IV = recoveredIV;
                    using FileStream outStream = File.OpenWrite(outFileName);
                    using CryptoStream cStream = new(inStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
                    cStream.CopyTo(outStream);
                    outStream.Flush();
                    cStream.Clear();
                }
                Console.WriteLine(terseName ? "done. Remember to remove '.decrypted' from the resulting file." : "done.");
                if (deleteOnFinish) File.Delete(file);
            }
            catch (Exception e)
            {
                Console.Write("failed. Reason: ");
                Console.WriteLine(e.Message);
                File.Delete(outFileName);
            }
        }
    }
}