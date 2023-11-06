using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

internal class CryptHandler : IDisposable
{
    private SymmetricAlgorithm CryptAlgorithmInstance;
    private HashAlgorithm HashAlgorithmInstance;

    private static Random StringGeneratorRandom = new();

    public CryptHandler(SymmetricAlgorithm CryptAlgorithmInstance, HashAlgorithm HashAlgorithmInstance)
    {
        this.CryptAlgorithmInstance = CryptAlgorithmInstance;
        this.HashAlgorithmInstance = HashAlgorithmInstance;
    }

    // F a n c y . Allows a "using" block to manage instances of this class.
    public void Dispose()
    {
        CryptAlgorithmInstance.Clear();
        HashAlgorithmInstance.Clear();
    }

    public void HashAndSetKey(byte[] bytes)
    {
        CryptAlgorithmInstance.Key = HashAlgorithmInstance.ComputeHash(bytes);
    }

    public void Decrypt(string[] files, bool deleteOnFinish = false)
    {
        foreach (string file in files)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine($"Cannot find/access file {file}");
                continue;
            }

            Console.Write($"Processing {file}... ");

            bool terseName = file.EndsWith(".vbcrypt");
            string outFileName = terseName ? $"{file[..^8]}" : $"{file}.decrypted";
            try
            {
                using (FileStream inStream = File.OpenRead(file))
                {
                    // Recover the IV, which was written in plaintext to the first 16 bytes of the file.
                    byte[] recoveredIV = new byte[16];
                    if (inStream.Read(recoveredIV, 0, 16) < 16) throw new EndOfStreamException("File is too small to contain any encrypted data.");
                    CryptAlgorithmInstance.IV = recoveredIV;

                    //Set up the streams
                    using FileStream outStream = File.OpenWrite(outFileName);
                    using CryptoStream cStream = new(inStream, CryptAlgorithmInstance.CreateDecryptor(), CryptoStreamMode.Read);

                    //Attempt to recover the eight null-bytes that serve as a password check (quick sanity check)
                    byte[] checksumBytes = new byte[8];
                    if(cStream.Read(checksumBytes, 0, 8) < 8) throw new EndOfStreamException("File is too small to contain any encrypted data.");
                    for(int i = 0; i < 8; i++)
                    {
                        if (checksumBytes[i] != 0)
                        {
                            throw new UnauthorizedAccessException("The password provided was incorrect.");
                        }
                    }

                    //Get the rest of the data from the file from this point.
                    cStream.CopyTo(outStream);
                    outStream.Flush();
                    cStream.Clear();
                }
                Console.WriteLine(terseName ? "done." : "done. Remember to remove '.decrypted' from the resulting file.");
                if (deleteOnFinish) File.Delete(file);
            }
            catch (Exception e)
            {
                Console.Write("failed. Reason: ");
                Console.WriteLine(e.Message);
                File.Delete(outFileName);
            }
        }
    }

    public void Encrypt(string[] files, bool deleteOnFinish = false, bool obfuscateNames = false)
    {
        Span<byte> zeroFill = stackalloc byte[8];
        zeroFill.Fill(0b00000000);
        foreach (string file in files)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine($"Cannot find file {file}");
                continue;
            }   

            Console.Write($"Processing {file}... ");

            //CryptAlgorithmInstance.GenerateIV();
            //var outFileName = $"{GenerateRandomString(12)}.vbcr";
            //Console.WriteLine(outFileName);

            try
            {
                using (FileStream inStream = File.OpenRead(file))
                {
                    using FileStream outStream = File.OpenWrite($"{file}.vbcrypt");
                    using CryptoStream cStream = new(outStream, CryptAlgorithmInstance.CreateEncryptor(), CryptoStreamMode.Write);
                    outStream.Write(CryptAlgorithmInstance.IV, 0, 16); // Write the IV to the beginning of the file for later decryption
                    // Write 8 encrypted zero bytes to the file to serve as a checksum.
                    // That is to say, the bytes will become zero again when extracting them from the encrypted file.
                    // If the password used for decryption is different than the one used for encryption, we would extract nonzero bytes.
                    cStream.Write(zeroFill.ToArray());
                    inStream.CopyTo(cStream); // Now we write the contents of the original file to the output stream
                    cStream.FlushFinalBlock();
                    cStream.Clear();
                }
                Console.WriteLine("done.");
                if (deleteOnFinish) File.Delete(file);

            }
            catch (Exception e)
            {
                Console.Write("failed. Reason: ");
                Console.WriteLine(e.Message);
                File.Delete($"{file}.vbcrypt");
            }
        }
    }

    //Helper method to do... exactly what it looks like it does.
    private static string GenerateRandomString(int size = 16)
    {
        const string characters = "QWERTYUIOPASDFGHJKLZXCVBNM1234567890qwertyuiopasdfghjklzxcvbnm";
        StringBuilder sb = new();
        for(int i = 0; i < 16; i++)
        {
            sb.Append(characters[StringGeneratorRandom.Next(characters.Length)]);
        }
        return sb.ToString();
    }
}