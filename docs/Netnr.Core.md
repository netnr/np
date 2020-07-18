# Netnr.Core
Public library

### Install from NuGet
```
Install-Package Netnr.Core
```

### [ChangeLog](Netnr.Core.ChangeLog.md)

### Class
- CacheTo.cs　　cache (Core requires dependency injection and assigns `Netnr.Core.memoryCache` object)
- CalcTo.cs　　algorithm, encryption, decryption (MD5, DES, SHA1, HMAC_SHA1)
- CmdTo.cs　　execute command, support Windows, Linux
- ConsoleTo.cs　　output log, error message
- Extend.cs　　Method extension (depending on the conversion of `Newtonsoft.Json`, JSON, entity, encoding, SQL, etc.)
- FileTo.cs　　read and write files
- HttpTo.cs　　HTTP request (GET, POST, etc., you can set `HttpWebRequest` object)
- LamdaTo.cs　　dynamically generates Lamda expressions
- RandomTo.cs　　generates random code (verification code)
- RsaTo.cs　　RSA encryption and decryption and RSA signature and verification
- TreeTo.cs　　Tree common methods (List data set generates JSON tree, menu multi-level navigation)
- UniqueTo.cs　　generates a unique logo (GUID becomes long)

### Frame
- .NET Standard 2.0
- .NET Standard 2.1
- .NET Framework 4.0