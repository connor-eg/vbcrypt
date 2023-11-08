using System.Security.Cryptography;
using System.Text;
using vbcrypt;

internal class Program
{
    private enum PasswordHandlingMode { DEFAULT, NO_CONFIRM, NO_HIDE }
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
        string[] toEncrypt = parsedArgs["toEncrypt"].GetValues();
        string[] toDecrypt = parsedArgs["toDecrypt"].GetValues();
        bool deleteOldFiles = parsedArgs.ContainsKey("delete");
        bool obfuscateNames = parsedArgs.ContainsKey("obfuscate");
        PasswordHandlingMode passwordHandling = PasswordHandlingMode.DEFAULT;
        if (parsedArgs.ContainsKey("pwNoConfirm")) passwordHandling = PasswordHandlingMode.NO_CONFIRM;
        if (parsedArgs.ContainsKey("pwNoHide")) passwordHandling = PasswordHandlingMode.NO_HIDE; // Fall-through logic to allow pwNoHide to override pwNoConfirm
        string password;

        // Getting the password from the terminal based on user-specified options
        Console.WriteLine("Please enter a password for encryption/decryption.");
        if(passwordHandling == PasswordHandlingMode.NO_HIDE)
        {
            Console.Write("> ");
            password = GetInputFromTerminal(false);
        } else
        {
            Console.WriteLine("The password will not be displayed as you type it for security.");
            Console.Write("> ");
            password = GetInputFromTerminal(true);
            Console.WriteLine();
            if(passwordHandling == PasswordHandlingMode.DEFAULT)
            {
                Console.WriteLine("Please retype the password you want to use.");
                Console.Write("> ");
                string confirmedPw = GetInputFromTerminal(true);
                if(password != confirmedPw)
                {
                    Console.WriteLine("Those passwords do not match. Please rerun this program.");
                    return;
                }
            }
        }

        Aes aes = Aes.Create();
        aes.KeySize = 256;
        using (CryptHandler cryptHandler = new(aes, SHA256.Create()))
        {
            // Choosing UTF8 here to slightly vary the length of a key if there are non-ASCII characters.
            cryptHandler.HashAndSetKey(Encoding.UTF8.GetBytes(password));
            cryptHandler.Encrypt(toEncrypt, deleteOldFiles, obfuscateNames);
            cryptHandler.Decrypt(toDecrypt, deleteOldFiles);
        }

        Console.WriteLine("Done.");
    }

    private static string GetInputFromTerminal(bool hideInput)
    {
        string input;
        if (hideInput)
        {
            StringBuilder phraseSB = new();
            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Enter)
                {
                    // do nothing
                }
                else if (cki.Key == ConsoleKey.Backspace && phraseSB.Length > 0) // backspace to remove
                {
                    phraseSB.Remove(phraseSB.Length - 1, 1);
                }
                else
                {
                    phraseSB.Append(cki.KeyChar);
                }
            } while (cki.Key != ConsoleKey.Enter);
            input = phraseSB.ToString();
        } else
        {
            input = Console.ReadLine() ?? "";
        }
        // Handling Unicode normalization. If this does not work for some reason (unprintable characters?), the program will proceed without.
        // The idea is to try to prevent issues when multiple callers use the same password, with different encodings at the terminal.
        try { input = input.Normalize(); } 
        catch (Exception) { }
        return input;
    }
}