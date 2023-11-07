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
                      vbcrypt [options] <run mode> <files>
                      Arguments:
                        Options:
                          -d, --delete      Delete the original files after each operation
                          -o, --obscure     Encrypt the original files' names (meaningful for encryption only)
                        Run modes:
                          -E, --encrypt     This program run will encrypt files.
                          -D, --decrypt     This program run will decrypt files.

                        files (<file[,file,...]>)   The file/files to encrypt/decrypt. Requires at least one argument.
                                                    Directories are not valid files. If you need to encrypt a directory,
                                                      consider compressing it into a single file first.
                    """);
            }

            // Start reading arguments until we hit the run mode (this gathers options)
            int argnbr = 0;
            do
            {
                string arg = input[argnbr++];
                switch (arg)
                {
                    case "-d":
                    case "--delete":
                        map.Add("delete", new Argument()); // Still internally debating on whether this should be nullable for options. It likely doesn't matter.
                        break;
                    case "-o":
                    case "--obscure":
                        map.Add("obscure", new Argument());
                        break;
                    case "-E":
                    case "--encrypt":
                        map.Add("mode", new Argument().Add("e"));
                        break;
                    case "-D":
                    case "--decrypt":
                        map.Add("mode", new Argument().Add("d"));
                        break;
                    default:
                        throw new ParseException($"The option {arg} is not valid. Run with no arguments to see all valid options.");
                }
            } while (argnbr < input.Length && !map.ContainsKey("mode"));

            // Check if the user has actually specified any run mode.
            if (!map.ContainsKey("mode"))
            {
                throw new ParseException("This program requires a run mode. Run with no arguments to see the valid run modes.");
            }

            // The remaining arguments all point to files.
            Argument filearg = new();
            while (argnbr < input.Length)
            {
                filearg.Add(input[argnbr++]);
            }

            if (filearg.Count == 0) throw new ParseException("You must specify at least one file to operate on. Run with no arguments to see a valid program run.");

            map.Add("files", filearg);

            return map;
        }
    }
}
