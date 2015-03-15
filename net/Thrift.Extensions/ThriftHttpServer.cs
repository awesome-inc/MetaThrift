using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Thrift.Protocol;
using Thrift.Server;
using Thrift.Transport;

namespace Thrift
{
    public class ThriftHttpServer : TServer, IDisposable
    {
        private readonly THttpHandler _httpHandler;
        private readonly HttpListener _listener = new HttpListener();
        private bool _isDisposed;
        private readonly Dictionary<string, string> _responseHeaders = new Dictionary<string, string>();

        public List<KeyValuePair<string, string>> CustomResponseHeaders
        {
            get { return _responseHeaders.ToList(); }
        }

        public ThriftHttpServer(string prefix, TProcessor processor)
            : base(processor, null)
        {
            if (prefix == null) throw new ArgumentNullException("prefix");
            if (processor == null) throw new ArgumentNullException("processor");
            var factory = new TJSONProtocol.Factory();
            _httpHandler = new THttpHandler(processor, factory);
            _listener.Prefixes.Add(prefix);
        }

        public void AddReponseHeader(string name, string value)
        {
            _responseHeaders.Add(name, value);
        }

        public override void Serve()
        {
            _listener.Start();
            HandleRequests();
        }

        public override void Stop()
        {
            _listener.Stop();
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            _listener.Close();
            _isDisposed = true;
        }

        private void HandleRequests()
        {
            while (_listener.IsListening)
            {
                try
                {
                    var context = _listener.GetContext();
                    AddCustomHeadersTo(context.Response);
                    _httpHandler.ProcessRequest(context);
                }
                catch (HttpListenerException ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }

        private void AddCustomHeadersTo(HttpListenerResponse response)
        {
            CustomResponseHeaders.ForEach(kvp => response.AddHeader(kvp.Key, kvp.Value));
        }
    }
}
