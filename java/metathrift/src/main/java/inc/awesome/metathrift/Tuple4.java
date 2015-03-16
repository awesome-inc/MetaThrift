package inc.awesome.metathrift;

public class Tuple4<T1, T2, T3, T4>
{
	public T1 Item1;
	public T2 Item2;
	public T3 Item3;
	public T4 Item4;
	
	public Tuple4() { }
	public Tuple4(T1 item1, T2 item2, T3 item3, T4 item4) 
	{
		this.Item1 = item1;
		this.Item2 = item2;
		this.Item3 = item3;
		this.Item4 = item4;
	}
	
	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result + ((Item1 == null) ? 0 : Item1.hashCode());
		result = prime * result + ((Item2 == null) ? 0 : Item2.hashCode());
		result = prime * result + ((Item3 == null) ? 0 : Item3.hashCode());
		result = prime * result + ((Item4 == null) ? 0 : Item4.hashCode());
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
		Tuple4<T1,T2,T3,T4> other = (Tuple4<T1,T2,T3,T4>) obj;
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
		if (Item4 == null) {
			if (other.Item4 != null)
				return false;
		} else if (!Item4.equals(other.Item4))
			return false;
		return true;
	}
} 
