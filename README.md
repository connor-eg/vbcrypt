# vbcrypt
I'm bored and making my own file encryptor, using AES.

# DISCLAIMER
Don't use this software for anything serious. It's probably insecure and/or will provide you with files which you have no actual way of recovering.
It's something I'm making for myself because I am bored and want to learn a bit of C#.

If you do use this and find yourself unable to recover your encrypted files later, that's not on me; you're the person who decided to use a random hobbyist's work.
If you do want to actually encrypt your files, consider looking up anyone else's file encryptor -- it's probably better.

# Known issues
### (as if anyone else is reading this lmao)
This garbles the first 16 bytes of output when decrypting files. No idea why. It now uses a stream to read the files tho so that's a vibe.
