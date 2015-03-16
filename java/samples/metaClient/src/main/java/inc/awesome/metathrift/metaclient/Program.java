package inc.awesome.metathrift.metaclient;

import static inc.awesome.metathrift.Meta.*;
import static inc.awesome.metathrift.Thrift.*;

import inc.awesome.metathrift.MetaOperation;
import inc.awesome.metathrift.MetaService;
import inc.awesome.metathrift.MetaService.Client;
import inc.awesome.metathrift.Tuple2;
import inc.awesome.metathrift.Tuple3;

import com.google.gson.reflect.TypeToken;

public class Program {

	static String serverUrl = "tcp://localhost:9092";
	
	public static void main(String[] args) {
		try 
		{
			parseArgs(args);
			
		     MetaService.Client client = new MetaService.Client(createProtocol(serverUrl));
		     System.out.println("Connected to metaserver at:" + serverUrl);
		     
		     perform(client);
		} 
		catch (Exception x) 
		{
			x.printStackTrace(); 
		}
	}

	private static void parseArgs(String[] args) {
		for (int i = 0; i < args.length; i++)
        {
            if (args[i] == "--url")
            {
            	serverUrl = args[++i];
            	continue;
            }
        }
	}
	
	private static void perform(Client service) throws Exception {

		System.out.printf("%nOperations:%n");
		for (MetaOperation operation : service.getOperations())
			System.out.println(prettyPrint(operation));
		
        call(service, "openBrowser", "http://www.heise.de", String.class);
		System.out.println("openBrowser(\"http://www.heise.de\")");

		call(service, "searchDoodles");
		System.out.println("searchDoodles()");
		
        String userName = System.getProperty("user.name");
        String strResult = call(service, "sayHello", userName, String.class, String.class);
        System.out.printf("sayHello(%s) = %s%n", userName, strResult);

        Integer n = 20;
        Integer intResult = call(service, "fibonacci", n, Integer.class, Integer.class);
        System.out.printf("fibonacci(%s) = %s%n", n, intResult);

        n = 6;
        intResult = call(service, "factorial", n, Integer.class, Integer.class);
        System.out.printf("factorial(%s) = %s%n", n, intResult);
        
		String greetings = call(service, "sayHello", String.class);
        System.out.println("sayHello() = " + greetings);
        
        // add
        Integer a = 3, b = 4;
        intResult = call(service, "add", new Tuple2<Integer, Integer>(a, b),
        		new TypeToken<Tuple2<Integer,Integer>>(){}.getType(), Integer.class);
        System.out.printf("add(%s,%s) = %s%n", a, b, intResult);

        // lerp
        a = -1; b = 1;
        Double t = 0.5;
        Double doubleResult = call(service, "lerp", new Tuple3<Integer, Integer, Double>(a, b, t),
        		new TypeToken<Tuple3<Integer,Integer,Double>>(){}.getType(), Double.class);
        System.out.printf("lerp(%s,%s, %s) = %s%n", a, b, t, doubleResult);
	}
}
