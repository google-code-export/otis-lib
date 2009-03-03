/*
 * Created by: Zdeslav Vojkovic
 * Created: Saturday, September 29, 2007
 */

using NUnit.Framework;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class AssemblyGenerationTests
	{
		[Test]
		[ExpectedException(typeof(OtisException))]
		public void Compilation_Fails_With_Invalid_Mapping_Expression()
		{
			Configuration cfg = new Configuration();
			cfg.AddType<InvalidDTO>();
			cfg.BuildAssemblers();
		}
	}

	[MapClass(typeof(User))]
	public class InvalidDTO
	{
		[Map("$NonExisitingProperty")]
		public int Id;
	}
}

