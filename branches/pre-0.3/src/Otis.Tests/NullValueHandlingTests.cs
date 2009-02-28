/*
 * Created by: Zdeslav Vojkovic
 * Created: Monday, May 26, 2008
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class NullValueHandlingTests
	{
		private IAssembler<AttributedUserDTO, User> m_asm;
		private User m_user;

		public void SetUpDefaults()
		{
			Configuration cfg = new Configuration();
			cfg.AddType<AttributedUserDTO>();
			m_asm = cfg.GetAssembler<AttributedUserDTO, User>();
			m_user = Helpers.CreateComplexUser();
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
			m_user.UserName = null;
			AttributedUserDTO dto = m_asm.AssembleFrom(m_user);
			Assert.AreEqual("[unknown]", dto.UserName);
		}

		[Test]
		public void Null_value_replacement_for_compound_expressions()
		{
			SetUpDefaults();
			m_user.FirstName = "AAA";
			m_user.LastName = null;
			AttributedUserDTO dto = m_asm.AssembleFrom(m_user);
			Assert.AreEqual("MISSING_NAME_PART", dto.FullName);

			m_user.FirstName = null;
			m_user.LastName = "BBB";
			dto = m_asm.AssembleFrom(m_user);
			Assert.AreEqual("MISSING_NAME_PART", dto.FullName);

		}

	}
}

