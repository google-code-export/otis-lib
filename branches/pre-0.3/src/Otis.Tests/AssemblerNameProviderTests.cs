/*
 * Created by: joe.garro
 * Created: 2009-03-02
 */

using System;
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
		public void Custom_Assembler_Provider_Is_Injectable()
		{
			Configuration cfg = new Configuration();
			cfg.AddAssemblyResources(Assembly.GetExecutingAssembly(), "mappings.xml");
			cfg.GenerationOptions.DefaultAssemblerBase.AssemblerNameProviderName =
				typeof(CustomAssemblerNameProvider).AssemblyQualifiedName;
			cfg.BuildAssemblers();

			IAssemblerNameProvider provider = cfg.GenerationOptions.DefaultAssemblerBase.AssemblerNameProvider;

			Assert.IsNotNull(provider);
			Assert.That(provider.GetType().AssemblyQualifiedName, Is.EqualTo(typeof(CustomAssemblerNameProvider).AssemblyQualifiedName));
		}

		[Test]
		[ExpectedException(typeof(OtisException), 
			ExpectedMessage = "Error Loading AssemblerNameProvider. Unable to Create AssemblerNameProvider. See inner exception for details.")]
		public void Custom_Assembler_Provider_Does_Not_Implement_Interface_Should_Fail()
		{
			Configuration cfg = new Configuration();
			cfg.AddAssemblyResources(Assembly.GetExecutingAssembly());
			cfg.GenerationOptions.DefaultAssemblerBase.AssemblerNameProviderName =
				typeof (InvalidAssemblerNameProvider).AssemblyQualifiedName;
			cfg.BuildAssemblers();
		}

		[Test]
		public void Default_Assembler_Provider_Is_Correctly_Instantiated()
		{
			Configuration cfg = new Configuration();
			cfg.AddAssemblyResources(Assembly.GetExecutingAssembly());
			cfg.BuildAssemblers();

			IAssemblerNameProvider provider = cfg.GenerationOptions.DefaultAssemblerBase.AssemblerNameProvider;

			Assert.IsNotNull(provider);
			Assert.That(provider.GetType().AssemblyQualifiedName, Is.EqualTo(typeof(AssemblerNameProvider).AssemblyQualifiedName));
		}

		[Test]
		public void Default_Assembler_Provider_Can_Retrieve_By_Types()
		{
			Configuration cfg = new Configuration();
			cfg.AddAssemblyResources(Assembly.GetExecutingAssembly());
			cfg.BuildAssemblers();

			string actual = cfg.AssemblerManager.GetAssemblerName(typeof(XmlUserDTO), typeof(User), typeof(IAssembler<,>));
			const string expected = "UserToXmlUserDTOAssembler";

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public void Default_Assembler_Provider_Can_Retrieve_By_Assembler()
		{
			Configuration cfg = new Configuration();
			cfg.AddAssemblyResources(Assembly.GetExecutingAssembly());
			cfg.BuildAssemblers();

			string actual = cfg.AssemblerManager.GetAssemblerName<IAssembler<XmlUserDTO, User>>();
			const string expected = "UserToXmlUserDTOAssembler";

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public void Default_Assembler_Provider_Can_Retrieve_By_NamedAssembler()
		{
			Configuration cfg = new Configuration();
			cfg.AddAssemblyResources(Assembly.GetExecutingAssembly());
			cfg.AddXmlFile("XmlMappings\\named_assembler_mappings.xml");
			cfg.BuildAssemblers();

			string actual = cfg.AssemblerManager.GetAssemblerName<IAssembler<NamedAssemblerUserDTO,User>>();
			const string expected = "NamedAssemblerFromUserDtoToUser";

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public void Configuration_Can_Retrieve_Auto_Named_Assembler()
		{
			Configuration cfg = new Configuration();
			cfg.AddAssemblyResources(Assembly.GetExecutingAssembly());
			cfg.BuildAssemblers();

			string actual = cfg.GetAssembler<IAssembler<XmlUserDTO, User>>().GetType().Name;
			const string expected = "UserToXmlUserDTOAssembler";

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public void Configuration_Can_Retrieve_Manual_Named_Assembler()
		{
			Configuration cfg = new Configuration();
			cfg.AddAssemblyResources(Assembly.GetExecutingAssembly());
			cfg.AddXmlFile("XmlMappings\\named_assembler_mappings.xml");
			cfg.BuildAssemblers();

			string actual = cfg.GetAssembler<IAssembler<NamedAssemblerUserDTO, User>>().GetType().Name;
			const string expected = "NamedAssemblerFromUserDtoToUser";

			Assert.That(actual, Is.EqualTo(expected));
		}
	}

	public class CustomAssemblerNameProvider : IAssemblerNameProvider 
	{
		#region Implementation of IAssemblerNameProvider

		public string GenerateName(Type target, Type source)
		{
			return String.Format("Custom{0}To{1}Assembler", source.Name, target.Name);
		}

		public string GenerateName<TargetType, SourceType>()
		{
			return GenerateName(typeof (TargetType), typeof (SourceType));
		}

		#endregion
	}

	public class InvalidAssemblerNameProvider {}
}