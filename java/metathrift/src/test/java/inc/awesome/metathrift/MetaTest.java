package inc.awesome.metathrift;

import inc.awesome.metathrift.Meta;
import inc.awesome.metathrift.Tuple2;
import inc.awesome.metathrift.Tuple3;
import inc.awesome.metathrift.Tuple4;

import java.lang.reflect.Type;
import java.math.BigDecimal;
import java.math.RoundingMode;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Arrays;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import static org.junit.Assert.*;

import org.junit.Ignore;
import org.junit.Test;

import com.google.gson.reflect.TypeToken;

public class MetaTest {

    static final SimpleDateFormat sdf = new SimpleDateFormat("dd.MM.yyyy HH:mm:ss");

	@Test
	public void toFromType_should_support_void() 
	{
		testType(void.class, "");
	}

	@Test
	public void toFromType_should_support_primitive_types() 
	{
		testType(Boolean.class, "boolean");
        testType(Byte.class, "byte");
        testType(String.class, "string");
        testType(Date.class, "date");
        testType(BigDecimal.class, "decimal");
        testType(Double.class, "double");
        testType(Float.class, "float");
        testType(Integer.class, "int");
        testType(Long.class, "long");
        testType(Short.class, "short");
	}

    @Test
    public void toFromType_should_support_inner_classes()
    {
    	testType(InnerClass.class, "inc.awesome.metathrift.MetaTest$InnerClass");
    }
	
	@Test
	public void toFromType_should_support_arrays_of_primitive_types() 
	{
        testType(Boolean[].class, "array<boolean>");
		testType(Byte[].class, "array<byte>");
        testType(String[].class, "array<string>");
        testType(Date[].class, "array<date>");
        testType(BigDecimal[].class, "array<decimal>");
        testType(Double[].class, "array<double>");
        testType(Float[].class, "array<float>");
        testType(Integer[].class, "array<int>");
        testType(Long[].class, "array<long>");
        testType(Short[].class, "array<short>");
	}

	@Ignore("component types of inner class array have a different type name than the inner class itself")
	@Test 
    public void toFromType_should_support_arrays_of_inner_classes()
    {
    	testType(InnerClass[].class, "array<inc.awesome.metathrift.MetaTest$InnerClass>");
    }

	@Ignore("gson TypeToken() does not support arrays of generics")
	@Test
	public void toFromType_should_support_arrays_of_lists() 
	{
        testType(new TypeToken<List<Integer>[]>(){}.getType(), "array<list<int>>");
	}

	@Ignore("gson TypeToken() does not support arrays of generics")
	@Test
	public void toFromType_should_support_arrays_of_maps() 
	{
        testType(new TypeToken<Map<Integer,String>[]>(){}.getType(), "array<maps<int,string>>");
	}
	
	@Ignore("gson TypeToken() does not support arrays of generics")
	@Test
	public void toFromType_should_support_arrays_of_tuples() 
	{
        testType(new TypeToken<Tuple2<Integer,String>[]>(){}.getType(), "array<tuple<int,string>>");
	}
	
	@Test
	public void toFromType_should_support_lists_of_primitive_types() 
	{
        testType(new TypeToken<List<Boolean>>(){}.getType(), "list<boolean>");
		testType(new TypeToken<List<Byte>>(){}.getType(), "list<byte>");
        testType(new TypeToken<List<String>>(){}.getType(), "list<string>");
        testType(new TypeToken<List<Date>>(){}.getType(), "list<date>");
        testType(new TypeToken<List<BigDecimal>>(){}.getType(), "list<decimal>");
        testType(new TypeToken<List<Double>>(){}.getType(), "list<double>");
        testType(new TypeToken<List<Float>>(){}.getType(), "list<float>");
        testType(new TypeToken<List<Integer>>(){}.getType(), "list<int>");
        testType(new TypeToken<List<Long>>(){}.getType(), "list<long>");
        testType(new TypeToken<List<Short>>(){}.getType(), "list<short>");
	}

	@Test 
    public void toFromType_should_support_lists_of_inner_classes()
    {
    	testType(new TypeToken<List<InnerClass>>(){}.getType(), "list<inc.awesome.metathrift.MetaTest$InnerClass>");
    }

	@Ignore("gson TypeToken() does not support nested generics")
	@Test 
    public void toFromType_should_support_lists_of_lists()
    {
    	testType(new TypeToken<List<List<Integer>>>(){}.getType(), "list<list<int>>");
    }

	@Ignore("gson TypeToken() does not support nested generics")
	@Test 
    public void toFromType_should_support_lists_of_maps()
    {
    	testType(new TypeToken<List<Map<Integer,String>>>(){}.getType(), "list<map<int,string>>");
    }
	
	@Ignore("gson TypeToken() does not support nested generics")
	@Test 
    public void toFromType_should_support_lists_of_tuples()
    {
    	testType(new TypeToken<List<Tuple2<Integer,String>>>(){}.getType(), "list<tuple<int,string>>");
    }
	
