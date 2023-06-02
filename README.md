libamiibo
========
amiibo™ parsing library, now with AmiiboApi Support and in .NET 6

Usage
=====
**libamiibo expects a binary dump. It will not work with XMLs or hexadecimal text files**.  See the unix program `xxd` to convert from hexadecimal to binary.

It offers support for encryption/decryption, figurine data, amiibo settings and AppData.

To parse a tag directly from a binary (encrypted) dump, use ```new LibAmiibo().ReadEncryptedTag(..)```.

When using an encrypted binary, the Settings and AppData information will not contain any valid information.

Examples
--------
!! You will need the AmiiboApi database, check notice !!

- Decrypting a NTAG215 dump "mario.bin" with the "retail.bin" keys:
When instantiating `LibAmiibo`, enter the path to the retail.bin.
To set the CDNKeys, call static `CDNUtils.LoadCDNKey(..)` with the keys path.
```
byte[] encryptedNtagData = System.IO.File.ReadAllBytes("mario.bin");
var lib = new LibAmiibo("retail.bin");
var amiiboTag = lib.DecryptTag(encryptedNtagData);
```

- Parsing a NTAG215 dump "mario.bin" only for the non-encrypted information:
```
byte[] encryptedNtagData = System.IO.File.ReadAllBytes("mario.bin");
var amiiboTag = new LibAmiibo().ReadEncryptedTag(encryptedNtagData);
```

Notice
------
This LibAmiibo Modification doesn includes images or Amiibo information data anymore.
To be more update friendly, it now includes a simple implementation to make use of N3evin/AmiiboAPI.
For functionality it is required to download "https://raw.githubusercontent.com/N3evin/AmiiboAPI/master/database/amiibo.json"
and put besides the libammibo.dll into `./AmiiboApi/amiibo.json` or you can call `UpdateLocalAmiiboData()` from LibAmiibo.

To download images, either download them manually from "https://github.com/N3evin/AmiiboAPI/tree/master/images"
and put besides the libammibo.dll into `./AmiiboApi/images/` with the same naming as from the git or you can call `UpdateMissingLocalAmiiboImages()` from LibAmiibo to get all images.

To get single images, call `GetLocalAmiiboImage` from LibAmiibo with the Amiibo Hex Id and if it should be downloaded if its missings.


It is not planned to actively maintain this repository.
The main repository and credits goes to https://github.com/Falco20019/libamiibo

Special Thanks
==============
- Lucas "MacGuffen" Romo for his great work with https://docs.google.com/spreadsheets/d/1WJ4HxS9hkLquq-ATt1Rq9mioH6RDgP3qQrtYVaOdimM
- N3vin and CheatFreak for their great work with https://docs.google.com/spreadsheets/d/19E7pMhKN6x583uB6bWVBeaTMyBPtEAC-Bk59Y6cfgxA
- The people over at 3dbrew.org for there work on http://3dbrew.org/wiki/Amiibo and http://3dbrew.org/wiki/Mii
- socram8888 for his great work on https://github.com/socram8888/amiitool
- John "LouieGeetoo" Pray for his great work with http://www.amiibo.life
- HouseBreaker for the CDN parts I borrowed from https://github.com/HouseBreaker/NintendoCDN-TicketParser
- https://code.google.com/archive/p/3dsexplorer/ for the image processing parts
- http://www.codeproject.com/Articles/31702/NET-Targa-Image-Reader for the TGA processing part
