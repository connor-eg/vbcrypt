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
                      vbcrypt <phrase> <run mode> <files>
                      Arguments:
                        phrase      Determines what key is used to encrypt/decrypt files. This is effectively a password lock.

                        Run modes:
                          e         This program run will encrypt files.
                          d         This program run will decrypt files.
                          ex        This program run will encrypt files and delete the original file when done.
                          dx        This program run will decrypt files and delete the encrypted file when done.

                        files (<file[,file,...]>)   The file/files to encrypt/decrypt. Requires at least one argument.
                                                    Directories are not valid files. If you need to encrypt a directory,
                                                      consider compressing it into a single file first.
                    """);
            }

            // First argument specifies the key we are using.
            map.Add("key", new Argument().Add(input[0]));

            // Second argument specifies whether we are doing encryption or decryption.
            String[] validList = { "e", "d", "ex", "dx" }; // Cool and scalable to more run modes.
            if (validList.Contains(input[1]))
            {
                map.Add("mode", new Argument().Add(input[1]));
            }
            else
            {
                throw new ParseException($"Run mode '{input[1]}' is invalid. Run with no arguments to see all valid run modes.");
            }

            // The remaining arguments all point to files.
            Argument filearg = new();
            for (int i = 2; i < input.Length; i++) filearg.Add(input[i]);
            map.Add("files", filearg);

            return map;
        }
    }
}
