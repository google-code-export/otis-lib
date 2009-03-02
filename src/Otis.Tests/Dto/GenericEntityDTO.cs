namespace Otis.Tests.Dto
{
	public class XmlGenericEntityDTO
	{
		private int _id;
		private int? _nullableProperty;

		public int Id
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