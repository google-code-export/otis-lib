/*
 * Created by: Zdeslav Vojkovic
 * Created: Friday, October 26, 2007
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class CollectionMappingTest
	{
		private IAssembler<AttributedUserDTO, User> m_assembler;
		private List<User> m_source;

		[SetUp]
		public void SetUp()
		{
			Configuration cfg = new Configuration();
			cfg.AddType(typeof(AttributedUserDTO));
			m_assembler = cfg.GetAssembler<AttributedUserDTO, User>();
			m_source = new List<User>(3);
			m_source.Add(new User());
			m_source[0].FirstName = "Zdeslav";
			m_source[0].LastName = "Vojkovic";
			m_source[0].Age = 33;
			m_source[0].Id = 1;
			m_source[0].UserName = "zdeslavv";

			m_source.Add(Helpers.CreateComplexUser());
			m_source.Add(m_source[1].Boss);
		}

		[TearDown]
		public void TearDown() {}

		[Test]
		public void Enumerable_Can_Convert_To_Array()
		{
			AttributedUserDTO[] dtos = m_assembler.ToArray(m_source);
			Assert.AreEqual(m_source[0].FirstName + " " + m_source[0].LastName, dtos[0].FullName);
			Assert.AreEqual(m_source[1].FirstName + " " + m_source[1].LastName, dtos[1].FullName);
			Assert.AreEqual(m_source[2].FirstName + " " + m_source[2].LastName, dtos[2].FullName);
		}

		[Test]
		public void Enumerable_Can_Convert_To_Collection()
		{
			ICollection<AttributedUserDTO> dtos = m_assembler.ToList(m_source);
			int i = 0;
			foreach (AttributedUserDTO dto in dtos)
			{
				Assert.AreEqual(m_source[i].FirstName + " " + m_source[i].LastName, dto.FullName);
				i++;
			}
		}
	}
}