	@Test
	public void toFromType_should_support_maps_of_primitive_types() 
	{
        testType(new TypeToken<Map<String,String>>(){}.getType(), "map<string,string>");
        testType(new TypeToken<Map<Integer,String>>(){}.getType(), "map<int,string>");
        testType(new TypeToken<Map<String,Integer>>(){}.getType(), "map<string,int>");
        testType(new TypeToken<Map<Integer,Integer>>(){}.getType(), "map<int,int>");
	}

	@Ignore("gson TypeToken() does not support nested generics")
	@Test 
    public void toFromType_should_support_maps_of_lists()
    {
    	testType(new TypeToken<Map<Integer,List<Integer>>>(){}.getType(), "map<int,list<int>>");
    }

	@Ignore("gson TypeToken() does not support nested generics")
	@Test 
    public void toFromType_should_support_maps_of_maps()
    {
    	testType(new TypeToken<Map<Integer,Map<Integer,String>>>(){}.getType(), "map<int,map<int,string>>");
    }
	
	@Test 
    public void toFromType_should_support_maps_of_inner_classes()
    {
    	testType(new TypeToken<Map<Integer,InnerClass>>(){}.getType(), "map<int,inc.awesome.metathrift.MetaTest$InnerClass>");
    }
	
	@Test
	public void toFromType_should_support_tuples_of_primitive_types() 
	{
        testType(new TypeToken<Tuple2<Integer,Integer>>(){}.getType(), "tuple<int,int>");
        testType(new TypeToken<Tuple3<Integer,String,Double>>(){}.getType(), "tuple<int,string,double>");
        testType(new TypeToken<Tuple4<Integer,String,Double,Float>>(){}.getType(), "tuple<int,string,double,float>");
	}

	@Test
	public void toFromMetaObject_should_support_primitive_types() throws ParseException 
	{
        testSerialization(true, Boolean.class);
        testSerialization((byte)-4, Byte.class);

        testSerialization(sdf.parse("05.02.1979 14:30:12"), Date.class);
        
        testSerialization(new BigDecimal(1).divide(new BigDecimal(3), 8, RoundingMode.HALF_UP), BigDecimal.class);
        testSerialization(0.5, Double.class);
        testSerialization("The quick brown fox jumped over the lazy dog", String.class);
        testSerialization(0.5f, Float.class);
        testSerialization(4, Integer.class);
        testSerialization((long)44, Long.class);
        testSerialization((short)-44, Short.class);
	}
	
    @Test
    public void toFromMetaObject_should_support_inner_classes()
    {
    	testSerialization(new InnerClass(
    			"http://www.heise.de",
    			new Byte[] { 0, 0, 0, -128, 0, 0, 0, -128, 0, 0, 0, -128, -128, -128, -128 }),
    			InnerClass.class);
    }
    
    @Test
    public void toFromMetaObject_should_support_arrays_of_primitive_types() throws ParseException
    {
        testSerialization(new Byte[] { 1, 2, 3, 4 }, Byte[].class);
        testSerialization(new Boolean[] { true, false, true, false }, Boolean[].class);
        testSerialization(new Date[] { sdf.parse("05.02.1979 14:30:12"), sdf.parse("05.02.1979 18:30:12") }, Date[].class);
        testSerialization(new BigDecimal[] { new BigDecimal(1), new BigDecimal(3) }, BigDecimal[].class);
        testSerialization(new Double[] { 0.1, 0.5, 1D, 20D, 320D }, Double[].class);
        testSerialization("The quick brown fox jumped over the lazy dog".split(" "), String[].class);
        testSerialization(new Float[] { 0.1f, 0.5f, 1f, 20f, 320f }, Float[].class);
        testSerialization(new Integer[] { 1, 2, 3, 4 }, Integer[].class);
        testSerialization(new Long[] { 1L, 2L, 3L, 4L}, Long[].class);
        testSerialization(new Short[] { 1, 2, 3, 4 }, Short[].class);    
    }

	@Test
    public void toFromMetaObject_should_support_lists_of_primitive_types() throws ParseException
    {
        testSerialization(Arrays.asList(true, false, true, false), new TypeToken<List<Boolean>>(){}.getType());
        testSerialization(Arrays.asList((byte)1, (byte)2, (byte)3, (byte)4), new TypeToken<List<Byte>>(){}.getType());
        testSerialization(Arrays.asList("The","quick","brown","fox","jumped","over","the","lazy","dog"), new TypeToken<List<String>>(){}.getType());
        testSerialization(Arrays.asList(sdf.parse("05.02.1979 14:30:12"), sdf.parse("05.02.1979 18:30:12")), new TypeToken<List<Date>>(){}.getType());
        testSerialization(Arrays.asList(new BigDecimal(1), new BigDecimal(3)), new TypeToken<List<BigDecimal>>(){}.getType());
        testSerialization(Arrays.asList(0.1, 0.5, 1D, 20D, 320D),  new TypeToken<List<Double>>(){}.getType());
        testSerialization(Arrays.asList(0.1f, 0.5f, 1f, 20f, 320f), new TypeToken<List<Float>>(){}.getType());
        testSerialization(Arrays.asList(1, 2, 3, 4), new TypeToken<List<Integer>>(){}.getType());
        testSerialization(Arrays.asList(1L, 2L, 3L, 4L), new TypeToken<List<Long>>(){}.getType());
        testSerialization(Arrays.asList((short)1, (short)2, (short)3, (short)4), new TypeToken<List<Short>>(){}.getType());
    }
	
