using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CryptoUtil.Test;

public abstract class BaseTest
{
    public const string TEST_STRING = "TEST";
    public const string TEST_STRING_TR = "TEST ıçöişüğ IÇÖİŞÜĞ 123";
    public const int TEST_NUMBER = 123;
    public const decimal TEST_NUMBER_DECIMAL = 123.45m;

    public virtual void Reset() { }
    public virtual void CleanUp() { }

    private int _countOk = 0;
    private int _countFail = 0;

    private readonly TestUtil _testUtil;

    public BaseTest()
    {
        _testUtil = new TestUtil();
    }

    public (int, int) Run()
    {
        _testUtil.WriteLine("Running " + this.GetType().Name, ConsoleColor.White);

        var publicMethods = this.GetType().GetMethods().Where(x => x.Name.StartsWith("test_")).ToList();
        for (var i = 0; i < publicMethods.Count; i++)
        {
            var testCase = publicMethods[i];
            _testUtil.WriteLine("  " + testCase.Name, ConsoleColor.Blue);

            try
            {
                var stopwatch = Stopwatch.StartNew();
                Reset();

                testCase.Invoke(this, null);
                stopwatch.Stop();

                _testUtil.WriteLine(" - OK (" + _testUtil.GetTimeSpanString(stopwatch.Elapsed) + ")", ConsoleColor.Green);

                _countOk++;
            }
            catch (Exception ex)
            {
                _testUtil.WriteLine(" - FAILED", ConsoleColor.Red);

                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                    Console.WriteLine(Environment.NewLine);
                }

                Console.WriteLine(ex);
                Console.WriteLine(Environment.NewLine);

                _countFail++;
            }
        }

        _testUtil.WriteLine(Environment.NewLine + "Pass       = " + _countOk, ConsoleColor.Green);
        _testUtil.WriteLine("Fail       = " + _countFail, ConsoleColor.Red);

        CleanUp();

        return (_countOk, _countFail);
    }


    #region Assertions
    private string GetArgumentName()
    {
        var trace = new StackTrace(true).GetFrame(2);
        if (trace == null)
        {
            throw new Exception("stactrace frame 2 is null, the argument name can't retrieved!");
        }

        var fileName = trace.GetFileName();
        if (fileName == null)
        {
            throw new Exception("stactrace fileName is null, the argument name can't retrieved!");
        }

        var line = File.ReadAllLines(fileName)[trace.GetFileLineNumber() - 1];
        var argumentNames = line.Split(new[] { ",", "(", ")", ";" }, StringSplitOptions.TrimEntries)
                                .Where(x => x.Trim().Length > 0).Skip(1).ToList();
        return argumentNames.Last();

    }

    protected void AssertEqual(object expectedValue, object actualValue)
    {
        if (expectedValue.GetType() == typeof(decimal))
        {
            if (((decimal)actualValue).ToString("{0:0.####}") != ((decimal)expectedValue).ToString("{0:0.####}"))
            {
                var argumentName = GetArgumentName();

                throw new Exception(argumentName + " is not correct. It is " + actualValue + " but expected was " + expectedValue);
            }
        }
        else if (actualValue.ToString() != expectedValue.ToString())
        {
            var argumentName = GetArgumentName();

            throw new Exception(argumentName + " is not correct. It is " + actualValue + " but expected was " + expectedValue);
        }
    }

    protected void AssertNotEqual(object expectedValue, object actualValue)
    {
        if (actualValue.ToString() == expectedValue.ToString())
        {
            var argumentName = GetArgumentName();

            throw new Exception(argumentName + " is not correct. " + actualValue + " not expected to equal " + expectedValue + ".");
        }
    }

    protected void AssertTrue(bool booleanValue)
    {
        if (booleanValue == true)
        {
            return;
        }

        var argumentName = GetArgumentName();

        throw new Exception(argumentName + " is not true.");
    }

    protected void AssertFalse(bool booleanValue)
    {
        if (booleanValue == false)
        {
            return;
        }

        var argumentName = GetArgumentName();

        throw new Exception(argumentName + " is not false.");
    }

    protected void AssertFileExists(string filePath)
    {
        if (File.Exists(filePath))
        {
            return;
        }

        throw new Exception(filePath + " is not exists");
    }

    protected void AssertFolderExists(string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            return;
        }

        throw new Exception(folderPath + " is not exists");
    }

    protected void AssertEmpty(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        var argumentName = GetArgumentName();

        throw new Exception(argumentName + " is not empty");
    }

    protected void AssertNotEmpty(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            var argumentName = GetArgumentName();

            throw new Exception(argumentName + " is empty");
        }
    }

    protected void AssertNull<T>(T value)
    {
        if (value != null)
        {
            var argumentName = GetArgumentName();

            throw new Exception(argumentName + " is not null");
        }
    }

    protected void AssertNotNull<T>(T value)
    {
        if (value == null)
        {
            var argumentName = GetArgumentName();

            throw new Exception(argumentName + " is null");
        }
    }

    protected void AssertType<T>(object classUnderTest)
    {
        if (typeof(T) != classUnderTest.GetType())
        {
            throw new Exception(classUnderTest.GetType().Name + " is not correct type. Expected was " + typeof(T).Name);
        }
    }

    protected void AssertException(Action action, Type exceptionType)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            if (ex.GetType() == exceptionType)
            {
                return;
            }

            Console.WriteLine(ex);
            throw new Exception(exceptionType.Name + "Exception type is not correct");

        }

        throw new Exception(exceptionType.Name + " is not thrown");
    }
    #endregion
}
