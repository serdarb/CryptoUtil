# CryptoUtil
A basic obfuscation-like crypto util for Turkish text content.

This util is basically getting different byte value for all chars in the content by adding a number to it.

To decrypt you need to know that byte randomization number.

```
var bytes = Encoding.UTF8.GetBytes(plainText);
var encryptedBytes = new byte[bytes.Length];
for (int i = 0; i < bytes.Length; i++)
{
    encryptedBytes[i] = (byte)(bytes[i] + _obfuscateNumber);
}
```

This sample project also has builtin testing facility for unit testing.
It is a trial for running tests, but lacks coverage calculation ability.


![Test Run Screenshot](/test-run-screenshot.png?raw=true "Test Run Screenshot")
