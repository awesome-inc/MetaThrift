package inc.awesome.metathrift;

import inc.awesome.metathrift.AbstractMetaService;
import inc.awesome.metathrift.Meta;
import inc.awesome.metathrift.Tuple2;
import inc.awesome.metathrift.Tuple3;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import org.apache.thrift.TException;

import com.google.gson.reflect.TypeToken;

class MockService extends AbstractMetaService {

	private final List<MetaOperation> operations = new ArrayList<MetaOperation>();

	public MockService() {
		super("MockService", null, null);
		
		operations.add( Meta.toMetaOperation("openBrowser", String.class, null,
				"Open Browser", "Opens the standard browser with the specified url"));
		operations.add( Meta.toMetaOperation("openVideo", String.class, null,
				"Open Video", "Opens the the specified video url/file"));
		operations.add( Meta.toMetaOperation("openAudio", String.class, null,
				"Open Audio", "Opens the the specified audio url/file"));

		operations.add( Meta.toMetaOperation("searchDoodles", null, null,
				"Search Doodles", "Opens the Google Doodle page"));
		
		operations.add( Meta.toMetaOperation("sayHello", String.class, String.class,
				"Greet Me", "Replies hello to the specified user"));

		operations.add( Meta.toMetaOperation("fibonacci", Integer.class, Integer.class,
				"Compute Fibonacci", "Computes the fibonacci series for the specified number"));
		operations.add( Meta.toMetaOperation("factorial", Integer.class, Integer.class,
				"Compute Factorial", "Computes the facorial for the specified number"));

		operations.add( Meta.toMetaOperation("add", 
				new TypeToken<Tuple2<Integer,Integer>>(){}.getType(), Integer.class,
				"Add", "Adds the two specified integers"));
		operations.add( Meta.toMetaOperation("lerp", 
				new TypeToken<Tuple3<Integer,Integer,Double>>(){}.getType(), Double.class,
				"Lerp", "Linear interpolates the first two arguments by the third argument"));

		operations.add( Meta.toMetaOperation("sayHello", null, String.class,
				"Hello Everyone", "Replies hello"));
	}
	
	@Override
	public List<MetaOperation> getOperations() throws TException { return operations; }

	@SuppressWarnings("unchecked")
	@Override
	public Object call(MetaOperation operation, Object input) throws Exception {
		if (operation.getName().equals("openBrowser") 
				|| operation.getName().equals("openVideo")
				|| operation.getName().equals("openAudio")) { launchApp((String)input); return null; }
		if (operation.getName().equals("searchDoodles")) { launchApp("www.google.com/doodles/"); return null; }
		if (operation.getName().equals("sayHello")) return sayHello(input != null ? (String)input : "everyone");
		if (operation.getName().equals("fibonacci")) return fibonacci((Integer)input);
		if (operation.getName().equals("factorial")) return factorial((Integer)input);
		if (operation.getName().equals("add")) return add((Tuple2<Integer,Integer>)input);
		if (operation.getName().equals("lerp")) return lerp((Tuple3<Integer,Integer,Double>)input);
		throw new IllegalArgumentException("Unknown operation");
	}
	
	static void launchApp(String input) throws IOException {
		//String cmdLine = "cmd.exe /c start " + input;
		//Runtime.getRuntime().exec(cmdLine);
		System.out.println("Pretending to start: " + input);
	}
	
	static String sayHello(String name) { 
		return "Hello, " + (name != null ? name : "everyone"); 
	}
	
	static Integer fibonacci(Integer n) {
		if (n < 0)
            throw new IllegalArgumentException("Input value must be greater or equal to zero.");
        if (n == 0)
            return 0;
        if (n == 1)
            return 1;
        return fibonacci(n - 1) + fibonacci(n - 2);
	}

	static Integer factorial(Integer n) {
		if (n < 0)
            throw new IllegalArgumentException("Input value must be greater or equal to zero.");
        if (n == 0)
            return 1;
        return n * factorial(n - 1);
	}
	
	static Integer add(Tuple2<Integer, Integer> input)	{
		return input.Item1 + input.Item2;	
	}

	static Double lerp(Tuple3<Integer, Integer, Double> input) {
		return input.Item1 + (input.Item2 - input.Item1) * input.Item3;
	}
}
