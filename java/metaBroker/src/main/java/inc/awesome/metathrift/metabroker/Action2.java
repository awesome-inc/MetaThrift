package inc.awesome.metathrift.metabroker;

public interface Action2<TInput1, TInput2> 
{
	void call(TInput1 input1, TInput2 input2) throws Exception;
}
