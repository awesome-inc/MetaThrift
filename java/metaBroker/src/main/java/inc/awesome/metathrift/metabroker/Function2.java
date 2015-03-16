package inc.awesome.metathrift.metabroker;

public interface Function2<TInput1, TInput2, TOutput> {

	TOutput call(TInput1 input1, TInput2 input2) throws Exception;
}