using System;
using Thrift.Protocol;
using Thrift.Server;
using Thrift.Transport;

namespace Thrift
{
    public static class Extensions
    {
        public static TProtocol CreateProtocol(this string url)
        {
            if (url.StartsWith("http://"))
                return CreateHttpProtocol(url);
            if (url.StartsWith("tcp://"))
                return CreateTcpProtocol(url);
            throw new NotSupportedException("Unsupported service url: " + url);
        }

        public static TServer CreateServer(this TProcessor processor, string url)
        {
            if (url.StartsWith("http://"))
                return new ThriftHttpServer(url, processor);
            if (url.StartsWith("tcp://"))
                return CreateTcpServer(url, processor);
            throw new NotSupportedException("Unsupported service url: " + url);
        }

        private static TProtocol CreateHttpProtocol(string url)
        {
            var client = new THttpClient(new Uri(url));
            if (!client.IsOpen)
                client.Open();
            return new TJSONProtocol(client);
        }

        private static TProtocol CreateTcpProtocol(string url)
        {
            var token = url.Substring("tcp://".Length).Split(':');
            var socket = new TSocket(token[0], int.Parse(token[1]));
            if (!socket.IsOpen)
                socket.Open();
            return new TBinaryProtocol(socket);
        }

        private static TServer CreateTcpServer(string url, TProcessor processor)
        {
            var port = int.Parse(url.Substring("tcp://".Length).Split(':')[1]);
            var socket = new TServerSocket(port);
            return new TThreadPoolServer(processor, socket);
        }
    }
}
