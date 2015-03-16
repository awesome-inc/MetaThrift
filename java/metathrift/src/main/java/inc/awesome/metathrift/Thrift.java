package inc.awesome.metathrift;

import org.apache.thrift.TProcessor;
import org.apache.thrift.protocol.TBinaryProtocol;
import org.apache.thrift.protocol.TJSONProtocol;
import org.apache.thrift.protocol.TProtocol;
import org.apache.thrift.server.TServer;
import org.apache.thrift.server.TThreadPoolServer;
import org.apache.thrift.transport.THttpClient;
import org.apache.thrift.transport.TServerSocket;
import org.apache.thrift.transport.TServerTransport;
import org.apache.thrift.transport.TSocket;
import org.apache.thrift.transport.TTransport;
import org.apache.thrift.transport.TTransportException;

public final class Thrift 
{
	final static String TCPPrefix = "tcp://";
	final static String HTTPPrefix = "http://";

	private Thrift() { }

	public static TProtocol createProtocol(String url) throws TTransportException
    {
        if (url.startsWith(HTTPPrefix))
            return createHttpProtocol(url);

        if (url.startsWith(TCPPrefix))
            return createTcpProtocol(url);

        throw new IllegalArgumentException("Unsupported service url: " + url);
    }

	public static TServer createServer(TProcessor processor, String url) throws TTransportException
	{
		if (url.startsWith(HTTPPrefix))
			return createHttpServer(url, processor);
		
		if (url.startsWith(TCPPrefix))
			return createTcpServer(url, processor);
		
		throw new IllegalArgumentException("Unsupported service url: " + url);
    }
	
	private static TProtocol createHttpProtocol(String url) throws TTransportException {
		TTransport transport = new THttpClient(url);
        if (!transport.isOpen())
            transport.open();
        return new TJSONProtocol(transport);
	}

	private static TProtocol createTcpProtocol(String url) throws TTransportException {
		String[] tokens = url.substring(TCPPrefix.length()).split(":");
        String host = tokens[0];
        int port = Integer.parseInt(tokens[1]);
        TTransport transport = new TSocket(host, port);
        if (!transport.isOpen())
            transport.open();
        //return new TCompactProtocol(transport); // TODO: test compact protocol implementation! --> does not work with 0.9.0!
        return new TBinaryProtocol(transport);
	}
	
	private static TServer createTcpServer(String url, TProcessor processor) throws TTransportException
    {
        String[]  tokens = url.substring(TCPPrefix.length()).split(":");
        //String host = tokens[0];
        int port = Integer.parseInt(tokens[1]);
        TServerTransport tcpTransport = new TServerSocket(port);
        //return new TSimpleServer(processor, tcpTransport);
        return new TThreadPoolServer( new TThreadPoolServer.Args(tcpTransport).processor(processor) );
    }
	
	private static TServer createHttpServer(String url, TProcessor processor) {
		throw new UnsupportedOperationException("Not yet implemented");
	}
}
