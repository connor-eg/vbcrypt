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
        
        /**
         *  The process
         *   1. Generate an AES key from the key phrase.
         *   2. Use that key to encrypt or decrypt all of the files which the user has specified.
         *   3. There is no step 3.
         */

        // Extracting arguments from the ArgumentParser.
        string phrase = parsedArgs["key"].GetValue();
        try {                                       // Handling Unicode normalization. If this does not work, the program can proceed without.
            string normalized = phrase.Normalize(); // The idea is to try to prevent issues when multiple callers use the same args, with different encodings.
            phrase = normalized;                    // I do not have a good way to generate a lot of different encodings so the only testing I'll do
        } catch (Exception){}                       //  is to ensure that this change doesn't break anything.
        string runMode = parsedArgs["mode"].GetValue();
        string[] fileNames = parsedArgs["files"].GetValues();

        // This looks so much cleaner than before :)
        // Just look at that using block go!
        using (CryptHandler cryptHandler = new(Aes.Create(), SHA256.Create(), 256))
        {
            // This makes assumptions (bad), but it's the best I know how to do at this moment.
            // It will work for ASCII strings and UTF8 though, so that should be good enough.
            cryptHandler.HashAndSetKey(Encoding.UTF8.GetBytes(phrase));

            if (runMode == "e") cryptHandler.Encrypt(fileNames, false);
            if (runMode == "ex") cryptHandler.Encrypt(fileNames, true);
            if (runMode == "d") cryptHandler.Decrypt(fileNames, false);
            if (runMode == "dx") cryptHandler.Decrypt(fileNames, true);
        }

        Console.WriteLine("Done.");
    }
}