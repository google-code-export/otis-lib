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
	public class AssemblerFactoryTests
	{
		[Test]
		public void Custom_Assembler_Factory_Is_Injectable()
		{
			Configuration cfg = new Configuration();
			cfg.AddXmlFile("XmlMappings\\mappings.xml");
			cfg.GenerationOptions.AssemblerFactoryProviderName = typeof (CustomAssemblerFactoryProvider).AssemblyQualifiedName;
			cfg.GenerationOptions.PostInstantiate();

			IAssemblerFactoryProvider provider = cfg.GenerationOptions.AssemblerFactoryProvider;

			Assert.IsNotNull(provider);
			Assert.That(provider.GetType().AssemblyQualifiedName, Is.EqualTo(typeof(CustomAssemblerFactoryProvider).AssemblyQualifiedName));
		}

		[Test]
		[ExpectedException(typeof(OtisException),
			ExpectedMessage = "Error Loading AssemblerFactoryProvider. Unable to Create AssemblerFactoryProvider. See inner exception for details.")]
		public void Custom_Assembler_Factory_Does_Not_Implement_Interface_Should_Fail()
		{
			Configuration cfg = new Configuration();
			cfg.AddXmlFile("XmlMappings\\mappings.xml");
			cfg.GenerationOptions.AssemblerFactoryProviderName = typeof (InvalidAssemblerFactoryProvider).AssemblyQualifiedName;
			cfg.GenerationOptions.PostInstantiate();
		}

		[Test]
		public void Default_Assembler_Factory_Is_Correctly_Instantiated()
		{
			Configuration cfg = new Configuration();
			cfg.AddXmlFile("XmlMappings\\mappings.xml");
			cfg.GenerationOptions.PostInstantiate();

			IAssemblerFactoryProvider provider = cfg.GenerationOptions.AssemblerFactoryProvider;

			Assert.IsNotNull(provider);
			Assert.That(provider.GetType().AssemblyQualifiedName, Is.EqualTo(typeof(AssemblerFactoryProvider).AssemblyQualifiedName));
		}

		[Test]
		[ExpectedException(typeof(OtisException), ExpectedMessage = "It is not possible to retrieve Assembler/AssemblerFactory because source code generation is chosen.")]
		public void Default_Assembler_Factory_Not_Available_Because_Source_Code_Generation_Is_Output()
		{
			Configuration cfg = new Configuration();
			cfg.AddXmlFile("XmlMappings\\mappings.xml");
			cfg.GenerationOptions.OutputType = OutputType.SourceCode;
			cfg.GenerationOptions.Namespace = "OtisTest";

			object asm = cfg.GetAssemblerFactory();
		}

		[Test]
		public void Default_Assembler_Factory_Base_Type_Has_Get_Assemblers_By_Name()
		{
			Configuration cfg = new Configuration();
			cfg.AddXmlFile("XmlMappings\\mappings.xml");
			cfg.AddXmlFile("XmlMappings\\named_assembler_mappings.xml");

			object factory = cfg.GetAssemblerFactory();

			Type factoryType = factory.GetType();

			MethodInfo method1 = factoryType.GetMethod("GetUserToXmlUserDTOAssembler");
			MethodInfo method2 = factoryType.GetMethod("GetUserToUserDTOAssembler");
			MethodInfo method3 = factoryType.GetMethod("GetNamedAssemblerFromUserDtoToUser");

			Assert.IsNotNull(method1);
			Assert.IsNotNull(method2);
			Assert.IsNotNull(method3);

			Assert.That(method1.ReturnType, Is.EqualTo(typeof(IAssembler<XmlUserDTO, User>)));
			Assert.That(method2.ReturnType, Is.EqualTo(typeof(IAssembler<UserDTO, User>)));
			Assert.That(method3.ReturnType, Is.EqualTo(typeof(IAssembler<NamedAssemblerUserDTO, User>)));
		}
	}

	public class CustomAssemblerFactoryProvider : AssemblerFactoryProvider {}

	public class InvalidAssemblerFactoryProvider {}
}