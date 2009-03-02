using Otis.Tests.Entity;

namespace Otis.Tests.Dto
{
	[MapClass(typeof(User), Preparer = "Otis.Tests.Dto.Util.Convert")]
	public class MappingPreparerDTO_Duplicate
	{
		[Map("$Id")]
		public int Id;

		[MappingPreparer]
		public void Preparer(ref MappingPreparerDTO_Duplicate dto, ref User user)
		{ }
	}

	[MapClass(typeof(User))]
	public class MappingPreparerDTO_NonPublic
	{
		[Map("$Id")]
		public int Id;
		[MappingPreparer]
		void Preparer(ref MappingPreparerDTO_NonPublic dto, ref User user) { }
	}

	[MapClass(typeof(User))]
	public class MappingPreparerDTO_InstancePreparer
	{
		[Map("$Id")]
		public int Id;
		public string FullName;

		[MappingPreparer]
		public void Preparer(ref MappingPreparerDTO_InstancePreparer dto, ref User user)
		{
			dto.FullName = "custom_mapping_InstancePreparer";
		}
	}

	[MapClass(typeof(User), Preparer = "Otis.Tests.Dto.MappingPreparerTestConverter.Convert")]
	public class MappingPreparerDTO_StaticPreparer
	{
		[Map("$Id")]
		public int Id;
		public string FullName;
	}

	[MapClass(typeof(User))]
	public class MappingPreparerDTO_CheckOrder
	{
		[Map("$Id")]
		public int Id;

		[MappingPreparer]
		public void Preparer(ref MappingPreparerDTO_CheckOrder dto, ref User user)
		{
			dto.Id = -1;
		}
	}

	public class MappingPreparerTestConverter
	{
		public static void Convert(ref MappingPreparerDTO_StaticPreparer dto, ref User user)
		{
			dto.FullName = "custom_mapping_StaticPreparer";
		}
	}
}