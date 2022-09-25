using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CryptoUtil;

// A basic obfusucation-like crypto util
// for turkish text content
public class CryptoUtil
{
    public readonly string NUMBERS = "0123456789";
    public readonly string CHARS = "abcçdefghıijklmnoöprsştuüvyzxwq";
    public readonly string CHARS_UPPER = "ABCÇDEFGHIİJKLMNOÖPRSŞTUÜVYZXWQ";

    public readonly string DataFolderName = "_Data";
    public readonly string ObfuscateOrderPath = $"_Data/_Obfuscate";
    public bool IsObfusucateFileCreated { get; }

    private HashAlgorithm _hashAlgorithm;
    public bool IsHashAlgorithmAssigned
    {
        get { return _hashAlgorithm != null; }
    }

    public readonly char[] _NUMBERS;
    public readonly char[] _CHARS;
    public readonly char[] _CHARS_UPPER;
    public readonly int _obfuscateNumber;

    private void ArrangeObfusucation(int obfuscateNumber)
    {
        var _numbers = NUMBERS.ToCharArray();
        Shuffle(_numbers);
        var lines = new List<string>();
        lines.Add(new string(_numbers));

        var _chars = CHARS.ToCharArray();
        Shuffle(_chars);
        lines.Add(new string(_chars));

        var _charsUpper = CHARS_UPPER.ToCharArray();
        Shuffle(_charsUpper);
        lines.Add(new string(_charsUpper));

        if (obfuscateNumber == 0)
        {
            lines.Add(new Random().Next(10, 99).ToString());
        }
        else
        {
            lines.Add(obfuscateNumber.ToString());
        }

        File.WriteAllLines(ObfuscateOrderPath, lines);
    }

    public CryptoUtil(int obfuscateNumber = 0, string dataFolderName = "")
    {
        if (!string.IsNullOrWhiteSpace(dataFolderName))
        {
            DataFolderName = dataFolderName;
        }

        if (!Directory.Exists(DataFolderName))
        {
            Directory.CreateDirectory(DataFolderName);
        }

        _hashAlgorithm = SHA512.Create();

        IsObfusucateFileCreated = File.Exists(ObfuscateOrderPath);
        if (!IsObfusucateFileCreated)
        {
            ArrangeObfusucation(obfuscateNumber);
        }

        var obfusucateLines = File.ReadAllLines(ObfuscateOrderPath);
        _NUMBERS = obfusucateLines[0].ToCharArray();
        _CHARS = obfusucateLines[1].ToCharArray();
        _CHARS_UPPER = obfusucateLines[2].ToCharArray();
        _obfuscateNumber = Convert.ToInt32(obfusucateLines[3]);
    }

    public void Shuffle<T>(T[] array)
    {
        var random = new Random();
        for (var i = 0; i < array.Length; i++)
        {
            var x = random.Next(i, array.Length);
            var tmp = array[i];
            array[i] = array[x];
            array[x] = tmp;
        }
    }

    public string GetRandomString(int size = 16)
    {
        var bytes = new byte[size];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    public string Hash(string text)
    {
        var hashBytes = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(text));
        return Convert.ToBase64String(hashBytes);
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
        {
            return string.Empty;
        }

        var bytes = Encoding.UTF8.GetBytes(plainText);
        var encryptedBytes = new byte[bytes.Length];
        for (var i = 0; i < bytes.Length; i++)
        {
            encryptedBytes[i] = (byte)(bytes[i] + _obfuscateNumber);
        }

        return Convert.ToBase64String(encryptedBytes);
    }

    public string Decrypt(string encryptedText)
    {
        var plaintext = string.Empty;
        if (string.IsNullOrWhiteSpace(encryptedText))
        {
            return plaintext;
        }

        var encryptedBytes = Convert.FromBase64String(encryptedText);

        var bytes = new byte[encryptedBytes.Length];
        for (var i = 0; i < encryptedBytes.Length; i++)
        {
            bytes[i] = (byte)(encryptedBytes[i] - _obfuscateNumber);
        }

        return Encoding.UTF8.GetString(bytes);
    }

    public string Obfuscate(string text)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            bool predicate(char x) => x == c;

            var theNumber = NUMBERS.FirstOrDefault(predicate);
            if (theNumber != default(char))
            {
                sb.Append(_NUMBERS[NUMBERS.IndexOf(c)]);
                continue;
            }

            var theChar = CHARS.FirstOrDefault(predicate);
            if (theChar != default(char))
            {
                sb.Append(_CHARS[CHARS.IndexOf(c)]);
                continue;
            }

            var theCharUpper = CHARS_UPPER.FirstOrDefault(predicate);
            if (theCharUpper != default(char))
            {
                sb.Append(_CHARS_UPPER[CHARS_UPPER.IndexOf(c)]);
                continue;
            }

            sb.Append(c);
        }

        return sb.ToString();
    }
}