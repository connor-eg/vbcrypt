using vbcrypt;
using System.Security.Cryptography;
using System.Text;

internal class Program
{
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

        // Extracting arguments from the ArgumentParser.
        string phrase = parsedArgs["key"].GetValue();
        try {                                       // Handling Unicode normalization. If this does not work, the program can proceed without.
            string normalized = phrase.Normalize(); // The idea is to try to prevent issues when multiple callers use the same args, with different encodings.
            phrase = normalized;                    // I do not have a good way to generate a lot of different encodings so the only testing I'll do
        } catch (Exception){}                       //  is to ensure that this change doesn't break anything.
        string runMode = parsedArgs["mode"].GetValue();
        string[] fileNames = parsedArgs["files"].GetValues();

        // This looks so much cleaner than before :)
        // Dependency injection, the using block, it's all so much better.
        Aes aes = Aes.Create();
        aes.KeySize = 256;
        using (CryptHandler cryptHandler = new(aes, SHA256.Create()))
        {
            // This makes assumptions about string encoding (bad), but I don't think I can do it better at this time.
            cryptHandler.HashAndSetKey(Encoding.UTF8.GetBytes(phrase));

            if (runMode == "e") cryptHandler.Encrypt(fileNames, false);
            if (runMode == "ex") cryptHandler.Encrypt(fileNames, true);
            if (runMode == "d") cryptHandler.Decrypt(fileNames, false);
            if (runMode == "dx") cryptHandler.Decrypt(fileNames, true);
        }

        Console.WriteLine("Done.");
    }
}