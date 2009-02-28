using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class MappingPreparerTests
	{
		const string errDuplicatePreparer = "Method 'Preparer' on type 'Otis.Tests.MappingPreparerDTO_Duplicate' is marked with [MappingPreparer], but mapping preparer is already set to 'Otis.Tests.Util.Convert'";
		const string errPreparerIsPrivate = "Non public method 'Preparer' on type 'Otis.Tests.MappingPreparerDTO_NonPublic'is marked with [MappingPreparer]. Only public methods can be used as preparers";
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
		[ExpectedException(typeof(OtisException), ExpectedMessage = errDuplicatePreparer)]
		public void Configuration_Throws_With_Duplicate_Preparers()
		{
			ConfigureType<MappingPreparerDTO_Duplicate>();
		}

		[Test]
		[ExpectedException(typeof(OtisException), ExpectedMessage = errPreparerIsPrivate)]
		public void Configuration_Throws_With_NonPublic_Preparers()
		{
			ConfigureType<MappingPreparerDTO_NonPublic>();
		}

		[Test]
		public void Non_Static_Preparer()
		{
			IAssembler<MappingPreparerDTO_InstancePreparer, User> asm = ConfigureType<MappingPreparerDTO_InstancePreparer>();
			MappingPreparerDTO_InstancePreparer dto = asm.AssembleFrom(_user);
			Assert.AreEqual(77, dto.Id);
			Assert.AreEqual("custom_mapping_InstancePreparer", dto.FullName);
		}

		[Test]
		public void Preparer_Is_Called_Before_Transformation()
		{
			IAssembler<MappingPreparerDTO_CheckOrder, User> asm = ConfigureType<MappingPreparerDTO_CheckOrder>();
			MappingPreparerDTO_CheckOrder dto = asm.AssembleFrom(_user);
			Assert.AreEqual(77, dto.Id);  // check it is not -1
		}

		[Test]
		public void Static_Preparer()
		{
			IAssembler<MappingPreparerDTO_StaticPreparer, User> asm = ConfigureType<MappingPreparerDTO_StaticPreparer>();
			MappingPreparerDTO_StaticPreparer dto = asm.AssembleFrom(_user);
			Assert.AreEqual(77, dto.Id);
			Assert.AreEqual("custom_mapping_StaticPreparer", dto.FullName);
		}

		IAssembler<T, User> ConfigureType<T>()
		{
			Configuration cfg = new Configuration();
			cfg.AddType(typeof(T));
			return cfg.GetAssembler<IAssembler<T,User>>();
		}
	}
}
