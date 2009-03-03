using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Otis.Cfg;
using Otis.Tests.Dto;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class AssemblerNameProviderTests
	{
		[Test]
		public void Custom_Provider_Correctly_Instantiated()
		{
			Assert.Fail();
		}

		[Test]
		public void Custom_Provider_Of_Wrong_Type_Should_Fail()
		{
			Assert.Fail();
		}

		[Test]
		public void Default_Provider_Is_Created_Correctly()
		{
			Configuration cfg = new Configuration();
			cfg.AddAssemblyResources(Assembly.GetExecutingAssembly());
			cfg.BuildAssemblers();

			IAssemblerNameProvider provider = cfg.GenerationOptions.DefaultAssemblerBase.AssemblerNameProvider;

			Assert.IsNotNull(provider);
		}

		[Test]
		public void Default_Provider_Can_Retrieve_By_Types()
		{
			Configuration cfg = new Configuration();
			cfg.AddAssemblyResources(Assembly.GetExecutingAssembly());
			cfg.BuildAssemblers();

			string actual = cfg.AssemblerManager.GetAssemblerName(typeof(XmlUserDTO), typeof(User));
			const string expected = "UserToXmlUserDTOAssembler";

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public void Default_Provider_Can_Retrieve_By_Assembler()
		{
			Configuration cfg = new Configuration();
			cfg.AddAssemblyResources(Assembly.GetExecutingAssembly());
			cfg.BuildAssemblers();

			string actual = cfg.AssemblerManager.GetAssemblerName<IAssembler<XmlUserDTO, User>>();
			const string expected = "UserToXmlUserDTOAssembler";

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public void Default_Provider_Can_Retrieve_By_NamedAssembler()
		{
			Configuration cfg = new Configuration();
			cfg.AddAssemblyResources(Assembly.GetExecutingAssembly());
			cfg.AddXmlFile("XmlMappings\\named_assembler_mappings.xml");
			cfg.BuildAssemblers();

			string actual = cfg.AssemblerManager.GetAssemblerName<NamedAssemblerUserDTO, User>();
			const string expected = "NamedAssemblerFromUserDtoToUser";

			Assert.That(actual, Is.EqualTo(expected));
		}
	}
}