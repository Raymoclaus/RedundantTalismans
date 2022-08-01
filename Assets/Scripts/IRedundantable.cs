namespace RedundantTalismans
{
	public interface IRedundantable<T>
	{
		bool IsRedundantTo(T other);
	}
}