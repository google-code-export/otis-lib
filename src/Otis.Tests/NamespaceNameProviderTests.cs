using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Otis.Cfg;
using Otis.Tests.Dto;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class NamespaceNameProviderTests
	{
		[Test]
		public void Default_Namespace_Name_Provider_No_Namespace_Provided()
		{
			Configuration cfg1 = new Configuration();
			cfg1.AddXmlFile("XmlMappings\\mappings.xml");
			cfg1.BuildAssemblers();

			Configuration cfg2 = new Configuration();
			cfg2.AddXmlFile("XmlMappings\\mappings.xml");
			cfg2.BuildAssemblers();

			IAssembler<UserDTO, User> assembler1 = cfg1.GetAssembler<IAssembler<UserDTO, User>>();
			IAssembler<UserDTO, User> assembler2 = cfg2.GetAssembler<IAssembler<UserDTO, User>>();

			Type assembler1Type = assembler1.GetType();
			Type assembler2Type = assembler2.GetType();

			Assert.IsNotNull(assembler1Type.Namespace);
			Assert.IsNotEmpty(assembler1Type.Namespace);
			Assert.IsNotNull(assembler2Type.Namespace);
			Assert.IsNotEmpty(assembler2Type.Namespace);
			Assert.That(assembler1Type.Namespace, Is.Not.EqualTo(assembler2Type.Namespace));
		}

		[Test]
		public void Default_Namespace_Name_Provider_Namespace_Provided()
		{
			const string Namespace = "ManualNamespace";

			Configuration cfg = new Configuration();
			cfg.AddXmlFile("XmlMappings\\mappings.xml");
			cfg.GenerationOptions.Namespace = Namespace;
			cfg.BuildAssemblers();

			IAssembler<UserDTO, User> assembler = cfg.GetAssembler<IAssembler<UserDTO, User>>();

			Assert.That(assembler.GetType().Namespace, Is.EqualTo(Namespace));
		}

		[Test]
		public void Custom_Namespace_Name_Provider_Generates_Correct_Namespace_When_None_Provided()
		{
			Configuration cfg = new Configuration();
			cfg.AddXmlFile("XmlMappings\\mappings.xml");
			cfg.GenerationOptions.NamespaceNameProvider = typeof (CustomNamespaceNameProvider).AssemblyQualifiedName;
			cfg.BuildAssemblers();

			IAssembler<UserDTO, User> assembler = cfg.GetAssembler<IAssembler<UserDTO, User>>();

			Assert.That(assembler.GetType().Namespace, Is.EqualTo(CustomNamespaceNameProvider.AutoGenNamespace));
		}

		[Test]
		public void Custom_Namespace_Name_Provider_Generates_Correct_Namespace_When_Provided()
		{
			const string Namespace = "ManualNamespace";

			Configuration cfg = new Configuration();
			cfg.AddXmlFile("XmlMappings\\mappings.xml");
			cfg.GenerationOptions.NamespaceNameProvider = typeof(CustomNamespaceNameProvider).AssemblyQualifiedName;
			cfg.GenerationOptions.Namespace = Namespace;
			cfg.BuildAssemblers();

			IAssembler<UserDTO, User> assembler = cfg.GetAssembler<IAssembler<UserDTO, User>>();

			Assert.That(assembler.GetType().Namespace, Is.EqualTo(Namespace));
		}

		[Test]
		[ExpectedException(typeof(OtisException), ExpectedMessage = "Namespace Name : \"\", is not valid")]
		public void Custom_Namespace_Name_Provider_Without_Generate_On_Null_Or_Empty_Fails()
		{
			Configuration cfg = new Configuration();
			cfg.AddXmlFile("XmlMappings\\mappings.xml");
			cfg.GenerationOptions.NamespaceNameProvider = typeof(CustomNamespaceNameProviderNoAutoGenOnEmptyOrNull).AssemblyQualifiedName;
			cfg.BuildAssemblers();
		}
	}

	public class CustomNamespaceNameProvider : DefaultNamespaceNameProvider
	{
		public const string AutoGenNamespace = "AutoGenNamespace";

		#region Overrides of OtisNamespaceNameProvider

		protected override void Generate()
		{
			_namespace = AutoGenNamespace;
		}

		#endregion
	}

	public class CustomNamespaceNameProviderNoAutoGenOnEmptyOrNull : DefaultNamespaceNameProvider
	{
		public CustomNamespaceNameProviderNoAutoGenOnEmptyOrNull()
		{
			_shouldAutoGenNamespaceNameWhenNullOrEmpty = false;
		}
	}
}