using System.Runtime.CompilerServices;

namespace vbcrypt
{
    internal class ArgumentParser
    {
        private enum ExpectingState { FLAG, ENCRYPT_FILE, DECRYPT_FILE }
        private const string HelpText = """
            Program usage:
                vbcrypt [options...]
                Arguments:
                Options:
                    -?, --help        Display this help text and do no further processing.
                    -e file, --encrypt file     Encrypt the specified file (can be used multiple times)
                    -d file, --decrypt file     Decrypt the specified file (can be used multiple times)

                Other run-modifying options:
                    -x, --delete      Delete the original files after each operation
                    -o, --obfuscate   Encrypt the original files' names (meaningful for encryption only)
                    --pw-no-confirm   Skip confirming your encryption/decryption password (not generally recommended)
                    -n, --pw-no-hide  Do not hide your password on input (also disables confirmation)

                Example (encrypting cat.png and decrypting secret.txt.vbcr with delete and obfuscate mode on):
                    vbcrypt -x --obfuscate -e cat.png -d secret.txt.vbcr
            """;

        public static Dictionary<string, Argument> Parse(string[] args)
        {
            Dictionary<string, Argument> map = new();
            if (args.Length == 0)
            {
                throw new ParseException(HelpText);
            }

            ExpectingState state = ExpectingState.FLAG; // What we are expecting to see next

            // Start reading arguments
            Argument encryptArg = new();
            Argument decryptArg = new();

            foreach(var arg in args)
            {
                switch(state)
                {
                    case ExpectingState.FLAG:
                        switch (arg)
                        {
                            case "-?":
                            case "--help":
                                throw new ParseException(HelpText);
                            case "-e":
                            case "--encrypt":
                                state = ExpectingState.ENCRYPT_FILE;
                                break;
                            case "-d":
                            case "--decrypt":
                                state = ExpectingState.DECRYPT_FILE;
                                break;
                            case "-x":
                            case "--delete":
                                map.TryAdd("delete", new Argument());
                                break;
                            case "-o":
                            case "--obfuscate":
                                map.TryAdd("obfuscate", new Argument());
                                break;
                            case "--pw-no-confirm":
                                map.TryAdd("pwNoConfirm", new Argument());
                                break;
                            case "-n":
                            case "--pw-no-hide":
                                map.TryAdd("pwNoHide", new Argument());
                                break;
                            default:
                                throw new ParseException($"The option {arg} is not valid. Run with --help to see all valid options.");
                        }
                        break;
                    case ExpectingState.ENCRYPT_FILE:
                        encryptArg.Add(arg);
                        state = ExpectingState.FLAG;
                        break;
                    case ExpectingState.DECRYPT_FILE:
                        decryptArg.Add(arg);
                        state = ExpectingState.FLAG;
                        break;
                    default:
                        throw new ParseException("THIS TEXT SHOULD NEVER APPEAR.");
                }
            }

            map.Add("toEncrypt", encryptArg);
            map.Add("toDecrypt", decryptArg);

            if(state != ExpectingState.FLAG) // The last thing in args was -e or -d.
            {
                throw new ParseException("You have specified -e or -d without a file. Try running with --help");
            }

            if(encryptArg.Count == 0 && decryptArg.Count == 0) // The user never specified -e or -d
            {
                throw new ParseException("This program requires a run mode. Try running with --help");
            }

            return map;
        }
    }
}
