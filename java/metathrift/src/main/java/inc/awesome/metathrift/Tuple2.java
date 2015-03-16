package inc.awesome.metathrift;

public class Tuple2<T1, T2>
{
	public T1 Item1;
	public T2 Item2;
	
	public Tuple2() { }
	public Tuple2(T1 item1, T2 item2) 
	{
		this.Item1 = item1;
		this.Item2 = item2;
	}
	
	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result + ((Item1 == null) ? 0 : Item1.hashCode());
		result = prime * result + ((Item2 == null) ? 0 : Item2.hashCode());
		return result;
	}
	
	@SuppressWarnings("unchecked")
	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (getClass() != obj.getClass())
			return false;
		Tuple2<T1,T2> other = (Tuple2<T1,T2>) obj;
		if (Item1 == null) {
			if (other.Item1 != null)
				return false;
		} else if (!Item1.equals(other.Item1))
			return false;
		if (Item2 == null) {
			if (other.Item2 != null)
				return false;
		} else if (!Item2.equals(other.Item2))
			return false;
		return true;
	}
} 
