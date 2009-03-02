using Otis.Tests.Entity;

namespace Otis.Tests.Dto
{
	[MapClass(typeof(GenericEntity<int>))]
	public class AttributedGenericEntityDTO
	{
		private int _id;
		private int? _nullableProperty;

		[Map]
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[Map]
		public int? NullableProperty
		{
			get { return _nullableProperty; }
			set { _nullableProperty = value; }
		}
	}
}