package inc.awesome.metathrift;

import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.apache.thrift.TException;

public class DynamicMetaService extends AbstractMetaService {

    private final Map<MetaOperationEqualsAdapter, Function<Object, Object>> registeredOperations = 
    		new HashMap<MetaOperationEqualsAdapter, Function<Object, Object>>();

	public DynamicMetaService(String name, String displayName, String description)
	{
		super(name, displayName, description);
	}
	
    //------------------------- overrides --------------------------------------------------
	@Override
    public List<MetaOperation> getOperations() throws TException {
		List<MetaOperation> operations = new ArrayList<MetaOperation>();
		for (MetaOperationEqualsAdapter a : registeredOperations.keySet()) 
			operations.add(a.getOperation());
		return operations;
	}

	@Override
	public Object call(MetaOperation operation, Object input) throws Exception {
		MetaOperationEqualsAdapter key = new MetaOperationEqualsAdapter(operation);
		Function<Object, Object> f = registeredOperations.get(key);
		return f.call(input);
	}
	
    //------------------------- actions --------------------------------------------------
	/**
	 * Registers an action.
	 * 
	 * @param name			the action name
	 * @param clazz			the input type token
	 * @param action		the operation to execute
	 * @param displayName	the display name. Can be null
	 * @param description	the description. Can be null
	 */
	public <TInput> void registerAction(String name, Class<TInput> clazz, 
			final Action<TInput> action, String displayName, String description)
	{
		register( Meta.toMetaOperation(name, 
				clazz, null, displayName, description), 
				action);
	}

	/**
	 * Registers an action.
	 * 
	 * @param name			the action name
	 * @param type			the input type token
	 * @param action		the operation to execute
	 * @param displayName	the display name. Can be null
	 * @param description	the description. Can be null
	 */
	public <TInput> void registerAction(String name, Type type, 
			final Action<TInput> action, String displayName, String description)
	{
		register( Meta.toMetaOperation(name,
				type, null, displayName, description), 
				action);
	}

	/**
	 * Registers a parameterless action.
	 * 
	 * @param name			the action name
	 * @param action		the operation to execute
	 * @param displayName	the display name. Can be null
	 * @param description	the description. Can be null
	 */
	public void registerAction(String name, 
			final VoidAction action, String displayName, String description)
	{
		register( Meta.toMetaOperation(name, 
				null, null, displayName, description), 
				action);
	}

	/**
	 * Unregisters the action specified by name and type.
	 * 
	 * @param name	the action name
	 * @param type	the input type token
	 */
	public void unregisterAction(String name, Type type)
	{
		unregister(Meta.toMetaOperation(name, type, null, null, null));
	}

	/**
	 * Unregisters a parameterless action specified by name.
	 * 
	 * @param name	the action name
	 */
	public void unregisterAction(String name)
	{
		unregister(Meta.toMetaOperation(name, null, null, null, null));
	}
	
    //------------------------- functions --------------------------------------------------
	/**
	 * Registers a function.
	 * 
	 * @param name			the function name
	 * @param clazzIn		the input type token
	 * @param clazzOut		the output type token
	 * @param function		the operation to execute
	 * @param displayName	the display name. Can be null
	 * @param description	the description. Can be null
	 */
	public <TInput, TOutput> void registerFunc(String name, 
			Class<TInput> clazzIn, Class<TOutput> clazzOut,
			final Function<TInput, TOutput> function,
			String displayName, String description) 
	{
		register(Meta.toMetaOperation(name, 
				clazzIn, clazzOut, displayName, description), 
				function);
	}

	/**
	 * Registers a function.
	 * 
	 * @param name			the function name
	 * @param typeIn		the input type token
	 * @param typeOut		the output type token
	 * @param function		the operation to execute
	 * @param displayName	the display name. Can be null
	 * @param description	the description. Can be null
	 */
	public <TInput, TOutput> void registerFunc(String name, 
			Type typeIn, Type typeOut,
			final Function<TInput, TOutput> function,
			String displayName, String description) 
	{
		register(Meta.toMetaOperation(name, 
				typeIn, typeOut, displayName, description), 
				function);
	}

	/**
	 * Registers a parameterless function.
	 * 
	 * @param name			the function name
	 * @param clazzOut		the output type token
	 * @param function		the operation to execute
	 * @param displayName	the display name. Can be null
	 * @param description	the description. Can be null
	 */
	public <TOutput> void registerFunc(String name, 
			Class<TOutput> clazzOut,
			final VoidFunction<TOutput> function,
			String displayName, String description) 
	{
		register(Meta.toMetaOperation(name, 
				null, clazzOut, displayName, description), 
				function);
	}

	/**
	 * Registers a parameterless function.
	 * 
	 * @param name			the function name
	 * @param typeOut		the output type token
	 * @param function		the operation to execute
	 * @param displayName	the display name. Can be null
	 * @param description	the description. Can be null
	 */
	public <TOutput> void register(String name, 
			Type typeOut,
			final VoidFunction<TOutput> function,
			String displayName, String description) 
	{
		register(Meta.toMetaOperation(name, 
				null, typeOut, displayName, description), 
				function);
	}

	/**
	 * Unregisters the function specified by name and input/output type.
	 * 
	 * @param name		the function name
	 * @param typeIn	the input type
	 * @param typeOut	the output type
	 */
	public void unregisterFunc(String name, Type typeIn, Type typeOut)
	{
		unregister(Meta.toMetaOperation(name, typeIn, typeOut, null, null));
	}

	/**
	 * Unregisters the parameterless function specified by name and output type.
	 * 
	 * @param name		the function name
	 * @param typeOut	the output type token
	 */
	public void unregisterFunc(String name, Type typeOut)
	{
		unregister(Meta.toMetaOperation(name, null, typeOut, null, null));
	}
	
    //------------------------- privates------------------------------------------------
	private <TInput> void register(MetaOperation operation, final Action<TInput> action)
	{
		MetaOperationEqualsAdapter key = new MetaOperationEqualsAdapter(operation);
		Function<Object,Object> value = new Function<Object,Object>() {
			@SuppressWarnings("unchecked") @Override
			public Object call(Object input) throws Exception { action.call((TInput)input); return null; }
		};
		registeredOperations.put(key, value);
	}

	private void register(MetaOperation operation, final VoidAction action)
	{
		MetaOperationEqualsAdapter key = new MetaOperationEqualsAdapter(operation);
		Function<Object,Object> value = new Function<Object,Object>() {
			@Override
			public Object call(Object input) throws Exception { action.call(); return null; }
		};
		registeredOperations.put(key, value);
	}

	private <TInput, TOutput> void register(MetaOperation operation, 
			final Function<TInput, TOutput> function) 
	{
		MetaOperationEqualsAdapter key = new MetaOperationEqualsAdapter(operation);
		Function<Object, Object> value = new Function<Object, Object>() {
			@SuppressWarnings("unchecked") @Override
			public Object call(Object input) throws Exception { return function.call((TInput)input); }
		};
		registeredOperations.put(key, value);
	}
	
	private <TOutput> void register(MetaOperation operation, 
			final VoidFunction<TOutput> function) 
	{
		MetaOperationEqualsAdapter key = new MetaOperationEqualsAdapter(operation);
		Function<Object, Object> value = new Function<Object, Object>() {
			@Override
			public Object call(Object input) throws Exception { return function.call(); }
		};
		registeredOperations.put(key, value);
	}
	
	private void unregister(MetaOperation operation)
	{
		registeredOperations.remove(new MetaOperationEqualsAdapter(operation));
	}
}