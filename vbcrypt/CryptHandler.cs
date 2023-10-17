using System.Security.Cryptography;

internal class CryptHandler : IDisposable
{
    private SymmetricAlgorithm CryptAlgorithmInstance;
    private HashAlgorithm HashAlgorithmInstance;

    public CryptHandler(SymmetricAlgorithm CryptAlgorithmInstance, HashAlgorithm HashAlgorithmInstance, int keySize = 256)
    {
        this.CryptAlgorithmInstance = CryptAlgorithmInstance;
        this.CryptAlgorithmInstance.KeySize = keySize;

        this.HashAlgorithmInstance = HashAlgorithmInstance;
    }

    // F a n c y . Allows a "using" block to manage instances of this class.
    public void Dispose()
    {
        CryptAlgorithmInstance.Clear();
    }

    public void HashAndSetKey(byte[] bytes)
    {
        CryptAlgorithmInstance.Key = HashAlgorithmInstance.ComputeHash(bytes);
    }

    public void Decrypt(string[] files, bool deleteOnFinish)
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
                    byte[] recoveredIV = new byte[16];
                    if (inStream.Read(recoveredIV, 0, 16) < 16) throw new EndOfStreamException("failed. File is too small to contain any encrypted data.");
                    CryptAlgorithmInstance.IV = recoveredIV;
                    using FileStream outStream = File.OpenWrite(outFileName);
                    using CryptoStream cStream = new(inStream, CryptAlgorithmInstance.CreateDecryptor(), CryptoStreamMode.Read);
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

    public void Encrypt(string[] files, bool deleteOnFinish)
    {
        foreach (string file in files)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine($"Cannot find file {file}");
                continue;
            }

            Console.Write($"Processing {file}... ");

            CryptAlgorithmInstance.GenerateIV();

            try
            {
                using (FileStream inStream = File.OpenRead(file))
                {
                    using FileStream outStream = File.OpenWrite($"{file}.vbcrypt");
                    using CryptoStream cStream = new(outStream, CryptAlgorithmInstance.CreateEncryptor(), CryptoStreamMode.Write);
                    outStream.Write(CryptAlgorithmInstance.IV, 0, 16);
                    inStream.CopyTo(cStream);
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

}