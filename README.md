# MetaThrift
Development is slowed down when changing contract [IDLs](https://thrift.apache.org/docs/idl) is discouraged, e.g. because the code generation maybe a fragile, non-automated process. An intermediate solution is to decouple the contract interfaces (API) from the transport protocol so changing the API becomes independent from changing the IDL. We did just that by introducing a meta IDL that supports a generic call pattern. The basic idea is that 

1. operations are called by name and
2. arguments and results are transparently serialized in a generic fashion.

How does that look like? Here is an example:

	MetaService.Client service = ...
	Integer result = call(service, "fibonacci", 5, Integer.class, Integer.class);

This calls the `fibonacci`-operation with the argument `5`. In .NET:

	MetaService.Iface service = ...
	var result = service.Call<int, int>("fibonacci", 5);

## Supported Features

- Actions (no return) and Functions with generic argument and return types including `void` parameters (no parameter)
- Generic serialization via [JSON](http://de.wikipedia.org/wiki/JavaScript_Object_Notation)
- Well-known types like `void, integral types, string, floats, decimal, date/time`
- Custom types by namespace-qualified name; overridable with explicit type registration
- Generic collections like `array<T>, list<T>, map<TKey,TValue>` 
- Tuples for up to four items like `tuple<T1,T2,T3,T4>` 

## Implementing a server
To implement a server subclass the `AbstractMetaService` or use the `DynamicMetaService` and register some operations. Here is an example copied from the unit tests:

**.NET**

    var service = new DynamicMetaService("MyName", "MyDisplayName", "MyDescription");

    service.RegisterAction<string>("openBrowser", uri => Process.Start(uri), "Open Browser", "Opens a web browser with the specified uri");

	service.RegisterAction("searchDoodles", () => Process.Start("http://www.google.com/doodles/"), "Search Doodles", "Opens the Google Doodle page");

    service.RegisterFunc<Tuple<int, int>, int>("add", t => t.Item1+t.Item2, "Add", "Adds two integers");

**.Java**

    final DynamicMetaService service = new DynamicMetaService("MyName",
		"MyDisplayName", "MyDescription");

	service.registerAction("openBrowser", String.class,
		new Action<String>() {
			public void call(String input) throws IOException { 
				launchApp(input); 
			}
		},
		"Open Browser", "Opens a web browser with the specified url");

	service.registerAction("searchDoodles", 
		new VoidAction() {
			public void call() throws Exception { 
				launchApp("http://www.google.com/doodles/"); 
			}
		}, 
		"Search Doodles", "Opens the Google Doodle page");

	service.registerFunc("fibonacci", Integer.class, Integer.class,
		new Function<Integer, Integer>() {
			public Integer call(Integer input) throws Exception { 
				return 	fibonacci(input); 
			}
		}, 
		"Fibonacci", "Computes the Fibonacci series for the specified number");
