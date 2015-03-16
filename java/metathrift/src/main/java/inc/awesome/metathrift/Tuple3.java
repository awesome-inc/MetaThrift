package inc.awesome.metathrift;

public class Tuple3<T1, T2, T3>
{
	public T1 Item1;
	public T2 Item2;
	public T3 Item3;
	
	public Tuple3() { }
	public Tuple3(T1 item1, T2 item2, T3 item3) 
	{
		this.Item1 = item1;
		this.Item2 = item2;
		this.Item3 = item3;
	}
	
	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result + ((Item1 == null) ? 0 : Item1.hashCode());
		result = prime * result + ((Item2 == null) ? 0 : Item2.hashCode());
		result = prime * result + ((Item3 == null) ? 0 : Item3.hashCode());
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
		Tuple3<T1,T2,T3> other = (Tuple3<T1,T2,T3>) obj;
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
		if (Item3 == null) {
			if (other.Item3 != null)
				return false;
		} else if (!Item3.equals(other.Item3))
			return false;
		return true;
	}
} 
