/*
 * Created by: Zdeslav Vojkovic
 * Created: Wednesday, October 24, 2007
 */

using System;
using System.CodeDom;
using System.Collections.Generic;
using NUnit.Framework;
using Otis.CodeGen;

namespace Otis.Tests
{
	[TestFixture]
	public class FunctionMapTest
	{
		[SetUp]
		public void SetUp() {}

		[TearDown]
		public void TearDown() {}

		[Test]
		[ExpectedException(typeof(OtisException))]
		public void Registration_Fails_If_Type_Doesnt_Implement_IAggregateFunction()
		{
			FunctionMap map = new FunctionMap();
			map.Register("test", typeof(string));
		}

		[Test]
		public void Registration_Suceeds_For_Well_Implemented_Aggregate_Function()
		{
			FunctionMap map = new FunctionMap();
			map.Register("test", typeof(TestFn<int>));
			Type t1 = typeof(TestFn<int>);
			Type t2 = map.GetTypeForFunction("test");
			Assert.AreEqual(typeof(TestFn<int>), map.GetTypeForFunction("test"));
		}

		[Test]
		[ExpectedException(typeof(OtisException))]
		public void Retrieving_Unregistered_Function_Fails()
		{
			FunctionMap map = new FunctionMap();
			map.GetTypeForFunction("test");
		}
	}
	/*
	class TestFn<T> : IAggregateFunction<T> 
	{
		public void Initialize(T initialValue){}
		public void ProcessValue(T value){}
		public int ProcessedItemCount { get { return 0; } }
		public T Result { get { return default(T); } }
		public string ExpressionFormat { get { return null; }
		}
	}*/

	class TestFn<T> : IAggregateFunctionCodeGenerator 
	{
		public IEnumerable<CodeStatement> GetInitializationStatements(AggregateFunctionContext context) { return null; }
		public IEnumerable<string> GetIterationStatements(AggregateFunctionContext context, IList<AggregateExpressionPathItem> pathItems) { return null; }
		public CodeStatement GetAssignmentStatement(AggregateFunctionContext context) { return null; }
	}
}

