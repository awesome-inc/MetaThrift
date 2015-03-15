using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Thrift;
using Thrift.Server;

namespace MetaBroker
{
    class Program
    {
        private static string _tcpUrl = "tcp://localhost:9090";
        private static TServer _tcpServer;

        private static string _httpUrl = "http://localhost:9091/services/metabroker/";
        private static TServer _httpServer;

        static void Main(string[] args)
        {
            try
            {
                ParseArgs(args);
                Startup();

                AppDomain.CurrentDomain.UnhandledException += (s, e) => Handle(e.ExceptionObject);
                Console.WriteLine("Servers started. Press <ENTER> to terminate");
                Console.ReadLine();

                Shutdown();
            }
            catch (Exception ex)
            {
                Handle(ex);
            }
        }

        private static void Handle(object ex)
        {
            Trace.TraceError("Unhandled exception: {0}", ex);
            Environment.Exit(-1);
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
                }
            }
        }

        private static void Startup()
        {
            var service = new MetaBrokerService();
            var processor = new MetaThrift.MetaBroker.Processor(service);
            _tcpServer = StartServer(processor, _tcpUrl);

            var httpServer = (ThriftHttpServer) StartServer(processor, _httpUrl);
            
            // add CORS
            httpServer.AddReponseHeader("Access-Control-Allow-Credentials", "true");
            httpServer.AddReponseHeader("Access-Control-Allow-Origin", "*");
            httpServer.AddReponseHeader("Access-Control-Origin", "*");
            httpServer.AddReponseHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            httpServer.AddReponseHeader("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
            httpServer.AddReponseHeader("Access-Control-Max-Age", "86400");

            _httpServer = httpServer;
        }

        private static void Shutdown()
        {
            StopServer(_tcpServer);
            StopServer(_httpServer);
            Trace.TraceInformation("Servers stopped.");
        }

        private static void StopServer(TServer server)
        {
            if (server == null) return;
            server.Stop();
            SafeDispose(server);
        }

        private static TServer StartServer(TProcessor processor, string url)
        {
            if (String.IsNullOrWhiteSpace(url)) return null;
            var server = processor.CreateServer(url);
            new Thread(server.Serve).Start();
            Trace.TraceInformation("Server started on: " + url);
            return server;
        }

        private static void SafeDispose(object obj)
        {
            var disposable = obj as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}
