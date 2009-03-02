namespace Otis.Tests.Entity
{
	public class GenericEntity<T>
	{
		private T _id;
		private int? _nullableProperty;

		public T Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public int? NullableProperty
		{
			get { return _nullableProperty; }
			set { _nullableProperty = value; }
		}
	}
}