using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CryptoUtil.Test;

public class CryptoUtilTests : BaseTest
{
    private readonly CryptoUtil _cryptoUtil;

    public CryptoUtilTests()
    {
        var _obfuscateNumber = 21;
        _cryptoUtil = new CryptoUtil(_obfuscateNumber);
    }

    public void test_have_DataFolderName_variable()
    {
        AssertEqual("_Data", _cryptoUtil.DataFolderName);
    }

    public void test_create_data_folder_if_not_exits_in_constructor()
    {
        AssertFolderExists(_cryptoUtil.DataFolderName);
    }

    public void test_have_ObfuscateOrderPath_variable()
    {
        AssertEqual("_Data/_Obfuscate", _cryptoUtil.ObfuscateOrderPath);
    }

    public void test_hash_algorithm_assigned()
    {
        AssertTrue(_cryptoUtil.IsHashAlgorithmAssigned);
    }

    public void test_have_NUMBERS_variable()
    {
        AssertEqual("0123456789", _cryptoUtil.NUMBERS);
    }

    public void test_have_CHARS_variable()
    {
        AssertEqual("abcçdefghıijklmnoöprsştuüvyzxwq", _cryptoUtil.CHARS);
    }

    public void test_have_CHARS_UPPER_variable()
    {
        AssertEqual("ABCÇDEFGHIİJKLMNOÖPRSŞTUÜVYZXWQ", _cryptoUtil.CHARS_UPPER);
    }

    public void test_create_obfuscate_order_if_not_exits()
    {
        AssertFileExists(_cryptoUtil.ObfuscateOrderPath);
        var lines = File.ReadAllLines(_cryptoUtil.ObfuscateOrderPath);
        AssertEqual(4, lines.Length);

        var numbers = lines[0];
        AssertEqual(10, numbers.Length);
        var chars = lines[1];
        AssertEqual(31, chars.Length);
        var charsUpper = lines[2];
        AssertEqual(31, charsUpper.Length);

        for (var i = 0; i < _cryptoUtil.NUMBERS.Length; i++)
        {
            AssertTrue(numbers.Contains(_cryptoUtil.NUMBERS[i]));
        }

        for (var i = 0; i < _cryptoUtil.CHARS.Length; i++)
        {
            AssertTrue(chars.Contains(_cryptoUtil.CHARS[i]));
        }

        for (var i = 0; i < _cryptoUtil.CHARS_UPPER.Length; i++)
        {
            AssertTrue(charsUpper.Contains(_cryptoUtil.CHARS_UPPER[i]));
        }

        AssertTrue(int.Parse(lines[3]) > 0);
    }

    public void test_Hash()
    {
        for (var i = 0; i < 5; i++)
        {
            AssertEqual("e/qVpoiSTEfH0iOB8gzJJvUkvqyxP4TiA9S9jLa6L86BxXpfBZvz1QmSZIe96SWzvO4GNeT3uuugVOXbppayvw==", _cryptoUtil.Hash(TEST_STRING));
            AssertEqual("mLGB6Kr94nvZWzJyBp+geDMy8JXRPDmbmTA+/rip4ftz8fOlok+ODjeMaYRDqPk3M0FkvX9IyeCQ1+LL86FzQQ==", _cryptoUtil.Hash(TEST_STRING_TR));
            AssertEqual("kYbv5WMBHZetfLQbBbRQ2WRpkYrs2L/9PgUb5TqiBxalbMzY8geEHqq/dYuf2ufJVAznMmPst2VPbym3y9QKBA==", _cryptoUtil.Hash(TEST_NUMBER_DECIMAL.ToString()));
        }
    }

    public void test_GetRandomString()
    {
        var strings = new List<string>();
        for (var i = 0; i < 1000; i++)
        {
            var randomString = _cryptoUtil.GetRandomString();

            AssertEqual(24, randomString.Length);
            AssertFalse(strings.Any(x => x == randomString));
            strings.Add(randomString);
        }
    }

    public void test_Encrypt()
    {
        var encrypted = _cryptoUtil.Encrypt(TEST_STRING);
        AssertNotEmpty(encrypted);
        AssertEqual("aVpoaQ==", encrypted);
    }

    public void test_Decrypt()
    {
        var decrypted = _cryptoUtil.Decrypt("aVpoaQ==");
        AssertNotEmpty(decrypted);
        AssertEqual(TEST_STRING, decrypted);
    }

    public void test_turkish_chars_Encrypt_Decrypt()
    {
        var encrypted = _cryptoUtil.Encrypt(TEST_STRING_TR);
        AssertNotEmpty(encrypted);
        AssertEqual("aVpoaTXZxti82Mt+2rTY0dm0NV7YnNir2cXas9ix2bM1RkdI", encrypted);
        AssertEqual(TEST_STRING_TR, _cryptoUtil.Decrypt(encrypted));
    }

