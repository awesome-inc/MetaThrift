package inc.awesome.metathrift;

import java.lang.reflect.ParameterizedType;
import java.lang.reflect.Type;
import java.math.BigDecimal;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.google.gson.Gson;
import com.google.gson.internal.*;

public final class Meta 
{
	private static final MetaObject emptyInput = new MetaObject();

	private static final String arrayPrefix="array<";
	private static final String listPrefix="list<";
	private static final String mapPrefix="map<";
	private static final String tuplePrefix="tuple<";
	
	private static final Gson gson = new Gson();
	private static final Map<String, Type> wellKnownTypes = new HashMap<String, Type>();
	private static final Map<Type, String> wellKnownTypeNames = new HashMap<Type, String>();
	private static final Map<String, Type> registeredTypes = new HashMap<String, Type>();
	private static final Map<Type, String> registeredTypeNames = new HashMap<Type, String>();
	private static final Map<String, Type> typeCache = new HashMap<String, Type>();
	
	static 
	{
        wellKnownTypes.put("boolean", Boolean.class);
        wellKnownTypes.put("byte", Byte.class);
            
        wellKnownTypes.put("date", Date.class);
        wellKnownTypes.put("decimal", BigDecimal.class);
        wellKnownTypes.put("double", Double.class);
        wellKnownTypes.put("string", String.class);

        wellKnownTypes.put("float", Float.class);
        wellKnownTypes.put("int", Integer.class);
        wellKnownTypes.put("long", Long.class);
        wellKnownTypes.put("short", Short.class);

        for (Map.Entry<String, Type> item : wellKnownTypes.entrySet())
        	wellKnownTypeNames.put(item.getValue(), item.getKey());
    }
	
	private Meta() { }
	
	// ----------------------- call (action) -----------------------------------------
	/**
	 * Calls an action.
	 * 
	 * @param service	the instance serving the action
	 * @param name		the action name
	 * @param input		the action input
	 * @param clazz		the input type token
	 * @throws Exception
	 */
	public static <TInput> void call(MetaService.Iface service, 
			String name, TInput input, Class<TInput> clazz) throws Exception {
		MetaOperation action = new MetaOperation(name);
		action.setInputTypeName(toTypeName(clazz));
		service.call(action, toMetaObject(input, clazz));
	}

	/**
	 * Calls an action with generic input types.
	 * 
	 * @param service	the instance serving the action
	 * @param name		the action name
	 * @param input		the action input
	 * @param type		the input type token
	 * @throws Exception
	 */
	public static void call(MetaService.Iface service, 
			String name, Object input, Type type) throws Exception {
		MetaOperation action = new MetaOperation(name);
		action.setInputTypeName(toTypeName(type));
		service.call(action, toMetaObject(input, type));
	}
	
	/**
	 * Calls a void action.
	 * 
	 * @param service	the instance serving the action
	 * @param name		the action name
	 * @throws Exception
	 */
	public static void call(MetaService.Iface service, String name) throws Exception {
		MetaOperation action = new MetaOperation(name);
		service.call(action, emptyInput);
	}

	// ----------------------- call (function) -----------------------------------------
	/**
	 * Calls a function.
	 * 
	 * @param service	the instance serving the function
	 * @param name		the function name
	 * @param input		the function input
	 * @param clazzIn	the input type token
	 * @param clazzOut	the output type token
	 * @return the function result
	 * @throws Exception
	 */
	@SuppressWarnings("unchecked")
	public static <TInput, TOutput> TOutput call(MetaService.Iface service, 
			String name, TInput input, Class<TInput> clazzIn, Class<TOutput> clazzOut)
					throws Exception {
		MetaOperation func = new MetaOperation(name);
		func.setInputTypeName(toTypeName(clazzIn));
		func.setOutputTypeName(toTypeName(clazzOut));
		MetaObject metaInput = toMetaObject(input, clazzIn);
		MetaObject metaOutput = service.call(func, metaInput);
		return (TOutput)fromMetaObject(metaOutput);
	}

