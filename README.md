# vbcrypt
I'm bored and making my own file encryptor, using AES.

# DISCLAIMER
Don't use this software for anything serious. It's probably insecure and/or will provide you with files which you have no actual way of recovering.
It's something I'm making for myself because I am bored and want to lean a bit of C#.

If you do use this and find yourself unable to recover your encrypted files later, that's not on me; you're the person who decided to use a random hobbyist's work.
If you do want to actually encrypt your files, consider looking up anyone else's file encryptor -- it's probably better.

# Known issues
### (as if anyone else is reading this lmao)
This currently reads entire files to memory before processing them. While that is fast for small files, it means that the largest file you could encrypt is limited by the .NET runtime, the maximum size of an array in C#, and your computer's RAM. The current plan is to fix this by making it use a file stream instead, which s h o u l d make it possible to read/encrypt/decrypt files of arbitrary size.
