package inc.awesome.metathrift;

public interface Function<TInput, TOutput> {

	TOutput call(TInput input) throws Exception;
}