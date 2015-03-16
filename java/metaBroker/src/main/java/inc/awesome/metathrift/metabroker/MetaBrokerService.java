package inc.awesome.metathrift.metabroker;

import inc.awesome.metathrift.Action;
import inc.awesome.metathrift.ArgumentException;
import inc.awesome.metathrift.Broker;
import inc.awesome.metathrift.CanceledException;
import inc.awesome.metathrift.Function;
import inc.awesome.metathrift.MetaBroker.Iface;
import inc.awesome.metathrift.MetaObject;
import inc.awesome.metathrift.MetaOperation;
import inc.awesome.metathrift.MetaService;
import inc.awesome.metathrift.MetaServiceInfo;
import inc.awesome.metathrift.ServiceException;
import inc.awesome.metathrift.Thrift;
import inc.awesome.metathrift.VoidFunction;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentMap;

import org.apache.thrift.TException;
import org.apache.thrift.transport.TTransportException;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class MetaBrokerService implements Iface {

	static final Logger log = LoggerFactory.getLogger(MetaBrokerService.class);
    private final ConcurrentMap<String, ServiceContext> services = new ConcurrentHashMap<String, ServiceContext>();
	private final String name;
	private final String displayName;
	private final String description;
	
    public MetaBrokerService(String name, String displayName, String description)
    {
    	if (name == null || name.equals("")) throw new IllegalArgumentException("Must specifiy a valid service name");
		this.name = name;
		this.displayName = displayName != null ? displayName : name;
		this.description = description != null ? description : "";
    }

    public MetaBrokerService() {
    	this("MetaBroker", null, null);
    }
    // ----------------------------------- overrides --------------------------------------------------
	public String getName() throws ServiceException, TException { return name; }
	public String getDisplayName() throws ServiceException, TException { return displayName; }
	public String getDescription() throws ServiceException, TException { return description; }
	public void ping() throws TException {}

	public List<MetaOperation> getOperations() throws TException {
		return handledCall(new VoidFunction<List<MetaOperation>>() {
			public List<MetaOperation> call() throws Exception {
				List<MetaOperation> allOperations = new ArrayList<MetaOperation>();
				for (ServiceContext serviceContext : services.values())
					allOperations.addAll( Broker.wrap(serviceContext.getService().getOperations(), serviceContext.info.getName()));
				return allOperations;
			}
		}, "Could not get operations");
	}

	public MetaObject call(MetaOperation func, MetaObject input)
			throws ArgumentException, ServiceException, CanceledException, TException {
		return handledCall(new Function2<MetaOperation, MetaObject, MetaObject>() {
			public MetaObject call(MetaOperation o, MetaObject i) throws Exception {
				return services.get(Broker.getServiceName(o)).getService().call(Broker.unwrap(o), i);
			}
		}, func, input, "Could not call operation");
	}

	public void bind(MetaServiceInfo serviceInfo) throws ArgumentException,
			ServiceException, TException 
	{
        handledCall(new Action<MetaServiceInfo>() {
			public void call(MetaServiceInfo info) throws Exception {
	            String serviceName = info.getName();
	            ServiceContext serviceContext = new ServiceContext(info);
	            services.put(serviceName, serviceContext);
	        }
		}, serviceInfo, "Could not bind service");
	}

	public void unbind(String serviceName) throws ArgumentException,
			ServiceException, TException 
	{
        handledCall(new Action<String>() {
			public void call(String key) throws Exception { services.remove(key);	}
		}, serviceName, "Could not unbind service");

	}

	public List<MetaServiceInfo> getInfos() throws ServiceException, TException 
	{
		return handledCall(new VoidFunction<List<MetaServiceInfo>>() {
			public List<MetaServiceInfo> call() throws Exception {
				List<MetaServiceInfo> allServices = new ArrayList<MetaServiceInfo>();
				for (ServiceContext serviceContext : services.values())
					allServices.add(serviceContext.getInfo());
				return allServices;
			}
		}, "Could not get services");
	}

	public MetaServiceInfo getInfo(String serviceName) throws ServiceException, TException 
	{
		return handledCall(new Function<String, MetaServiceInfo>() {
			public MetaServiceInfo call(String key) throws Exception {
				return services.get(key).getInfo();
			}
		}, serviceName, "Could not get service");
	}
	
	// ----------------------------------- privates --------------------------------------
	private class ServiceContext
    {
        private MetaServiceInfo info;
        private MetaService.Iface service;

		public MetaServiceInfo getInfo() { return info; }
        public MetaService.Iface getService() { return service; }

        public ServiceContext(MetaServiceInfo serviceInfo) throws TTransportException
        {
            info = serviceInfo;
            service = new MetaService.Client(Thrift.createProtocol(serviceInfo.getUrl()));
        }
    }
	
    private static <TInput> void handledCall(Action<TInput> action, TInput input, 
    		String couldNot) throws ServiceException
    {
        try
        {
            action.call(input);
        }
        catch (Exception ex)
        {
        	log.error(couldNot + ": " + ex);
        	throw buildException(couldNot + ": " + ex.getMessage());
        }
    }

	private static <TOutput> TOutput handledCall(VoidFunction<TOutput> func, 
			String couldNot) throws ServiceException
    {
        try
        {
            return func.call();
        }
        catch (Exception ex)
        {
        	log.error(couldNot + ": " + ex);
        	throw buildException(couldNot + ": " + ex.getMessage());
        }
    }

    private static <TInput, TOutput> TOutput handledCall(Function<TInput, TOutput> func,
    		TInput input, String couldNot) throws ServiceException
    {
        try
        {
            return func.call(input);
        }
        catch (Exception ex)
        {
        	log.error(couldNot + ": " + ex);
        	throw buildException(couldNot + ": " + ex.getMessage());
        }
    }
	
    private static <TInput1, TInput2, TOutput> TOutput handledCall(Function2<TInput1, TInput2, TOutput> func,
    		TInput1 input1, TInput2 input2, 
    		String couldNot) throws ServiceException
    {
        try
        {
            return func.call(input1, input2);
        }
        catch (Exception ex)
        {
        	log.error(couldNot + ": " + ex);
        	throw buildException(couldNot + ": " + ex.getMessage());
        }
    }

    private static ServiceException buildException(String reason) {
    	ServiceException ex = new ServiceException();
    	ex.setReason(reason);
    	return ex;
	}
}
