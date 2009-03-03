/*
 * Created by: joe.garro
 * Created: 2009-03-01
 */

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
		public void Default_Namespace_Provider_Generates_Correct_Namespace_When_None_Provided()
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
		public void Default_Namespace_Provider_Uses_Provided_Namespace()
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
		public void Custom_Namespace_Provider_Is_Injectible()
		{
			Configuration cfg = new Configuration();
			cfg.AddXmlFile("XmlMappings\\mappings.xml");
			cfg.GenerationOptions.NamespaceNameProviderName = typeof(CustomNamespaceNameProvider).AssemblyQualifiedName;
			cfg.BuildAssemblers();

			Assert.IsNotNull(cfg.GenerationOptions.NamespaceNameProvider);
			Assert.That(cfg.GenerationOptions.NamespaceNameProvider.GetType().AssemblyQualifiedName, 
				Is.EqualTo(typeof(CustomNamespaceNameProvider).AssemblyQualifiedName));
		}

		[Test]
		[ExpectedException(typeof(OtisException), 
			ExpectedMessage = "Error Loading NamespaceNameProvider. Unable to Create NamespaceNameProvider. See inner exception for details.")]
		public void Custom_Namespace_Provider_Does_Not_Implement_Interface_Should_Fail()
		{
			Configuration cfg = new Configuration();
			cfg.AddXmlFile("XmlMappings\\mappings.xml");
			cfg.GenerationOptions.NamespaceNameProviderName = typeof(InvalidNamespaceNameProvider).AssemblyQualifiedName;
			cfg.BuildAssemblers();
		}

		[Test]
		public void Custom_Namespace_Provider_Generates_Correct_Namespace_When_None_Provided()
		{
			Configuration cfg = new Configuration();
			cfg.AddXmlFile("XmlMappings\\mappings.xml");
			cfg.GenerationOptions.NamespaceNameProviderName = typeof (CustomNamespaceNameProvider).AssemblyQualifiedName;
			cfg.BuildAssemblers();

			IAssembler<UserDTO, User> assembler = cfg.GetAssembler<IAssembler<UserDTO, User>>();

			Assert.That(assembler.GetType().Namespace, Is.EqualTo(CustomNamespaceNameProvider.AutoGenNamespace));
		}

		[Test]
		public void Custom_Namespace_Provider_Uses_Provided_Namespace()
		{
			const string Namespace = "ManualNamespace";

			Configuration cfg = new Configuration();
			cfg.AddXmlFile("XmlMappings\\mappings.xml");
			cfg.GenerationOptions.NamespaceNameProviderName = typeof(CustomNamespaceNameProvider).AssemblyQualifiedName;
			cfg.GenerationOptions.Namespace = Namespace;
			cfg.BuildAssemblers();

			IAssembler<UserDTO, User> assembler = cfg.GetAssembler<IAssembler<UserDTO, User>>();

			Assert.That(assembler.GetType().Namespace, Is.EqualTo(Namespace));
		}
	}

	public class CustomNamespaceNameProvider : INamespaceNameProvider
	{
		public const string AutoGenNamespace = "AutoGenNamespace";

		#region Implementation of INamespaceNameProvider

		public string GetNamespaceName()
		{
			return AutoGenNamespace;
		}

		#endregion
	}

	public class InvalidNamespaceNameProvider {}
}