package inc.awesome.metathrift;

import inc.awesome.metathrift.Action;
import inc.awesome.metathrift.DynamicMetaService;
import inc.awesome.metathrift.Function;
import inc.awesome.metathrift.Tuple2;
import inc.awesome.metathrift.Tuple3;
import inc.awesome.metathrift.VoidAction;
import inc.awesome.metathrift.VoidFunction;

import java.io.IOException;

import org.apache.thrift.TException;
import org.junit.Before;
import org.junit.Test;

import static org.junit.Assert.*;

import com.google.gson.reflect.TypeToken;

public class DynamicMetaServiceTest {

	final DynamicMetaService service = new DynamicMetaService("DynamicTestService", null, null);

	final Action<String> launchApp = new Action<String>() {
		@Override
		public void call(String input) throws IOException { MockService.launchApp(input); }
	};
	
	final Function<String,String> sayHello = new Function<String,String>() {
		@Override
		public String call(String name) throws Exception { return MockService.sayHello(name); }
	};
	
	final VoidAction searchDoodles = new VoidAction() {
		@Override
		public void call() throws Exception { MockService.launchApp("www.google.com/doodles/"); }
	};

	final VoidFunction<String> sayHelloVoid = new VoidFunction<String>() {
		@Override
		public String call() throws Exception { return "Hello, everyone"; }
	};

	@Before
	public void setUp() throws Exception {
		
		service.registerAction("openBrowser", String.class, launchApp, 
				"Open Browser", "Opens a web browser with the specified url");
		service.registerAction("openVideo", String.class, launchApp,
			"Open Video", "Opens a video player for the specified video url");
		service.registerAction("openAudio", String.class, launchApp,
			"Open Audio", "Opens an audio player for the specified audio url");
		service.registerAction("searchDoodles", searchDoodles,
			"Search Doodles", "Opens the Google Doodle page");

        service.registerFunc("sayHello", String.class, String.class, sayHello,
            	"Greet Me", "Replies hello to the specified user");
        service.registerFunc("fibonacci", Integer.class, Integer.class, 
        	new Function<Integer, Integer>() {
    			@Override
    			public Integer call(Integer input) throws Exception { return MockService.fibonacci(input); }
    		}, "Fibonacci", "Computes the Fibonacci series for the specified number");
        service.registerFunc("factorial", Integer.class, Integer.class, 
        	new Function<Integer, Integer>() {
    			@Override
    			public Integer call(Integer input) throws Exception { return MockService.factorial(input); }
    		}, "Factorial", "Computes the facorial for the specified number");
        service.registerFunc("add",
    		new TypeToken<Tuple2<Integer,Integer>>(){}.getType(), Integer.class, 
    		new Function<Tuple2<Integer,Integer>,Integer>() {
    			@Override
    			public Integer call(Tuple2<Integer,Integer> tuple) throws Exception { return tuple.Item1 + tuple.Item2; }
            }, "Add", "Adds two integers");
        service.registerFunc("lerp", 
        	new TypeToken<Tuple3<Integer,Integer,Double>>(){}.getType(), Double.class, 
    		new Function<Tuple3<Integer,Integer,Double>,Double>() {
    			@Override
    			public Double call(Tuple3<Integer,Integer,Double> tuple) throws Exception { return MockService.lerp(tuple); }
            }, "Lerp", "Linear interpolates the first two arguments by the third argument");
        service.registerFunc("sayHello", String.class, sayHelloVoid,
        	"Hello Everyone", "Replies Hello");
	}

	@Test
	public void Should_List_Operations() throws TException {
		MetaServiceTest.Should_List_Operations(service);
	}
	
	@Test
	public void Should_Call_Actions() throws Exception
    {
		MetaServiceTest.Should_Call_Actions(service);
    }

	@Test
	public void Should_Call_Functions() throws Exception
    {
		MetaServiceTest.Should_Call_Functions(service);
    }
	
	@Test
	public void Should_Call_Void_Action() throws Exception {
		MetaServiceTest.Should_Call_Void_Action(service);
	}

	@Test
	public void Should_Call_Void_Function() throws Exception {
		MetaServiceTest.Should_Call_Void_Function(service);
	}
	
	@Test
    public void Should_Unregister_Action() throws TException
    {
        int count = service.getOperations().size();
        service.unregisterAction("openBrowser", String.class);
        assertEquals(count - 1, service.getOperations().size());
        
		service.registerAction("openBrowser", String.class, launchApp, null, null);
        assertEquals(count, service.getOperations().size());
    }

    @Test
    public void Should_Unregister_Function() throws TException
    {
        int count = service.getOperations().size();
        service.unregisterFunc("sayHello", String.class, String.class);
        assertEquals(count - 1, service.getOperations().size());

        service.registerFunc("sayHello", String.class, String.class, sayHello, null, null);
        assertEquals(count, service.getOperations().size());
    }

    @Test
    public void Should_Unregister_Void_Action() throws TException
    {
        int count = service.getOperations().size();
        service.unregisterAction("searchDoodles");
        assertEquals(count - 1, service.getOperations().size());

		service.registerAction("searchDoodles", searchDoodles, null, null);
        assertEquals(count, service.getOperations().size());
    }

    @Test
    public void Should_Unregister_Void_Function() throws TException
    {
        int count = service.getOperations().size();
        service.unregisterFunc("sayHello", String.class);
        assertEquals(count - 1, service.getOperations().size());

        service.registerFunc("sayHello", String.class, sayHelloVoid, null, null);
        assertEquals(count, service.getOperations().size());
    }
}
