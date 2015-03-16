package inc.awesome.metathrift;

import inc.awesome.metathrift.MetaService.Iface;

import org.apache.thrift.TException;
import org.junit.Test;

public class AbstractMetaServiceTest {

	final Iface service = new MockService();
	
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
}