	/**
	 * Calls a function with generic types for both input and output.
	 * 
	 * @param service	the instance serving the function
	 * @param name		the function name
	 * @param input		the function input
	 * @param typeIn	the input type token
	 * @param typeOut	the output type token
	 * @return the function result
	 * @throws Exception
	 */
	@SuppressWarnings("unchecked")
	public static <TInput, TOutput> TOutput call(MetaService.Iface service, 
			String name, TInput input, Type typeIn, Type typeOut)
					throws Exception {
		MetaOperation func = new MetaOperation(name);
		func.setInputTypeName(toTypeName(typeIn)); 
		func.setOutputTypeName(toTypeName(typeOut));
		MetaObject metaInput = toMetaObject(input, typeIn);
		MetaObject metaOutput = service.call(func, metaInput);
		return (TOutput)fromMetaObject(metaOutput);
	}

	/**
	 * Calls a function with generic input.
	 * 
	 * @param service	the instance serving the function
	 * @param name		the function name
	 * @param input		the function input
	 * @param typeIn	the input type token
	 * @param clazzOut	the output type token
	 * @return the function result
	 * @throws Exception
	 */
	@SuppressWarnings("unchecked")
	public static <TOutput> TOutput call(MetaService.Iface service, 
			String name, Object input, Type typeIn, Class<TOutput> clazzOut)
					throws Exception {
		MetaOperation func = new MetaOperation(name);
		func.setInputTypeName(toTypeName(typeIn));
		func.setOutputTypeName(toTypeName(clazzOut));
		MetaObject metaInput = toMetaObject(input, typeIn);
		MetaObject metaOutput = service.call(func, metaInput);
		return (TOutput)fromMetaObject(metaOutput);
	}
	
	/**
	 * Calls a function with generic output.
	 * 
	 * @param service	the instance serving the function
	 * @param name		the function name
	 * @param input		the function input
	 * @param clazzIn	the input type token
	 * @param typeOut	the output type token
	 * @return the function result
	 * @throws Exception
	 */
	@SuppressWarnings("unchecked")
	public static <TInput, TOutput> TOutput call(MetaService.Iface service, 
			String name, TInput input, Class<TInput> clazzIn, Type typeOut)
					throws Exception {
		MetaOperation func = new MetaOperation(name); 
		func.setInputTypeName(toTypeName(clazzIn)); 
		func.setOutputTypeName(toTypeName(typeOut));
		MetaObject metaInput = toMetaObject(input, clazzIn);
		MetaObject metaOutput = service.call(func, metaInput);
		return (TOutput)fromMetaObject(metaOutput);
	}

	/**
	 * Calls a void function.
	 * 
	 * @param service	the instance serving the function
	 * @param name		the function name
	 * @param clazzOut	the output type token
	 * @return the function result
	 * @throws Exception
	 */
	@SuppressWarnings("unchecked")
	public static <TOutput> TOutput call(MetaService.Iface service, 
			String name, Class<TOutput> clazzOut) throws Exception {
		MetaOperation func = new MetaOperation(name);
		func.setOutputTypeName(toTypeName(clazzOut));
		MetaObject metaOutput = service.call(func, emptyInput);
		return (TOutput)fromMetaObject(metaOutput);
	}

	/**
	 * Calls a void function with a generic output type.
	 * 
	 * @param service	the instance serving the function
	 * @param name		the function name
	 * @param clazzOut	the output type token
	 * @return the function result
	 * @throws Exception
	 */
	@SuppressWarnings("unchecked")
	public static <TOutput> TOutput call(MetaService.Iface service, 
			String name, Type typeOut) throws Exception {
		MetaOperation func = new MetaOperation(name); 
		func.setOutputTypeName(toTypeName(typeOut));
		MetaObject metaOutput = service.call(func, emptyInput);
		return (TOutput)fromMetaObject(metaOutput);
	}

	// ----------------------- MetaOperation -----------------------------------------
	/**
	 * Creates a MetaOperation.
	 * 
	 * @param name			the operation name
	 * @param typeIn		the input type. Can be null (parameterless).
	 * @param typeOut		the output type. Can be null (void)
	 * @param displayName	an optional display name. Can be null
	 * @param description	an optional description. Can be null
	 * @return	the created action
	 */
    public static MetaOperation toMetaOperation(String name, 
    		Type typeIn, Type typeOut, 
    		String displayName, String description)
    {
    	MetaOperation operation = new MetaOperation(name);
    	if (typeIn != null) operation.setInputTypeName(toTypeName(typeIn));
    	if (typeOut != null) operation.setOutputTypeName(toTypeName(typeOut));
    	if (displayName != null) operation.setDisplayName(displayName);
    	if (description != null) operation.setDescription(description);
    	return operation;
    }
    
