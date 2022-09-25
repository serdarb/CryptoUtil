using System;
using System.Text;
using CryptoUtil.Test;

Console.OutputEncoding = Encoding.UTF8;

if (args == null) { args = Array.Empty<string>(); }
if (args.Length > 0
    && args[0] == "test")
{
    var testUtil = new TestUtil();
    testUtil.Run();
    return;
}

