using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Otis.Cfg;
using Otis.Tests.Dto;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class InheritanceMappingTests
	{
		private DerivedUser _source;

		public IAssembler<DerivedUserDTO, DerivedUser> CreateAttributedAssembler()
		{
			Configuration cfg = new Configuration();
			cfg.AddType(typeof(DerivedUserDTO));
			cfg.AddType<ProjectDTO>(); // todo: should this be automatically detected
			cfg.AddType<DocumentDTO>(); // todo: should this be automatically detected
			cfg.AddType<TaskDTO>(); // todo: should this be automatically detected
			return cfg.GetAssembler<IAssembler<DerivedUserDTO,DerivedUser>>();
		}

		public IAssembler<DerivedXmlUserDTO, DerivedUser> CreateXmlAssembler()
		{
			Configuration cfg = new Configuration();
			cfg.AddAssemblyResources(Assembly.GetAssembly(typeof(DerivedXmlUserDTO)),"otis.xml");
			return cfg.GetAssembler<IAssembler<DerivedXmlUserDTO,DerivedUser>>();
		}

		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			_source = new DerivedUser();
			_source.FirstName = "Nikolay";
			_source.LastName = "Paraskov";
			_source.Age = 34;
			_source.Id = 1;
			_source.UserName = "nparaskov";
			_source.DerivedProperty = "derived1";
		}

		[Test]
		public void TestInheritanceAttributes()
		{
			IAssembler<DerivedUserDTO, DerivedUser> asm = CreateAttributedAssembler();
			DerivedUserDTO userDto =  asm.AssembleFrom(_source);
			Assert.That(userDto.FullName, Is.EqualTo(_source.FirstName + " " + _source.LastName));
			Assert.That(userDto.Age,Is.EqualTo(_source.Age));
			Assert.That(userDto.DerivedProperty,Is.EqualTo(_source.DerivedProperty));
		}

		[Test]
		public void TestInheritanceXml()
		{
			IAssembler<DerivedXmlUserDTO, DerivedUser> asm = CreateXmlAssembler();
			DerivedXmlUserDTO userDto = asm.AssembleFrom(_source);
			Assert.That(userDto.FullName, Is.EqualTo(_source.FirstName + " " + _source.LastName));
			Assert.That(userDto.Age, Is.EqualTo(_source.Age));
			Assert.That(userDto.DerivedProperty, Is.EqualTo(_source.DerivedProperty));
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