	public static String prettyPrint(MetaOperation operation) {
        return 
        		(!isNullOrEmpty(operation.getOutputTypeName()) ? operation.getOutputTypeName() : "void")
        		+ " " + operation.getName() + "("
        		+ (!isNullOrEmpty(operation.getInputTypeName()) ? operation.getInputTypeName() + " value" : "")
        		+ ");"
                + (!isNullOrEmpty(operation.getDescription()) ? " // " + operation.getDescription() : "")
                ;
	}

	private static boolean isNullOrEmpty(String value) {
		return value == null || value.equals("");
	}
	
	public static void registerType(Type type, String typeName) {
        if (type == null) throw new IllegalArgumentException("type");
        if (isNullOrEmpty(typeName)) throw new IllegalArgumentException("typeName");

        if (wellKnownTypes.containsKey(typeName))
            throw new IllegalArgumentException("A well known type name must not be used when registering custom types.");
        if (wellKnownTypeNames.containsKey(type))
            throw new IllegalArgumentException("A well known type must not be used when registering custom types.");
        if (registeredTypes.containsKey(typeName))
            throw new IllegalArgumentException("A type with the specified name has already been registered.");
        if (registeredTypeNames.containsKey(type))
            throw new IllegalArgumentException("The specified type has already been registered.");

        registeredTypes.put(typeName, type);
        registeredTypeNames.put(type, typeName);
	
	}

	public static void unregisterType(Type type) {
        if (type == null) throw new IllegalArgumentException("null type specified.");
        String typeName = registeredTypeNames.get(type);
        if (typeName == null)
        	throw new IllegalArgumentException("The specified type has not been registered as a custom type.");
        registeredTypeNames.remove(type);
        registeredTypes.remove(typeName);
	}
	
	// ------------ MetaObject (package scope, test only) -------------

	public static String prettyPrint(MetaObject value)
    {
        return "{type=" + value.getTypeName() + ", value=" + value.getData() + "}";
    }
	
	static <T> MetaObject toMetaObject(T value, Class<T> clazz) 
	{
		MetaObject metaObject = new MetaObject();
		metaObject.setTypeName(toTypeName(clazz));
		metaObject.setData(gson.toJson(value));
		return metaObject;
	}

	static <T> MetaObject toMetaObject(T[] value, Class<T[]> clazz) 
	{
		MetaObject metaObject = new MetaObject();
		metaObject.setTypeName(toTypeName(clazz));
		metaObject.setData(gson.toJson(value));
		return metaObject;
	}

	static MetaObject toMetaObject(Object value, Type type) 
	{
		MetaObject metaObject = new MetaObject();
		metaObject.setTypeName(toTypeName(type));
		metaObject.setData(gson.toJson(value));
		return metaObject;
	}
	
	static Object fromMetaObject(MetaObject metaObject) 
	{
		String typeName = metaObject.getTypeName();
		Type type = toType(typeName);
		if (type != null)
			return gson.fromJson( metaObject.getData(), type );
		throw new IllegalArgumentException("Unknown type: " + typeName);
	}

	// ---------------- package scope (test only) --------------------
	static String toTypeName(Type type) 
	{
		if (type == null || type.equals(void.class)) return "";
		
		if (type instanceof Class)
		{
			Class<?> clazz = (Class<?>)type;
			if (clazz.isArray())
				return arrayPrefix + toTypeName(clazz.getComponentType()) + ">";
			else
				return toTypeName((Class<?>)type);
		}
		
		ParameterizedType parameterizedType = (ParameterizedType)type;
		Class<?> rawType = (Class<?>)parameterizedType.getRawType();
		Type[] actualTypeArguments = parameterizedType.getActualTypeArguments();

		if (rawType == List.class)
			return listPrefix + toTypeName((Class<?>)actualTypeArguments[0]) + ">";
		
		if (rawType == Map.class)
			return mapPrefix 
					+ toTypeName((Class<?>)actualTypeArguments[0]) + ","
					+ toTypeName((Class<?>)actualTypeArguments[1]) 
					+ ">";

		if (rawType == Tuple2.class)
			return tuplePrefix 
					+ toTypeName((Class<?>)actualTypeArguments[0]) + ","
					+ toTypeName((Class<?>)actualTypeArguments[1]) 
					+ ">";
		if (rawType == Tuple3.class)
			return tuplePrefix 
					+ toTypeName((Class<?>)actualTypeArguments[0]) + ","
					+ toTypeName((Class<?>)actualTypeArguments[1]) + ","
					+ toTypeName((Class<?>)actualTypeArguments[2]) 
					+ ">";
		if (rawType == Tuple4.class)
			return tuplePrefix 
					+ toTypeName((Class<?>)actualTypeArguments[0]) + ","
					+ toTypeName((Class<?>)actualTypeArguments[1]) + ","
					+ toTypeName((Class<?>)actualTypeArguments[2]) + ","
					+ toTypeName((Class<?>)actualTypeArguments[3]) 
					+ ">";
		
		// should not reach here
		return null;
	}
	
