# vbcrypt
I'm bored and making my own file encryptor which uses AES.

# DISCLAIMER
Don't use this software for anything serious. It could be insecure and/or provide you with files which you have no actual way of recovering.
It's something I'm making for myself because I want to learn a bit of C#.

If you do use this and find yourself unable to recover your encrypted files later, that's not on me; you're the person who decided to use a random hobbyist's work.
If you need a tool to encrypt your files, consider using literally anyone else's file encryptor first. Seriously, this doesn't even have unit tests.

That said, it does look like it works so far. Check the releases panel for executables.

# Features
- Encrypts files
- Decrypts files
- Can hide and store file names inside of the encrypted files

# Planned features (viability of features and my willingness to implement them TBD)
- Allow the encryption of whole directories by automatically compressing them into a .zip file and encrypting that.
- Build a whole GUI for this in .NET MAUI (in a new repository)

# How to use
Download the loose files in the releases (the non-zipped stuff), place them in a directory together, and invoke `vbcrypt.exe` with your favorite command line.
If it launches, the program will guide you through proper program usage afterwards. I don't have more specific information because I keep changing how the argument parser works
 with each new release to try and make it suck less. Yet another good reason to not use this tool.
