/*
 * Created by: Zdeslav Vojkovic
 * Created: Saturday, March 01, 2008
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class AggregateFunctionTargetConstraintsTest
	{
		private const string errUnsupportedTarget1 = "Aggregate function 'concat' only supports type 'System.String' as target type";
		private const string errUnsupportedTarget2 = "Value of type 'System.Int32' can't be assigned to member of type 'System.String'";
		private const string errUnsupportedSource = 
			"Type 'Otis.Tests.Entity.NamedEntity' can't be used with aggregate functions 'min' and 'max' because it doesn't implement IComparable or IComparable<T> interfaces";
		private User _user;
		private Configuration _cfg;

		[SetUp]
		public void SetUp()
		{
			_cfg = new Configuration();
			_user = Helpers.CreateComplexUser();
		}

		private void CheckType<T>(string msg)
		{	
			_cfg.AddType(typeof(T));

			try
			{
				IAssembler<T, User> asm = _cfg.GetAssembler<IAssembler<T,User>>();	
			}
			catch(OtisException e)
			{
				if (e.Message.EndsWith(msg))
					return;
				Assert.Fail("Tested method threw an OtisException with incorrect message:"
					+ Environment.NewLine
					+ "Expected to end with: "
					+ msg
					+ Environment.NewLine
					+ "Instead was:          "
					+ e.Message);
			}
			Assert.Fail("Tested method didn't throw an OtisException!");
		}

		[Test]
		public void Concat_Function_Supports_Strings_Only()
		{
			CheckType<ConcatTestDTO>(errUnsupportedTarget1);
		}


		[Test]
		public void MinMax_Function_Supports_Only_Target_Types_Assignable_From_Source()
		{
			CheckType<MinMaxTestDTO>(errUnsupportedTarget2);
		}

		[Test]
		public void MinMax_Function_Supports_Only_Source_Types_Which_Are_Comparable()
		{
			CheckType<InvalidMinMaxTestDTO>(errUnsupportedSource);
		}

	}

	[MapClass(typeof(User))]
	public class ConcatTestDTO
	{
		[Map("concat:$Documents/Name")]
		public int Age;
	}

	[MapClass(typeof(User))]
	public class MinMaxTestDTO
	{
		[Map("max:$Projects/Tasks/Duration")]
		public double MaxTaskDuration;

		[Map("min:$Projects/Tasks/Duration")]
		public string MinTaskDuration;
	}

	[MapClass(typeof(User))]
	public class InvalidMinMaxTestDTO
	{
		[Map("max:$Documents")] // test with non-comparable type
		public NamedEntity LongestDoc;
	}

}

