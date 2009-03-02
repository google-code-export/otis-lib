/*
 * Created by: Zdeslav Vojkovic
 * Created: Monday, May 26, 2008
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Otis.Tests.Dto;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class NullValueHandlingTests
	{
		private IAssembler<AttributedUserDTO, User> _asm;
		private User _user;

		public void SetUpDefaults()
		{
			Configuration cfg = new Configuration();
			cfg.AddType<AttributedUserDTO>();
			_asm = cfg.GetAssembler<IAssembler<AttributedUserDTO,User>>();
			_user = Helpers.CreateComplexUser();
		}

		[TearDown]
		public void TearDown() {}

		[Test]
		public void Nullable_parts_extraction()
		{
			MemberMappingDescriptor member = new MemberMappingDescriptor();

			member.Expression = "$FirstName";
			Assert.IsTrue(member.HasNullableParts);
			Assert.AreEqual(1, member.NullableParts.Count);
			Assert.AreEqual("$FirstName", member.NullableParts[0]);

			member.Expression = "$FirstName.ToUpper()";
			Assert.IsTrue(member.HasNullableParts);
			Assert.AreEqual(1, member.NullableParts.Count);
			Assert.AreEqual("$FirstName", member.NullableParts[0]);

			member.Expression = "$FirstName.ToUpper(true)";
			Assert.IsTrue(member.HasNullableParts);
			Assert.AreEqual(1, member.NullableParts.Count);
			Assert.AreEqual("$FirstName", member.NullableParts[0]);

			member.Expression = "$ToUpper()";
			Assert.IsTrue(member.HasNullableParts);
			Assert.AreEqual(1, member.NullableParts.Count);
			Assert.AreEqual("$ToUpper()", member.NullableParts[0]);

			member.Expression = "$ToUpper().ToLower()";
			Assert.IsTrue(member.HasNullableParts);
			Assert.AreEqual(1, member.NullableParts.Count);
			Assert.AreEqual("$ToUpper()", member.NullableParts[0]);

			member.Expression = "$ToUpper(true)";
			Assert.IsTrue(member.HasNullableParts);
			Assert.AreEqual(1, member.NullableParts.Count);
			Assert.AreEqual("$ToUpper(true)", member.NullableParts[0]);

			member.Expression = "$FirstName + $LastName";
			Assert.IsTrue(member.HasNullableParts);
			Assert.AreEqual(2, member.NullableParts.Count);
			Assert.AreEqual("$FirstName", member.NullableParts[0]);
			Assert.AreEqual("$LastName", member.NullableParts[1]);

			member.Expression = "tmp.GetSomeValue";
			Assert.IsFalse(member.HasNullableParts);
			Assert.AreEqual(0, member.NullableParts.Count);
		}

		[Test]
		public void Null_value_replacement()
		{
			SetUpDefaults();
			_user.UserName = null;
			AttributedUserDTO dto = _asm.AssembleFrom(_user);
			Assert.AreEqual("[unknown]", dto.UserName);
		}

		[Test]
		public void Null_value_replacement_for_compound_expressions()
		{
			SetUpDefaults();
			_user.FirstName = "AAA";
			_user.LastName = null;
			AttributedUserDTO dto = _asm.AssembleFrom(_user);
			Assert.AreEqual("MISSING_NAME_PART", dto.FullName);

			_user.FirstName = null;
			_user.LastName = "BBB";
			dto = _asm.AssembleFrom(_user);
			Assert.AreEqual("MISSING_NAME_PART", dto.FullName);

		}

	}
}

