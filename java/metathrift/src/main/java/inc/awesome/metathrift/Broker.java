package inc.awesome.metathrift;

import java.util.ArrayList;
import java.util.Collection;

public final class Broker 
{
	private Broker() { }
	
    public static String getServiceName(MetaOperation operation)
    {
        return getServiceName(operation.getName());
    }

    public static MetaOperation wrap(MetaOperation operation, String serviceName)
    {
    	MetaOperation wrappedOperation = new MetaOperation(operation);
        wrappedOperation.setName( serviceName + "/" + operation.getName());
        return wrappedOperation;
    }

    public static MetaOperation unwrap(MetaOperation operation)
    {
    	MetaOperation unwrappedOperation = new MetaOperation(operation);
        unwrappedOperation.setName( operation.getName().substring(operation.getName().indexOf('/')+1));
        return unwrappedOperation;
    }

    public static Collection<MetaOperation> wrap(Collection<MetaOperation> operations, String serviceName)
    {
    	Collection<MetaOperation> wrappedOperations = new ArrayList<MetaOperation>();
    	for (MetaOperation op : operations)
    		wrappedOperations.add(wrap(op, serviceName));
    	return wrappedOperations;
    }
    
    public static Collection<MetaOperation> unwrap(Collection<MetaOperation> operations)
    {
    	Collection<MetaOperation> unwrappedOperations = new ArrayList<MetaOperation>();
    	for (MetaOperation op : operations)
    		unwrappedOperations.add(unwrap(op));
    	return unwrappedOperations;
    }
    
    static String getServiceName(String operationName)
    {
        return operationName.split("/")[0];
    }
}