    public void test_turkish_sentences_Encrypt_Decrypt()
    {
        var sentences = new[] {
                "Örnek birkaç Türkçe cümle!",
                "İstikbalde dahi seni hazinelerinden mahrum etmek isteyecek dahili ve harici bedhahların olacaktır.",
                "Cebren ve hile ile aziz vatanın bütün kaleleri zapt edilmiş, bütün tersanelerine girilmiş, bütün orduları dağıtılmış ve memleketin her köşesi bilfiil işgal edilmiş olabilir.",
                "İşte, bu ahval ve şerait içinde dahi vazifen, Türk istiklal ve cumhuriyetini kurtarmaktır.",
                "Çalışmadan, yorulmadan ve üretmeden, rahat yaşamak isteyen toplumlar; evvela haysiyetlerini, sonra hürriyetlerini daha sonra da istiklal ve istikballerini kaybetmeye mahkumdurlar."
            };

        var encrypts = new[] {
                "2KuHg3qANXd+h4B22Lw1adjRh4DYvHo1eNjRgoF6Ng==",
                "2cWIiX6Ad3aBeXo1eXZ9fjWIeoN+NX12j36DeoF6h36DeXqDNYJ2fYeKgjV6iYJ6gDV+iIl6jnp4eoA1eXZ9foF+NYt6NX12h354fjV3enl9dn2BdofZxoM1hIF2eHaAidnGh0M=",
                "WHp3h3qDNYt6NX1+gXo1foF6NXaPfo81i3aJdoPZxoM1d9jRidjRgzWAdoF6gXqHfjWPdoWJNXp5foGCftq0QTV32NGJ2NGDNYl6h4h2g3qBeod+g3o1fH6HfoGCftq0QTV32NGJ2NGDNYSHeYqBdofZxjV5dtm02caJ2caBgtnG2rQ1i3o1gnqCgXqAeol+gzV9eoc1gNjL2rR6iH41d36Be35+gTV+2rR8doE1enl+gYJ+2rQ1hIF2d36BfodD",
                "2cXatIl6QTV3ijV2fYt2gTWLejXatHqHdn6JNX7YvH6DeXo1eXZ9fjWLdo9+e3qDQTVp2NGHgDV+iIl+gIF2gTWLejV4ioJ9iod+jnqJfoN+NYCKh4l2h4J2gInZxodD",
                "2Jx2gdnG2rSCdnl2g0E1joSHioGCdnl2gzWLejXY0Yd6iYJ6eXqDQTWHdn12iTWOdtq0doJ2gDV+iIl6jnqDNYmEhYGKgoF2h1A1eouLeoF2NX12joh+jnqJgXqHfoN+QTWIhIOHdjV92NGHh36OeomBeod+g341eXZ9djWIhIOHdjV5djV+iIl+gIF2gTWLejV+iIl+gHd2gYF6h36DfjWAdo53eomCeo56NYJ2fYCKgnmKh4F2h0M="
            };

        for (var i = 0; i < sentences.Length; i++)
        {
            var sentence = sentences[i];
            var encrypted = _cryptoUtil.Encrypt(sentence);
            AssertNotEmpty(encrypted);
            AssertEqual(encrypts[i], encrypted);
            AssertEqual(sentence, _cryptoUtil.Decrypt(encrypted));
        }
    }

    public void test_obfuscate()
    {
        var obfuscated = _cryptoUtil.Obfuscate(TEST_STRING_TR);

        var expected = new StringBuilder();
        for (var i = 0; i < TEST_STRING_TR.Length; i++)
        {
            var c = TEST_STRING_TR[i];
            bool predicate(char x) => x == c;

            var theNumber = _cryptoUtil.NUMBERS.FirstOrDefault(predicate);
            if (theNumber != default(char))
            {
                expected.Append(_cryptoUtil._NUMBERS[_cryptoUtil.NUMBERS.IndexOf(c)]);
                continue;
            }

            var theChar = _cryptoUtil.CHARS.FirstOrDefault(predicate);
            if (theChar != default(char))
            {
                expected.Append(_cryptoUtil._CHARS[_cryptoUtil.CHARS.IndexOf(c)]);
                continue;
            }

            var theCharUpper = _cryptoUtil.CHARS_UPPER.FirstOrDefault(predicate);
            if (theCharUpper != default(char))
            {
                expected.Append(_cryptoUtil._CHARS_UPPER[_cryptoUtil.CHARS_UPPER.IndexOf(c)]);
                continue;
            }

            expected.Append(c);
        }

        AssertEqual(expected.ToString(), obfuscated);
    }
}