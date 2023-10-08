using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vbcrypt
{
    internal class ArgumentParser
    {
        public static Dictionary<string, Argument> Parse(string[] input)
        {
            Dictionary<string, Argument> map = new();
            if (input.Length < 3)
            {
                throw new ParseException("""
                    Program usage:
                      vbcrypt <phrase> <run mode> [options] <files>
                      Arguments:
                        phrase      Determines what key is used to encrypt/decrypt files. This is effectively a password lock, but
                                      once I get this program working I do intend to allow a key file to be used as well.
                                    Note that if your key phrase contains special characters (spaces etc.) you will have to escape
                                      those in your terminal.
                        Run modes:
                          e         This program run will encrypt files.
                          d         This program run will decrypt files.

                        Options:
                          h         Obfuscate resulting files' names and store the original with the encrypted file. Only valid for encryption.
                          del       Delete the old files after encryption/decryption completes. Not recommended at this time.
                          X         Indicates that any further arguments are to be interpreted as files.
                                      Use this if the file you want to operate on would otherwise be interpreted as an option,
                                      or if you intend to call this program programmatically (to eliminiate unintended behavior)

                        files (<file[,file,...]>)   The file/files to encrypt/decrypt. Requires at least one argument.
                                                    Directories are not valid files; consider zipping the directory and encrypting that.
                    """);
            }

            // First argument specifies the key we are using.
            map.Add("key", new Argument().Add(input[0]));

            // Second argument specifies whether we are doing encryption or decryption.
            String[] validList = { "e", "d" }; // Cool and scalable to more run modes.
            if (validList.Contains(input[1]))
            {
                map.Add("mode", new Argument().Add(input[1]));
            }
            else
            {
                throw new ParseException($"Mode '{input[1]}' is invalid.");
            }

            // The remaining arguments all point to options and files.
            Argument options = new();
            Argument filearg = new();

            // Thanks to options handling, this is more complex than it was. Oh well.
            string[] validOptions = { "h", "del", "X" };
            bool interpretAsOption = true; // Whether we should attempt to interpret the argument we're staring at as an option first.
                                           // Once this is false, we always interpret remaining options as files.
            for (int i = 2; i < input.Length; i++)
            {
                string arg = input[i];
                if (interpretAsOption)
                {
                    if (validOptions.Contains(arg))
                    {
                        // The "X" option gets its own special handling. It disables interpretAsOption but is not itself added as a file.
                        if (arg == "X")
                        {
                            interpretAsOption = false;
                        } else
                        {
                            options.Add(arg); // The only nice thing about this is that it means that options can be searched for in the finished map:
                                              // map["options"].GetValues().Contains("del") for instance.
                                              // Holy hell that's bloated now that I'm looking at it from here.
                        }
                    } else // Once we run into the first option that isn't in the options list, we start interpreting everything as files.
                    {
                        interpretAsOption = false;
                        filearg.Add(arg);
                    }
                } else
                {
                    filearg.Add(arg);
                }
            }
            // This decision tree may be bigger than my family tree but whatever.
            // "clean code" is a myth perpetuated by enterprise coders who think they're better than you.

            map.Add("options", options);
            map.Add("files", filearg);

            return map;
        }
    }
}
