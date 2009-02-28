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
		private IAssembler<AttributedUserDTO, User> _assembler;
		private List<User> _source;

		[SetUp]
		public void SetUp()
		{
			Configuration cfg = new Configuration();
			cfg.AddType(typeof(AttributedUserDTO));
			_assembler = cfg.GetAssembler<IAssembler<AttributedUserDTO,User>>();
			_source = new List<User>(3);
			_source.Add(new User());
			_source[0].FirstName = "Zdeslav";
			_source[0].LastName = "Vojkovic";
			_source[0].Age = 33;
			_source[0].Id = 1;
			_source[0].UserName = "zdeslavv";

			_source.Add(Helpers.CreateComplexUser());
			_source.Add(_source[1].Boss);
		}

		[TearDown]
		public void TearDown() {}

		[Test]
		public void Enumerable_Can_Convert_To_Array()
		{
			AttributedUserDTO[] dtos = _assembler.ToArray(_source);
			Assert.AreEqual(_source[0].FirstName + " " + _source[0].LastName, dtos[0].FullName);
			Assert.AreEqual(_source[1].FirstName + " " + _source[1].LastName, dtos[1].FullName);
			Assert.AreEqual(_source[2].FirstName + " " + _source[2].LastName, dtos[2].FullName);
		}

		[Test]
		public void Enumerable_Can_Convert_To_Collection()
		{
			ICollection<AttributedUserDTO> dtos = _assembler.ToList(_source);
			int i = 0;
			foreach (AttributedUserDTO dto in dtos)
			{
				Assert.AreEqual(_source[i].FirstName + " " + _source[i].LastName, dto.FullName);
				i++;
			}
		}
	}
}

