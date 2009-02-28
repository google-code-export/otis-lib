using System;
using System.Collections.Generic;
using System.Text;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[MapClass(typeof(User), Helper = "Otis.Tests.Util.Convert")]
	public class MappingHelperDTO_Duplicate
	{
		[Map("$Id")] public int Id;

		[MappingHelper]
		public void Helper(ref MappingHelperDTO_Duplicate dto, ref User user)
		{ }
	}

	[MapClass(typeof(User))]
	public class MappingHelperDTO_NonPublic
	{
		[Map("$Id")] public int Id;
		[MappingHelper] void Helper(ref MappingHelperDTO_NonPublic dto, ref User user){}
	}

	[MapClass(typeof(User))]
	public class MappingHelperDTO_InstanceHelper
	{
		[Map("$Id")] public int Id;
		public string FullName;

		[MappingHelper]
		public void Helper(ref MappingHelperDTO_InstanceHelper dto, ref User user)
		{
			dto.FullName = "custom_mapping_InstanceHelper";
		}
	}

	[MapClass(typeof(User), Helper = "Otis.Tests.MappingHelperTestConverter.Convert")]
	public class MappingHelperDTO_StaticHelper
	{
		[Map("$Id")] public int Id;
		public string FullName;
	}

	[MapClass(typeof(User))]
	public class MappingHelperDTO_CheckOrder
	{
		[Map("$Id")]
		public int Id;

		[MappingHelper]
		public void Helper(ref MappingHelperDTO_CheckOrder dto, ref User user)
		{
			dto.Id = -100;
		}
	}

	public class MappingHelperTestConverter
	{
		public static void Convert(ref MappingHelperDTO_StaticHelper dto, ref User user)
		{
			dto.FullName = "custom_mapping_StaticHelper";	
		}
	}


}
