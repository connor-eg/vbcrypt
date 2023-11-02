namespace vbcrypt
{
    internal class ArgumentParser
    {
        public static Dictionary<string, Argument> Parse(string[] input)
        {
            Dictionary<string, Argument> map = new();
            if (input.Length < 2)
            {
                throw new ParseException("""
                    Program usage:
                      vbcrypt <run mode> <files>
                      Arguments:
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

            // Second argument specifies whether we are doing encryption or decryption.
            String[] validList = { "e", "d", "ex", "dx" }; // Cool and scalable to more run modes.
            if (validList.Contains(input[0]))
            {
                map.Add("mode", new Argument().Add(input[0]));
            }
            else
            {
                throw new ParseException($"Run mode '{input[0]}' is invalid. Run with no arguments to see all valid run modes.");
            }

            // The remaining arguments all point to files.
            Argument filearg = new();
            for (int i = 1; i < input.Length; i++) filearg.Add(input[i]);
            map.Add("files", filearg);

            return map;
        }
    }
}
