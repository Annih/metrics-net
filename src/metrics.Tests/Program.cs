using NUnitLite;
using System;
using System.Reflection;

namespace metrics.Tests
{
    public class Program
    {
        public static int Main(string[] args)
        {
#if COREFX
            return new AutoRun().Execute(typeof(Program).GetTypeInfo().Assembly, Console.Out, Console.In, args);
#else
            return new AutoRun().Execute(args);
#endif
        }
    }
}
