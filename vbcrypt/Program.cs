using System.Security.Cryptography;
using System.Text;
using vbcrypt;

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

        // Extracting arguments from the ArgumentParser
        string runMode = parsedArgs["mode"].GetValue();
        string[] fileNames = parsedArgs["files"].GetValues();

        // Getting the password from the terminal in a slightly more secure way
        Console.WriteLine("Enter a password for encryption/decryption, then press enter to use.");
        Console.WriteLine("The password will not be displayed as you type it for security.");
        Console.Write("> ");

        StringBuilder phraseSB = new();
        ConsoleKeyInfo cki = new();
        do
        {
            cki = Console.ReadKey(true);
            if(cki.Key == ConsoleKey.Enter)
            {
                // do nothing
            } else if (cki.Key == ConsoleKey.Backspace && phraseSB.Length > 0) // backspace to remove
            {
                phraseSB.Remove(phraseSB.Length - 1, 1);
            } else
            {
                phraseSB.Append(cki.KeyChar);
            }
        } while (cki.Key != ConsoleKey.Enter);

        // Extracting arguments from the ArgumentParser.
        string phrase = phraseSB.ToString();
        try
        {                                   // Handling Unicode normalization. If this does not work, the program can proceed without.
            phrase = phrase.Normalize();    // The idea is to try to prevent issues when multiple callers use the same args, with different encodings.
        }                                   // I do not have a good way to generate a lot of different encodings so the only testing I'll do
        catch (Exception) { }               //  is to ensure that this change doesn't break anything.



        // This looks so much cleaner than before :)
        // Dependency injection, the using block, it's all so much better.
        Aes aes = Aes.Create();
        aes.KeySize = 256;
        using (CryptHandler cryptHandler = new(aes, SHA256.Create()))
        {
            // This makes assumptions about string encoding (bad), but I don't think I can do it better at this time.
            // As much as I would like to just take the raw bytes out of the string, that is not an option here.
            cryptHandler.HashAndSetKey(Encoding.UTF8.GetBytes(phrase));

            if (runMode == "e") cryptHandler.Encrypt(fileNames, false);
            if (runMode == "ex") cryptHandler.Encrypt(fileNames, true);
            if (runMode == "d") cryptHandler.Decrypt(fileNames, false);
            if (runMode == "dx") cryptHandler.Decrypt(fileNames, true);
        }

        Console.WriteLine("Done.");
    }
}