	private static <TClass> String toTypeName(Class<TClass> clazz) 
	{
	    String typeName = wellKnownTypeNames.get(clazz);
	    if (typeName != null) return typeName;

	    typeName = registeredTypeNames.get(clazz);
	    if (typeName != null) return typeName;
	    
	    if (clazz.isArray())
	    	return arrayPrefix + toTypeName(clazz.getComponentType()) + ">";
	    
	    //return clazz.getCanonicalName();
	    return clazz.getName();
	}

	static Type toType(String typeName) 
	{
		if (typeName == null || typeName.equals("")) return void.class;
			
		if (typeName.startsWith(arrayPrefix))
    	{
    		String argTypeName = typeName.substring(arrayPrefix.length(), typeName.length()-1);
    		return $Gson$Types.arrayOf(toType(argTypeName));
    	}
		
		if (typeName.startsWith(listPrefix))
    	{
    		String argTypeName = typeName.substring(listPrefix.length(), typeName.length()-1);
    		return $Gson$Types.newParameterizedTypeWithOwner(null, List.class, new Type[] { toType(argTypeName) });
    	}

    	if (typeName.startsWith(mapPrefix))
    	{
    		String[] argTypeNames = typeName.substring(mapPrefix.length(), typeName.length()-1).split(",");
    		return $Gson$Types.newParameterizedTypeWithOwner(null, Map.class, toTypes(argTypeNames));
    	}

    	if (typeName.startsWith(tuplePrefix))
    	{
    		String[] argTypeNames = typeName.substring(tuplePrefix.length(), typeName.length()-1).split(",");
    		switch(argTypeNames.length)
    		{
	    		case 2: return $Gson$Types.newParameterizedTypeWithOwner(null, Tuple2.class, toTypes(argTypeNames)); 
	    		case 3: return $Gson$Types.newParameterizedTypeWithOwner(null, Tuple3.class, toTypes(argTypeNames)); 
	    		case 4: return $Gson$Types.newParameterizedTypeWithOwner(null, Tuple4.class, toTypes(argTypeNames)); 
    		}
    	}
    	
	    Type type = wellKnownTypes.get(typeName);
	    if (type != null) return type;

	    type = registeredTypes.get(typeName);
	    if (type != null) return type;
	    
	    type = typeCache.get(typeName);
	    if (type != null) return type;
	
	    try 
	    {
	    	type = Class.forName(typeName);
	    	typeCache.put(typeName, type);
	    	return type;
	    } 
	    catch(ClassNotFoundException cEx) 
	    {
	    	return null;
	    }
	}
	
    static boolean safeEquals(MetaOperation op1, MetaOperation op2)
    {
    	return op1.getName().equals(op2.getName())
		 && safeEquals(op1.getInputTypeName(),op2.getInputTypeName())
		 && safeEquals(op1.getOutputTypeName(), op2.getOutputTypeName());
    }

    static boolean safeEquals(String str1, String str2) {
        return (str1 == null ? str2 == null : str1.equals(str2));
    }
	
	// ----------------------- privates -----------------------
	private static Type[] toTypes(String[] typeNames) {
		Type[] types = new Type[typeNames.length];
		for (int i = 0; i < typeNames.length; i++)
			types[i] = toType(typeNames[i]);
		return types;
	}
}
