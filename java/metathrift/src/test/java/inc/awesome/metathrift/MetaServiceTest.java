package inc.awesome.metathrift;

import inc.awesome.metathrift.Tuple2;
import inc.awesome.metathrift.Tuple3;
import inc.awesome.metathrift.MetaService.Iface;

import org.apache.thrift.TException;

import com.google.gson.reflect.TypeToken;

import static org.junit.Assert.*;
import static inc.awesome.metathrift.Meta.*;

public class MetaServiceTest 
{
    public static void Should_List_Operations(Iface service) throws TException
    {
		System.out.printf("%nOperations:%n");
        for (MetaOperation operation : service.getOperations())
        	System.out.println(prettyPrint(operation));
		assertTrue(true);
    }
	
	public static void Should_Call_Actions(Iface service) throws Exception
    {
        call(service, "openBrowser", "http://www.heise.de", String.class);
		assertTrue(true);
    }
	
    public static void Should_Call_Functions(Iface service) throws Exception
    {
		System.out.println();
		
        // sayHello
        String userName = System.getProperty("user.name");
        String strResult = call(service, "sayHello", userName, String.class, String.class);
        System.out.printf("sayHello(%s) = %s%n", userName, strResult);

        // fibonacci
        Integer n = 20;
        Integer intResult = call(service, "fibonacci", n, Integer.class, Integer.class);
        System.out.printf("fibonacci(%s) = %s%n", n, intResult);

        // factorial
        n = 6;
        intResult = call(service, "factorial", n, Integer.class, Integer.class);
        System.out.printf("factorial(%s) = %s%n", n, intResult);

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
        
		System.out.println();
    }
	
	public static void Should_Call_Void_Action(Iface service) throws Exception 
	{
		call(service, "searchDoodles");
		assertTrue(true);
	}
	
	public static void Should_Call_Void_Function(Iface service) throws Exception 
	{
		String greetings = call(service, "sayHello", String.class);
		assertTrue(greetings.equals("Hello, everyone"));
	}
}
