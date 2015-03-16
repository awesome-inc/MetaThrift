package inc.awesome.metathrift;

public interface Action<TInput> 
{
	void call(TInput input) throws Exception;
}
