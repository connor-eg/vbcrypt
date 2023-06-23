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
        public static Dictionary<string, Argument>? Parse(string[] input)
        {
            Dictionary<string, Argument> map = new Dictionary<string, Argument>();
            if (input.Length < 3)
            {
                Console.WriteLine("""
                    Program usage:
                      vbcrypt <phrase> <run mode> <files>
                      Arguments:
                        phrase                      Determines what key is used to encrypt/decrypt files. This is effectively a password lock, but
                                                      once I get this program working I do intend to allow a key file to be used as well.
                                                    Note that if your key phrase contains special characters (spaces etc.) you will have to escape
                                                      those in your terminal.
                        Run modes:
                          e                         This program run will encrypt files.
                          d                         This program run will decrypt files.
                        files (<file[,file,...]>)   The file/files to encrypt/decrypt. Requires at least one argument.
                                                    Directories are not valid files (consider compressing/zipping the directory and encrypting that).
                    """);
                return null;
            }

            // First argument specifies the key we are using.
            map.Add("key", new Argument().Add(input[0]));

            // Second argument specifies whether we are doing encryption or decryption.
            String[] validList = { "e", "d" }; // Cool and scalable to more options (like I'm going to implement more options LOL)
            if (validList.Contains(input[1]))
            {
                map.Add("mode", new Argument().Add(input[1]));
            }
            else
            {
                Console.WriteLine($"Mode '{input[1]}' is invalid.");
                return null;
            }

            // The remaining arguments all point to files.
            Argument filearg = new Argument();
            for(int i = 2; i < input.Length; i++) filearg.Add(input[i]);
            // "clean code" is a myth perpetuated by enterprise coders who think they're better than you.

            map.Add("files", filearg);

            return map;
        }
    }
}
