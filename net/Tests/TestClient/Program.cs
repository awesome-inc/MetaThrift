using System;
using System.Collections.Generic;
using MetaThrift;
using Thrift;

namespace TestClient
{
    class Program
    {
        private static string _tcpUrl = "tcp://localhost:9092";
        private static string _httpUrl = String.Empty; //"http://localhost:9093/services/metaserver/";

        static void Main(string[] args)
        {
            ParseArgs(args);
            TestProtocol(_tcpUrl);
            TestProtocol(_httpUrl);

            Console.WriteLine("Press <ENTER> to terminate.");
            Console.ReadLine();
        }

        private static void ParseArgs(IList<string> args)
        {
            for (var i = 0; i < args.Count; i++)
            {
                switch (args[i])
                {
                    case "-tcp":
                        _tcpUrl = args[++i];
                        continue;
                    case "-http":
                        _httpUrl = args[++i];
                        continue;
                    default:
                        throw new System.ArgumentException("Unknown option: "+args[i], "args");
                }
            }
        }

        static void TestProtocol(string url)
        {
            if (String.IsNullOrWhiteSpace(url)) return;

            var client = new MetaService.Client(url.CreateProtocol());

            TestOperations(client);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void TestOperations(MetaService.Iface service)
        {
            Console.WriteLine("\r\nOperations:");
            foreach (var operation in service.getOperations())
                Console.WriteLine(operation.PrettyPrint());

            // check the service supports our operation
            if (!service.getOperations().Contains("openBrowser".ToMetaAction<string>()))
            {
                Console.WriteLine("The server does not support action \"openBrowser\".");
                return;
            }

            const string mediaUrl = "http://www.google.de";
            Console.Write("\r\nOpening website: openBrowser(\"{0}\")", mediaUrl);
            service.Call("openBrowser", mediaUrl);
            Console.WriteLine("\r\nPress <ENTER> to continue.");
            Console.ReadLine();

            // check the service supports the operation
            if (!service.getOperations().Contains("fibonacci".ToMetaFunction<int, int>()))
            {
                Console.WriteLine("The server does not support function \"fibonacci\".");
                return;
            }
            
            const int n = 20;
            Console.Write("\r\nCalculation of Fibonacci series: fibonacci({0}) = ", n);
            var intResult = service.Call<int, int>("fibonacci", n); // test xml
            Console.WriteLine(intResult);

            // add
            int a = 3, b = 4;
            intResult = service.Call<Tuple<int, int>, int>("add", new Tuple<int, int>(a, b));
            Console.WriteLine("add({0},{1}) = {2}", a, b, intResult);

            // lerp
            a = -1; b = 1;
            const double t = 0.5;
            var doubleResult = service.Call<Tuple<int, int, double>, double>("lerp", new Tuple<int, int, double>(a, b, t));
            Console.WriteLine("lerp({0},{1}, {2}) = {3}", a, b, t, doubleResult);

            Console.WriteLine("\r\nPress <ENTER> to continue.");
            Console.ReadLine();
        }
    }
}
