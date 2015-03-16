package inc.awesome.metathrift.metabroker;

import inc.awesome.metathrift.MetaBroker;
import inc.awesome.metathrift.Thrift;

import org.apache.thrift.TProcessor;
import org.apache.thrift.server.TServer;
import org.apache.thrift.transport.TTransportException;

public class Program {

	static MetaBroker.Iface service;
	static String serverUrl = "tcp://localhost:9090";
	static TServer server;
	
	public static void main(String[] args) {
		try 
		{
			parseArgs(args);
			setUp();
			
			Runtime.getRuntime().addShutdownHook(new Thread(new Runnable() {
			    public void run() { shutDown(); }
			}));			
			
			while (true) Thread.sleep(1000);
		} 
		catch (Exception x) 
		{
			x.printStackTrace(); 
		}
	}

	private static void parseArgs(String[] args) {
		for (int i = 0; i < args.length; i++)
        {
            if (args[i] == "-url")
            {
            	serverUrl = args[++i];
            	continue;
            }
        }
	}

	private static void setUp() throws TTransportException 
	{
		service = new MetaBrokerService();
		TProcessor processor = new MetaBroker.Processor<MetaBroker.Iface>(service);
		server = startServer(processor, serverUrl);
		System.out.println("Started server on:" + serverUrl);
	}
	
	private static void shutDown()
	{
		stopServer(server);
		System.out.println("Servers stopped.");
	}
	
	private static TServer startServer(final TProcessor processor, String url) throws TTransportException 
	{
		if (url == null || url.equals("")) return null;
		final TServer server = Thrift.createServer(processor, url);
		Thread serverThread = new Thread(new Runnable() 
		{
			public void run() 
			{
				server.serve();
			} 
		});
		serverThread.start();
		return server;
	}
	
	private static void stopServer(final TServer server)
	{
		if (server == null) return;
		server.stop();
	}
}
