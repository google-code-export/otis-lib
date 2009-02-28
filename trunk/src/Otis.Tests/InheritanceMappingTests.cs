using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class InheritanceMappingTests
	{
		private DerivedUser m_source;

		public IAssembler<DerivedUserDTO, DerivedUser> CreateAttributedAssembler()
		{
			Configuration cfg = new Configuration();
			cfg.AddType(typeof(DerivedUserDTO));
			cfg.AddType<ProjectDTO>(); // todo: should this be automatically detected
			cfg.AddType<DocumentDTO>(); // todo: should this be automatically detected
			cfg.AddType<TaskDTO>(); // todo: should this be automatically detected
			return cfg.GetAssembler<DerivedUserDTO, DerivedUser>();
		}

		public IAssembler<DerivedXmlUserDTO, DerivedUser> CreateXmlAssembler()
		{
			Configuration cfg = new Configuration();
			cfg.AddAssemblyResources(Assembly.GetAssembly(typeof(DerivedXmlUserDTO)),"otis.xml");
			return cfg.GetAssembler<DerivedXmlUserDTO, DerivedUser>();
		}

		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			m_source = new DerivedUser();
			m_source.FirstName = "Nikolay";
			m_source.LastName = "Paraskov";
			m_source.Age = 34;
			m_source.Id = 1;
			m_source.UserName = "nparaskov";
			m_source.DerivedProperty = "derived1";
		}

		[Test]
		public void TestInheritanceAttributes()
		{
			IAssembler<DerivedUserDTO, DerivedUser> asm = CreateAttributedAssembler();
			DerivedUserDTO userDto =  asm.AssembleFrom(m_source);
			Assert.That(userDto.FullName, Is.EqualTo(m_source.FirstName + " " + m_source.LastName));
			Assert.That(userDto.Age,Is.EqualTo(m_source.Age));
			Assert.That(userDto.DerivedProperty,Is.EqualTo(m_source.DerivedProperty));
		}

		[Test]
		public void TestInheritanceXml()
		{
			IAssembler<DerivedXmlUserDTO, DerivedUser> asm = CreateXmlAssembler();
			DerivedXmlUserDTO userDto = asm.AssembleFrom(m_source);
			Assert.That(userDto.FullName, Is.EqualTo(m_source.FirstName + " " + m_source.LastName));
			Assert.That(userDto.Age, Is.EqualTo(m_source.Age));
			Assert.That(userDto.DerivedProperty, Is.EqualTo(m_source.DerivedProperty));
		}

		[Test]
		public void CodeGen()
		{
			Configuration cfg = new Configuration();
			cfg.AddAssemblyResources(Assembly.GetAssembly(typeof(DerivedXmlUserDTO)), "otis.xml");
			cfg.GenerationOptions.OutputType = OutputType.SourceCode;
			cfg.GenerationOptions.OutputFile = "nixonAssembler.cs";
			cfg.BuildAssemblers();
		}
	}
}
