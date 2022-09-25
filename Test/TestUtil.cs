using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace CryptoUtil.Test;

public class TestUtil
{
    public void WriteLine(string message, ConsoleColor color)
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public string GetTimeSpanString(TimeSpan time)
    {
        if (time.Minutes > 1)
        {
            return time.Minutes + "m " + time.Seconds + "s " + time.Milliseconds + "ms";
        }

        if (time.Seconds > 1)
        {
            return time.Seconds + "s " + time.Milliseconds + "ms";
        }

        return time.Milliseconds + "ms";
    }

    public void Run()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var testClasses = assembly.GetTypes().Where(x => x.Name.EndsWith("Tests")).ToList();

        var _countOk = 0;
        var _countFail = 0;

        var stopwatchAll = Stopwatch.StartNew();

        for (var i = 0; i < testClasses.Count; i++)
        {
            var testClass = testClasses[i];
            var testClassInstance = Activator.CreateInstance(testClass) as BaseTest;
            if (testClassInstance == null)
            {
                WriteLine(testClass.Name + " is not derived from BaseTest and skipped!", ConsoleColor.Magenta);
                continue;
            }

            var stopwatch = Stopwatch.StartNew();
            var (_ok, _fail) = testClassInstance.Run();
            stopwatch.Stop();
            WriteLine("Time Spend = " + GetTimeSpanString(stopwatch.Elapsed), ConsoleColor.DarkYellow);
            Console.WriteLine(Environment.NewLine);

            _countOk += _ok;
            _countFail += _fail;
        }

        WriteLine("-".PadRight(30, '-'), ConsoleColor.DarkYellow);
        WriteLine("All Pass       > " + _countOk, ConsoleColor.Green);
        WriteLine("All Fail       > " + _countFail, ConsoleColor.Red);
        WriteLine("All Total      > " + (_countOk + _countFail), ConsoleColor.DarkYellow);
        stopwatchAll.Stop();
        WriteLine("All Time Spend > " + GetTimeSpanString(stopwatchAll.Elapsed), ConsoleColor.DarkYellow);
    }
}