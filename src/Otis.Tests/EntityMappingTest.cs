using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class EntityMappingTest
	{
		private IAssembler<UserDTO, User> m_assembler;
		private User m_user;

		[SetUp]
		public void Setup()
		{
			Configuration cfg;
			cfg = new Configuration();
			cfg.AddType<UserDTO>();
			cfg.AddType<ProjectDTO>(); // todo: should this be automatically detected
			cfg.AddType<DocumentDTO>(); // todo: should this be automatically detected
			cfg.AddType<TaskDTO>(); // todo: should this be automatically detected

			cfg.BuildAssemblers();

			m_assembler = cfg.GetAssembler<UserDTO, User>();

			m_user = Helpers.CreateComplexUser();  
		}

		[Test]
		public void Formatting_Is_Applied_To_String_Members()
		{
			m_user.BirthDay = new DateTime(1973, 10, 22);

			CultureInfo culture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

			UserDTO dto = m_assembler.AssembleFrom(m_user);

			Assert.AreEqual("Monday, October 22, 1973", dto.BirthDay);

			Thread.CurrentThread.CurrentCulture = culture;
		}

		[Test]
		public void Complex_Members_Are_Transformed()
		{
			UserDTO dto = m_assembler.AssembleFrom(m_user);

			Assert.IsNotNull(dto.Boss);
			Assert.AreEqual("X Y", dto.Boss.FullName);
			Assert.AreEqual(101, dto.Boss.Id);
			Assert.AreEqual(40, dto.Boss.Age);

			Assert.IsNotNull(dto.Boss.Boss);
			UserDTO ceo = dto.Boss.Boss;
			Assert.AreEqual("Mega Boss", ceo.FullName);
			Assert.AreEqual(102, ceo.Id);
			Assert.AreEqual(50, ceo.Age);

			Assert.IsNull(ceo.Boss);
		}

		[Test]
		public void Conversion_To_Array()
		{
			UserDTO dto = m_assembler.AssembleFrom(m_user);

			Assert.IsNotNull(dto.Projects);
			Assert.AreEqual(3, dto.Projects.Length);
			Assert.AreEqual(2, dto.Projects[0].TaskCount);
			Assert.AreEqual(3, dto.Projects[1].TaskCount);
			Assert.AreEqual(0, dto.Projects[2].TaskCount);
		   
			Assert.IsNotNull(dto.DocumentsCopy);
			Assert.AreEqual(2, dto.DocumentsCopy.Length);
			Assert.AreEqual("1000 - doc1", dto.DocumentsCopy[0].Description);
			Assert.AreEqual("1001 - doc2", dto.DocumentsCopy[1].Description);
			Assert.AreEqual("doc1, doc2", dto.DocumentInfo);  
		}

		[Test]
		public void Conversion_To_List()
		{
			UserDTO dto = m_assembler.AssembleFrom(m_user);

			Assert.IsNotNull(dto.Documents);
			Assert.AreEqual(2, dto.Documents.Count);
			Assert.AreEqual("1000 - doc1", dto.Documents[0].Description);
			Assert.AreEqual("1001 - doc2", dto.Documents[1].Description);

			Assert.IsNotNull(dto.ProjectsCopy);
			Assert.AreEqual(3, dto.ProjectsCopy.Count);
			Assert.AreEqual(2, dto.ProjectsCopy[0].TaskCount);
			Assert.AreEqual(3, dto.ProjectsCopy[1].TaskCount);
			Assert.AreEqual(0, dto.ProjectsCopy[2].TaskCount);
		}
	}


}
