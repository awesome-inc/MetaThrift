package inc.awesome.metathrift.metaserver;

import inc.awesome.metathrift.Action;
import inc.awesome.metathrift.ArgumentException;
import inc.awesome.metathrift.DynamicMetaService;
import inc.awesome.metathrift.Function;
import inc.awesome.metathrift.MetaBroker;
import inc.awesome.metathrift.MetaService;
import inc.awesome.metathrift.MetaService.Processor;
import inc.awesome.metathrift.MetaServiceInfo;
import inc.awesome.metathrift.ServiceException;
import inc.awesome.metathrift.Thrift;
import inc.awesome.metathrift.Tuple2;
import inc.awesome.metathrift.Tuple3;
import inc.awesome.metathrift.VoidAction;
import inc.awesome.metathrift.VoidFunction;

import java.io.IOException;

import org.apache.thrift.TException;
import org.apache.thrift.TProcessor;
import org.apache.thrift.server.TServer;
import org.apache.thrift.transport.TTransportException;

import com.google.gson.reflect.TypeToken;

final class Program {

	private static MetaService.Iface service;
	private static MetaService.Processor<MetaService.Iface> processor;
	private static String serverUrl = "tcp://localhost:9092";
    private static MetaServiceInfo serviceInfo = new MetaServiceInfo();
    private static TServer server;
	
	private final static String brokerUrl = "tcp://localhost:9090";
    private static MetaBroker.Iface broker;
    
	public static void main(String[] args) {
		try 
		{
			parseArgs(args);
	        setUp();
	        
			Runtime.getRuntime().addShutdownHook(new Thread(new Runnable() {
			    public void run() 
			    { 
			    	try { shutDown(); } 
			    	catch (TException e) { e.printStackTrace(); }
			    }
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

	private static void setUp() throws ArgumentException, ServiceException, TException {
		
		serviceInfo.setName("MetaServer");
		serviceInfo.setUrl(serverUrl);
		serviceInfo.setDescription("An example service");
		serviceInfo.setDisplayName("MetaServer");
		
		service = createService();
		processor = new Processor<MetaService.Iface>(service);
		
		server = startServer(processor, serverUrl);
		
		System.out.println("Started server at:" + serverUrl);
		
		broker = new MetaBroker.Client(Thrift.createProtocol(brokerUrl));
		broker.bind(serviceInfo);
		System.out.println("Bound service to MetaBroker at:" + brokerUrl);
	}
	
	private static void shutDown() throws ArgumentException, ServiceException, TException {
		broker.unbind(serviceInfo.getName());
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
	
	private static MetaService.Iface createService() {
		DynamicMetaService service = new DynamicMetaService("SimpleServer", null, null);
		
		final Action<String> launchApp = new Action<String>() {
			public void call(String input) throws IOException { launchApp(input); }
		};
		
		service.registerAction("openBrowser", String.class, launchApp, 
			"Open Browser", "Opens a web browser with the specified url");
		service.registerAction("openVideo", String.class, launchApp,
			"Open Video", "Opens a video player for the specified video url");
		service.registerAction("openAudio", String.class, launchApp,
			"Open Audio", "Opens an audio player for the specified audio url");
		service.registerAction("searchDoodles",
			new VoidAction() {
				public void call() throws Exception { launchApp("www.google.com/doodles/"); }
			}, "Search Doodles", "Opens the Google Doodle page");
		
        service.registerFunc("sayHello", String.class, String.class, 
    		new Function<String, String>() {
        		public String call(String input) throws Exception { return "Hello, " + input; }
        	}, "Greet Me", "Replies hello to the specified user");
        service.registerFunc("fibonacci", Integer.class, Integer.class, 
    		new Function<Integer, Integer>() {
    			public Integer call(Integer input) throws Exception { return fibonacci(input); }
    		}, "Fibonacci", "Computes the Fibonacci series for the specified number");
        service.registerFunc("factorial", Integer.class, Integer.class, 
    		new Function<Integer, Integer>() {
    			public Integer call(Integer input) throws Exception { return factorial(input); }
    		}, "Factorial", "Computes the facorial for the specified number");
        service.registerFunc("sayHello", String.class, 
    		new VoidFunction<String>() {
    			public String call() throws Exception { return "Hello, everyone"; }
            }, "Greet Me", "Replies hello to the specified user");
        service.registerFunc("add",
    		new TypeToken<Tuple2<Integer,Integer>>(){}.getType(), Integer.class, 
    		new Function<Tuple2<Integer,Integer>,Integer>() {
    			public Integer call(Tuple2<Integer,Integer> tuple) throws Exception { return tuple.Item1 + tuple.Item2; }
            }, "Add", "Adds two integers");
        
        service.registerFunc("lerp", 
        	new TypeToken<Tuple3<Integer,Integer,Double>>(){}.getType(), Double.class, 
    		new Function<Tuple3<Integer,Integer,Double>,Double>() {
    			public Double call(Tuple3<Integer,Integer,Double> tuple) throws Exception { return lerp(tuple); }
            }, "Lerp", "Linear interpolates the first two arguments by the third argument");
        
		return service;
	}

	private static void launchApp(String input) throws IOException {
		String cmdLine = "cmd.exe /c start " + input;
		Runtime.getRuntime().exec(cmdLine);
	}

	private static Integer fibonacci(Integer n) {
		if (n < 0)
            throw new IllegalArgumentException("Input value must be greater or equal to zero.");
        if (n == 0)
            return 0;
        if (n == 1)
            return 1;
        return fibonacci(n - 1) + fibonacci(n - 2);
	}

	private static Integer factorial(Integer n) {
		if (n < 0)
            throw new IllegalArgumentException("Input value must be greater or equal to zero.");
        if (n == 0)
            return 1;
        return n * factorial(n - 1);
	}
	
	private static Double lerp(Tuple3<Integer, Integer, Double> input) {
		return input.Item1 + (input.Item2 - input.Item1) * input.Item3;
	}	
}
