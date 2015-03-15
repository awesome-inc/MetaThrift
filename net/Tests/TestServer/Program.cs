using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MetaThrift;
using Thrift;
using Thrift.Server;

namespace TestServer
{
    class Program
    {
        private static string _tcpUrl = "tcp://localhost:9092";
        private static TServer _tcpServer;

        private static string _httpUrl = "http://localhost:9093/services/metaserver/";
        private static TServer _httpServer;
        
        private static string _brokerUrl = "tcp://localhost:9090";
        private static MetaBroker.Iface _broker;
        private static readonly MetaServiceInfo ServiceInfo = new MetaServiceInfo
            {
                Name = "MetaServer",
                Url = _tcpUrl,
                Description = "An example service",
                DisplayName = "MetaServer",
            };


        static void Main(string[] args)
        {
            try
            {
                ParseArgs(args);
                Startup();

                AppDomain.CurrentDomain.UnhandledException += (s, e) => Handle(e.ExceptionObject);
                Console.WriteLine("Server started. Press <ENTER> to terminate");
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
                    case "-broker":
                        _brokerUrl = args[++i];
                        continue;
                    default:
                        throw new System.ArgumentException("Unknown option: " + args[i], "args");
                }
            }
        }

        private static void Startup()
        {
            var service = CreateMetaService();
            var processor = new MetaService.Processor(service);
            _tcpServer = StartServer(processor, _tcpUrl);
            _httpServer = StartServer(processor, _httpUrl);

            _broker = new MetaBroker.Client(_brokerUrl.CreateProtocol());
            ServiceInfo.Url = _tcpUrl;
            _broker.bind(ServiceInfo);
            Trace.TraceInformation("Bound service to MetaBroker at: " + _brokerUrl);
        }

        private static void Shutdown()
        {
            _broker.unbind(ServiceInfo.Name);
            StopServer(_tcpServer);
            StopServer(_httpServer);
        }

        private static MetaService.Iface CreateMetaService()
        {
            var service = new DynamicMetaService("TestServer");
            service.RegisterAction<string>("openBrowser", SampleService.LaunchApp, "Open Browser", "Opens a web browser");
            service.RegisterAction<string>("openVideo", SampleService.LaunchApp, "Open Video", "Opens a video player");
            service.RegisterAction<string>("openAudio", SampleService.LaunchApp, "Open Audio", "Opens an audio player");
            service.RegisterAction("searchDoodles", () => SampleService.LaunchApp("www.google.com/doodles/"), "Search Doodles", "Opens the Google Doodle page");
         
            service.RegisterAction<MyInternalType>("openMedia", SampleService.LaunchMediaInternalType, "Open Media", "Opens the specified media");
            
            service.RegisterFunc<int, int>("fibonacci", SampleService.Fibonacci, "Compute Fibonacci", "Computes the Fibonacci series");
            service.RegisterFunc<int, int>("factorial", SampleService.Factorial, "Compute Factorial", "Computes the Facorial");
            service.RegisterFunc<Tuple<int, int>, int>("add", SampleService.Add, "Add", "Adds the two specified integers");
            service.RegisterFunc<Tuple<int, int, double>, double>("lerp", SampleService.Lerp, "Add", "Adds the two specified integers");
            
            service.RegisterFunc<string, string>("sayHello", user => "Hello, " + user, "Say Hello", "Says hello to the specified user");
            service.RegisterFunc("sayHello", () => "Hello, Everyone", "Say Hello", "Says hello to everyone");

            return service;
        }

        private static TServer StartServer(TProcessor processor, string url)
        {
            if (String.IsNullOrWhiteSpace(url)) return null;

            var server = processor.CreateServer(url);
            new Thread(server.Serve).Start();
            Trace.TraceInformation("Server started on: " + url);
            return server;
        }

        private static void StopServer(TServer server)
        {
            if (server == null) return;
            server.Stop();
            SafeDispose(server);
        }

        private static void SafeDispose(object obj)
        {
            var disposable = obj as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}
