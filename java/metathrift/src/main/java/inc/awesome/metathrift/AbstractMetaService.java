package inc.awesome.metathrift;

import java.lang.reflect.Type;
import java.util.List;

import org.apache.thrift.TException;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public abstract class AbstractMetaService implements MetaService.Iface {

	static final Logger log = LoggerFactory.getLogger(AbstractMetaService.class);

    private final String name;
	private final String displayName;
	private final String description;
	
	protected AbstractMetaService(String name, String displayName, String description)
	{
		this.name = name;
		this.displayName = displayName;
		this.description = description;
	}
	
	@Override public String getName() { return name; }
	@Override public String getDisplayName() { return displayName; }
	@Override public String getDescription() { return description; }
	@Override public void ping() {}
	public abstract List<MetaOperation> getOperations() throws TException;
	public abstract Object call(MetaOperation operation, Object input) throws Exception;

	@Override
	public MetaObject call(MetaOperation operation, MetaObject metaObject) 
    		throws TException
    {
		verifyOperation(operation);
        Object input = verifyInput(operation.getInputTypeName(), metaObject);
        Object output = verifiedCall(operation, input);
        return verifyOutput(operation.getOutputTypeName(), output);
    }

    private void verifyOperation(MetaOperation operation) throws TException {
		 for (MetaOperation o : getOperations()) 
			 if (Meta.safeEquals(o, operation)) 
				 return;
		 
		 String msg = "The specified operation is not served by this instance: "
				 + operation.getOutputTypeName() + " " + operation.getName() + "(" + operation.getInputTypeName() + ")";
		 log.warn(msg);
		 throw new ArgumentException(msg);
     }
	
    private static Object verifyInput(String typeName, MetaObject metaObject) throws TException 
	{
        if (!Meta.safeEquals(typeName, metaObject.getTypeName()))
        {
            String msg = String.format("The operation input type mismatches the type of the specified input: "
            		+ typeName + " != " + metaObject.getTypeName());
            log.warn(msg);
            throw new ArgumentException(msg);
        }

        try // try accessing the input value
        {
            return Meta.fromMetaObject(metaObject);
        }
        catch (Exception ex)
        {
            String msg = "The specified value could not be deserialized as the specified type: " + metaObject.getTypeName();
            log.warn(msg + ": " + ex);
            throw new ArgumentException(msg);
        }
	}
	
    private static MetaObject verifyOutput(String typeName, Object value) throws TException {
         try
         {
             Type type = Meta.toType(typeName);
             return Meta.toMetaObject(value, type);
         }
         catch (Exception ex)
         {
             String msg = "The specified value could not be deserialized to the specified output type: " + typeName;
             log.warn(msg +": " + ex);
             throw new ArgumentException(msg);
         }
     }
	
	
	private Object verifiedCall(MetaOperation operation, Object input) throws ServiceException {
		try
        {
        	return call(operation, input);
        }
        catch (Exception ex)
        {
        	log.warn("The operation failed: " + ex);
            throw new ServiceException(String.format("The operation failed: " + ex.getMessage()));
        }
	}
}
