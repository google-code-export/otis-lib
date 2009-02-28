using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class MappingHelperTests
	{
		const string errDuplicateHelper = "Method 'Helper' on type 'Otis.Tests.MappingHelperDTO_Duplicate' is marked with [MappingHelper], but mapping helper is already set to 'Otis.Tests.Util.Convert'";
		const string errHelperIsPrivate = "Non public method 'Helper' on type 'Otis.Tests.MappingHelperDTO_NonPublic'is marked with [MappingHelper]. Only public methods can be used as helpers";
		private User _user;

		[SetUp]
		public void SetUp()
		{
			_user = new User();
			_user.Id = 77;
			_user.FirstName = "aaa";
			_user.LastName = "bbb";
		}

		[Test]
		[ExpectedException(typeof(OtisException), ExpectedMessage = errDuplicateHelper)]
		public void Configuration_Throws_With_Duplicate_Helpers()
		{
			ConfigureType<MappingHelperDTO_Duplicate>();
		}

		[Test]
		[ExpectedException(typeof(OtisException), ExpectedMessage = errHelperIsPrivate)]
		public void Configuration_Throws_With_NonPublic_Helpers()
		{
			ConfigureType<MappingHelperDTO_NonPublic>();
		}

		[Test]
		public void Non_Static_Helper()
		{
			IAssembler<MappingHelperDTO_InstanceHelper, User> asm = ConfigureType<MappingHelperDTO_InstanceHelper>();
			MappingHelperDTO_InstanceHelper dto = asm.AssembleFrom(_user);
			Assert.AreEqual(77, dto.Id);
			Assert.AreEqual("custom_mapping_InstanceHelper", dto.FullName);
		}

		[Test]
		public void Static_Helper()
		{
			IAssembler<MappingHelperDTO_StaticHelper, User> asm = ConfigureType<MappingHelperDTO_StaticHelper>();
			MappingHelperDTO_StaticHelper dto = asm.AssembleFrom(_user);
			Assert.AreEqual(77, dto.Id);
			Assert.AreEqual("custom_mapping_StaticHelper", dto.FullName);
		}

		[Test]
		public void Helper_Is_Called_After_Transformation()
		{
			IAssembler<MappingHelperDTO_CheckOrder, User> asm = ConfigureType<MappingHelperDTO_CheckOrder>();
			MappingHelperDTO_CheckOrder dto = asm.AssembleFrom(_user);
			Assert.AreEqual(-100, dto.Id);  // check it is not -1
		}


		IAssembler<T, User> ConfigureType<T>()
		{
			Configuration cfg = new Configuration();
			cfg.AddType(typeof(T));
			return cfg.GetAssembler<IAssembler<T,User>>();
		}
	}
}