	@Test
	public void toFromMetaObject_should_support_maps_of_primitive_types()
	{
    	Map<String, String> map1 = new HashMap<String, String>();
        map1.put("1", "The");
        map1.put("2", "quick");
        map1.put("3", "brown");
        map1.put("4", "fox");
        map1.put("5", "jumped");
        map1.put("6", "over");
        map1.put("7", "the");
        map1.put("8", "lazy");
        map1.put("9", "dog");
        testSerialization(map1, new TypeToken<Map<String,String>>(){}.getType());

    	Map<Integer, String> map2 = new HashMap<Integer, String>();
        map2.put(1, "The");
        map2.put(2, "quick");
        map2.put(3, "brown");
        map2.put(4, "fox");
        testSerialization(map2,  new TypeToken<Map<Integer,String>>(){}.getType());
    }
	
	@Test
	public void toFromMetaObject_should_support_tuples_of_primitive_types()
	{
        testSerialization(new Tuple2<String, String>("Hello", "you"), new TypeToken<Tuple2<String,String>>(){}.getType());
        testSerialization(new Tuple2<String, Integer>("Hello", 1), new TypeToken<Tuple2<String,Integer>>(){}.getType());
        testSerialization(new Tuple2<String, Double>("Hello", 0.5), new TypeToken<Tuple2<String,Double>>(){}.getType());
        testSerialization(new Tuple2<Integer, String>(1, "you"), new TypeToken<Tuple2<Integer,String>>(){}.getType());
        testSerialization(new Tuple2<Integer, Integer>(1, 2), new TypeToken<Tuple2<Integer,Integer>>(){}.getType());
        testSerialization(new Tuple2<Double, Double>(0.5, 2.0), new TypeToken<Tuple2<Double,Double>>(){}.getType());
        testSerialization(new Tuple3<Integer, Integer, Integer>(1, 2, 3), new TypeToken<Tuple3<Integer,Integer,Integer>>(){}.getType());
        testSerialization(new Tuple3<Double, Double, Double>(2.0, 4.0, 0.5), new TypeToken<Tuple3<Double,Double,Double>>(){}.getType());
        testSerialization(new Tuple4<Integer, String, Double, Float>(2, "Hello", 4.0, 0.5f), new TypeToken<Tuple4<Integer,String,Double,Float>>(){}.getType());		
	}

	private static void testType(Type type, String typeName)
	{
		assertEquals(typeName, Meta.toTypeName(type));
		
		Type actualType = Meta.toType(typeName);
		if (type instanceof Class && !(actualType instanceof Class))
		{
			Class<?> clazz = (Class<?>)type;
			assertEquals(clazz.getCanonicalName(), Meta.toType(typeName).toString());
		}
		else // both classes or types
			assertEquals(type, actualType);
	}
	
	@SuppressWarnings("unchecked")
	private static <T> void testSerialization(T expected, Class<T> clazz)
    {
        MetaObject m = Meta.toMetaObject(expected, clazz);
        System.out.println(Meta.prettyPrint(m));
        T actual = (T)Meta.fromMetaObject(m);
        assertEquals(expected, actual);
    }
	
	@SuppressWarnings("unchecked")
	private static <T> void testSerialization(T[] expected, Class<T[]> clazz)
    {
        MetaObject m = Meta.toMetaObject(expected, clazz);
        System.out.println(Meta.prettyPrint(m));
        T[] actual = (T[])Meta.fromMetaObject(m);
        assertArrayEquals(expected, actual);
    }
	
	@SuppressWarnings("unchecked")
	private static <T> void testSerialization(T expected, Type type)
    {
        MetaObject m = Meta.toMetaObject(expected, type);
        System.out.println(Meta.prettyPrint(m));
        T actual = (T)Meta.fromMetaObject(m);
        assertEquals(expected, actual);
    }
	
    class InnerClass
    {	
		String uri = "";
    	Byte[] bytes = new Byte[0];
    	
        public String getUri() { return uri; }
        public void setUri(String uri) { this.uri = uri; }
        public Byte[] getBytes() { return bytes; }
        public void setBytes(Byte[] bytes) { this.bytes= bytes ; }

        public InnerClass() {
        }

        public InnerClass(String uri, Byte[] bytes) {
        	this.uri = uri;
        	this.bytes = bytes;
        }
        
        @Override
        public boolean equals(Object other)
        {
        	if ( this == other) return true;
        	if ( !(other instanceof InnerClass) ) return false;
        	InnerClass that = (InnerClass)other;
        	return (uri.equals(that.getUri()) && Arrays.equals(bytes, that.getBytes()));
        }
    }
}
