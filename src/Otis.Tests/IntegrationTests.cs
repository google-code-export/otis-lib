/*
 * Created by: 
 * Created: Friday, September 28, 2007
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class IntegrationTests
	{
		private const string errInvalidTransformation = "Assembler for transformation [Otis.Tests.Entity.User -> System.String] is not configured";
		private const string errSourceCodeGeneration = "It is not possible to retrieve assembler because source code generation is chosen.";
		private const string errInstanceSuppressed = "It is not possible to retrieve assembler because SupressInstanceCreation option is turned on.";

		private User _user;

		[SetUp]
		public void SetUp()
		{
			_user = new User();
			_user.FirstName = "Zdeslav";
			_user.LastName = "Vojkovic";
			_user.Age = 33;
			_user.Id = 1;
			_user.UserName = "zdeslavv";
			_user.Projects.Add(new Project("proj1"));
			_user.Projects.Add(new Project("proj2"));
			_user.Projects.Add(new Project("proj3"));
			_user.UserGender = "M";
		}

		[Test]
		public void Configure_From_Type()
		{
			Configuration cfg = new Configuration();
			cfg.AddType(typeof(AttributedUserDTO));

			IAssembler<AttributedUserDTO, User> asm = cfg.GetAssembler<IAssembler<AttributedUserDTO,User>>();
			AttributedUserDTO dto = asm.AssembleFrom(_user);

			Assert.AreEqual(dto.Id, _user.Id);
			Assert.AreEqual(dto.Age, _user.Age);
			Assert.AreEqual(dto.UserName, _user.UserName.ToUpper());
			Assert.AreEqual(dto.ProjectCount, _user.Projects.Count);
			Assert.AreEqual(dto.FullName, _user.FirstName + " " + _user.LastName);
			Assert.AreEqual(dto.Gender, Gender.Male);
			Assert.AreEqual(dto.GenderCode, "M");


			AttributedUserDTO dto2 = new AttributedUserDTO();
			asm.Assemble(ref dto2, ref _user);

			Assert.AreEqual(dto2.Id, _user.Id);
			Assert.AreEqual(dto2.Age, _user.Age);
			Assert.AreEqual(dto2.UserName, _user.UserName.ToUpper());
			Assert.AreEqual(dto2.ProjectCount, _user.Projects.Count);
		}

		[Test]
		public void Initialize_From_Xml()
		{
			Configuration cfg = new Configuration();
			cfg.AddXmlFile("XmlMappings\\mappings.xml");
			TestXmlConfiguration(cfg);
		}

		[Test]
		public void Helper_Is_Optional_In_Xml_Config()
		{
			Configuration cfg = new Configuration();
			cfg.AddXmlFile("XmlMappings\\mappings2.xml");
			TestXmlConfiguration(cfg);
		}

		[Test]
		public void Initialize_From_Assembly_Resource_Xml()
		{
			Configuration cfg = new Configuration();
			cfg.AddAssemblyResources(Assembly.GetExecutingAssembly(), "otis.xml");
			TestXmlConfiguration(cfg);
		}

		[Test]
		public void Configure_From_Multiple_Sources()
		{
			Configuration cfg = new Configuration();

			cfg.AddType(typeof(AttributedUserDTO));
			cfg.AddXmlFile("XmlMappings\\mappings.xml");

			IAssembler<AttributedUserDTO, User> asm1 = cfg.GetAssembler<IAssembler<AttributedUserDTO,User>>();
			Assert.IsNotNull(asm1);

			IAssembler<XmlUserDTO, User> asm2 = cfg.GetAssembler<IAssembler<XmlUserDTO,User>>();
			Assert.IsNotNull(asm2);
		}

		[Test]
		[ExpectedException(typeof(OtisException), ExpectedMessage = errInvalidTransformation)]
		public void Configuration_Fails_For_Nonexisting_Transformation()
		{
			Configuration cfg = new Configuration();
			cfg.AddType(typeof(AttributedUserDTO));

			IAssembler<string, User> asm = cfg.GetAssembler<IAssembler<string,User>>();
		}

		[Test]
		public void Source_Code_Generation()
		{
			Configuration cfg = new Configuration();
			cfg.GenerationOptions.OutputType = OutputType.SourceCode;
			cfg.GenerationOptions.Namespace = "OtisTest";
			cfg.GenerationOptions.OutputFile = @"src.cs";

			cfg.AddType(typeof(AttributedUserDTO));
			cfg.BuildAssemblers();

			Assert.IsTrue(File.Exists("src.cs"));
			string content = File.ReadAllText("src.cs");
			Assert.IsTrue(content.Length > 100);
			Assert.IsTrue(content.Contains("namespace OtisTest"));
			Assert.IsTrue(content.Contains("public class UserToAttributedUserDTOAssembler : IAssembler<Otis.Tests.AttributedUserDTO, Otis.Tests.Entity.User>"));
			Assert.IsTrue(content.Contains("AssembleFrom(Otis.Tests.Entity.User source)"));
			Assert.IsTrue(content.Contains("return target;"));
			Assert.IsTrue(content.Contains("Assemble(Otis.Tests.AttributedUserDTO target, Otis.Tests.Entity.User source)"));
			Assert.IsTrue(content.Contains("Assemble(ref Otis.Tests.AttributedUserDTO target, ref Otis.Tests.Entity.User source)"));
			Assert.IsTrue(content.Contains("Otis.Tests.AttributedUserDTO target = new Otis.Tests.AttributedUserDTO();"));
			File.Delete("src.cs");
		}

		[Test]
		public void Assembly_Generation()
		{
			Configuration cfg = new Configuration();
			cfg.GenerationOptions.Namespace = "OtisTest";
			cfg.GenerationOptions.OutputType = OutputType.Assembly;
			cfg.GenerationOptions.OutputFile = @"src.dll";

			if(File.Exists("src.dll"))
				File.Delete("src.dll");

			cfg.AddType(typeof(AttributedUserDTO));
			cfg.BuildAssemblers();

			Assert.IsTrue(File.Exists("src.dll"));
			Assembly assembly = Assembly.LoadFrom("src.dll");
			
			Type type = assembly.GetType("OtisTest.UserToAttributedUserDTOAssembler");
			Assert.IsNotNull(type);
			Type[] interfaces = type.GetInterfaces();
			Assert.AreEqual(1, interfaces.Length);
			type = interfaces[0];
			Assert.IsTrue(type.FullName.StartsWith("Otis.IAssembler`"));
			MethodInfo[] methods = type.GetMethods();
			Assert.AreEqual(5, methods.Length);

			Assert.IsTrue(methods[0].Name.StartsWith("AssembleFrom"));
			Assert.IsTrue(methods[1].Name.StartsWith("Assemble"));
			Assert.IsTrue(methods[2].Name.StartsWith("Assemble"));
			Assert.IsTrue(methods[3].Name.StartsWith("ToArray"));
			Assert.IsTrue(methods[4].Name.StartsWith("ToList"));
		}

		[Test]
		public void ProjectionTest()
		{
			Configuration cfg = new Configuration();
			cfg.AddType(typeof(AttributedUserDTO));
			IAssembler<AttributedUserDTO, User> asm = cfg.GetAssembler<IAssembler<AttributedUserDTO,User>>();
			AttributedUserDTO dto = asm.AssembleFrom(_user);

			Assert.AreEqual(dto.Gender, Gender.Male);
			Assert.AreEqual(dto.GenderCode, "M");

			_user.UserGender = "W";
			dto = asm.AssembleFrom(_user);

			Assert.AreEqual(dto.Gender, Gender.Female);
			Assert.AreEqual(dto.GenderCode, "W");
		}

		[Test]
		[ExpectedException(typeof(OtisException), ExpectedMessage = errSourceCodeGeneration)]
		public void Cant_Get_Assembler_If_Source_Code_Is_Generated()
		{
			Configuration cfg = new Configuration();
			cfg.GenerationOptions.OutputType = OutputType.SourceCode;
			cfg.GenerationOptions.Namespace = "OtisTest";

			cfg.AddType(typeof(AttributedUserDTO));
			IAssembler<AttributedUserDTO, User> asm = cfg.GetAssembler<IAssembler<AttributedUserDTO,User>>();
		}

		[Test]
		[ExpectedException(typeof(OtisException), ExpectedMessage = errInstanceSuppressed)]
		public void Cant_Get_Assembler_If_Instance_Creation_Is_Suppressed()
		{
			Configuration cfg = new Configuration();
			cfg.GenerationOptions.OutputType = OutputType.Assembly;
			cfg.GenerationOptions.SupressInstanceCreation = true;

			cfg.AddType(typeof(AttributedUserDTO));
			IAssembler<AttributedUserDTO, User> asm = cfg.GetAssembler<IAssembler<AttributedUserDTO,User>>();
		}

		private void TestXmlConfiguration(Configuration cfg)
		{
			IAssembler<XmlUserDTO, User> asm = cfg.GetAssembler<IAssembler<XmlUserDTO,User>>();
			XmlUserDTO dto = asm.AssembleFrom(_user);

			Assert.AreEqual(dto.Id, _user.Id);
			Assert.AreEqual(dto.Age, _user.Age);
			Assert.AreEqual(dto.UserName, _user.UserName.ToUpper());
			Assert.AreEqual(dto.ProjectCount, _user.Projects.Count);
			Assert.AreEqual(dto.FullName, _user.FirstName + " " + _user.LastName);
		}
	}
}

