using System;
using System.Collections.Generic;
using MetaThrift;

namespace TestServer
{
    class SampleService : AbstractMetaService
    {
        // ReSharper disable InconsistentNaming
        static readonly List<MetaOperation> operations = new List<MetaOperation>
        // ReSharper restore InconsistentNaming
        {
            "openBrowser".ToMetaAction<string>("Open Browser", "Opens a web browser with the specified uri"),
            "openVideo".ToMetaAction<string>("Open Video", "Opens a video player with the specified video url"),
            "openAudio".ToMetaAction<string>("Open Audio", "Opens an audio player with the specified audio url"),

            "sayHello".ToMetaFunction<string,string>("Greet Me", "Replies hello to the specified user"),
            "fibonacci".ToMetaFunction<int,int>("Compute Fibonacci", "Computes the Fibonacci series for the specified number"),
            "factorial".ToMetaFunction<int,int>("Compute Factorial", "Computes the facorial for the specified number"),
            "add".ToMetaFunction<Tuple<int,int>, int>("Add", "Adds the two specified integers"),
            "lerp".ToMetaFunction<Tuple<int,int,double>, double>("Lerp", "Linear interpolates the first two argument by the third argument")
        };

        public override IEnumerable<MetaOperation> Operations { get { return operations; } }

        public override object Call(MetaOperation operation, object value)
        {
            switch (operation.Name)
            {
                case "openBrowser":
                case "openVideo":
                case "openAudio":
                    LaunchApp((string) value);
                    return null;

                case "fibonacci":
                    return Fibonacci((int) value);
                case "factorial":
                    return Factorial((int)value);
                case "add":
                    return Add((Tuple<int, int>)value);
                case "lerp":
                    return Lerp((Tuple<int, int, double>)value);
                default:
                    throw new InvalidOperationException("Invalid operation: " + operation.Name);
            }
        }

        internal static void LaunchApp(String url)
        {
            System.Diagnostics.Process.Start(url);
        }

        internal static void LaunchMediaInternalType(MyInternalType mediaUrl)
        {
            System.Diagnostics.Process.Start(mediaUrl.MyUri);
        }

        internal static int Fibonacci(int n)
        {
            if (n < 0)
                throw new System.ArgumentException("Input value must be greater or equal to zero.", "n");
            if (n == 0)
                return 0;
            if (n == 1)
                return 1;
            return Fibonacci(n - 1) + Fibonacci(n - 2);
        }

        internal static int Factorial(int n)
        {
            if (n < 0)
                throw new System.ArgumentException("Input value must be greater or equal to zero.", "n");
            if (n == 0)
                return 1;
            return n * Factorial(n - 1);
        }

        internal static int Add(Tuple<int, int> value)
        {
            return value.Item1 + value.Item2;
        }

        internal static double Lerp(Tuple<int, int, double> value)
        {
            return value.Item1 + (value.Item2 - value.Item1) * value.Item3;
        }
    }
}