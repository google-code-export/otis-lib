/*
 * Created by: Zdeslav Vojkovic
 * Created: Friday, October 05, 2007
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Otis.Functions;

namespace Otis.Tests
{
	[TestFixture]
	public class AggregateFunctionTests
	{	
		/*
		[Test]
		public void Sum_Function_Decimal()
		{
			SumFunction<decimal> fn = new SumFunction<decimal>();
			decimal[] values = { 1.22m, 2.33m, 3.44m };

			foreach (decimal value in values) fn.ProcessValue(fn.Result + value);

			Assert.AreEqual(1.22m + 2.33m + 3.44m, fn.Result);
		}

		[Test]
		public void Sum_Function_String()
		{
			SumFunction<string> fn = new SumFunction<string>();
			string[] values = { "this", " ", "is", " ", "a", " ", "test" };

			foreach (string value in values) fn.ProcessValue(fn.Result + value);

			Assert.AreEqual("this is a test", fn.Result);
		}

		[Test]
		public void Sum_Function_With_Initial_Value_Test()
		{
			SumFunction<double> fn = new SumFunction<double>();
			fn.Initialize(11.2);
			double[] values = { 1.1, 2.1, 3.1, 4.1, 5.1 };

			foreach (double value in values) fn.ProcessValue(fn.Result + value);

			Assert.AreEqual(11.2 + 1.1 + 2.1 + 3.1 + 4.1 + 5.1, fn.Result);
			Assert.AreEqual(5, fn.ProcessedItemCount);
		}	  

		[Test]
		public void Min_Function_Test()
		{
			MinFunction<int> fn = new MinFunction<int>();
			int[] values = { 1, 2, 3, 4, 5 };

			ProcessValues(fn, values);
			Assert.AreEqual(1, fn.Result);
			Assert.AreEqual(5, fn.ProcessedItemCount);
		}

		[Test]
		public void Min_Function_With_Initial_Value_Test()
		{
			MinFunction<int> fn = new MinFunction<int>();
			fn.Initialize(-2);
			int[] values = { 1, 2, 3, 4, 5 };

			ProcessValues(fn, values);
			Assert.AreEqual(-2, fn.Result);
			Assert.AreEqual(5, fn.ProcessedItemCount);
		}

		[Test]
		public void Max_Function_Test()
		{
			MaxFunction<int> fn = new MaxFunction<int>();
			int[] values = { 1, 2, 3, 4, 5 };

			ProcessValues(fn, values);
			Assert.AreEqual(5, fn.Result);
			Assert.AreEqual(5, fn.ProcessedItemCount);
		}

		[Test]
		public void Max_Function_With_Initial_Value_Test()
		{
			MaxFunction<int> fn = new MaxFunction<int>();
			fn.Initialize(22);
			int[] values = { 1, 2, 3, 4, 5 };

			ProcessValues(fn, values);
			Assert.AreEqual(22, fn.Result);
			Assert.AreEqual(5, fn.ProcessedItemCount);
		}

		[Test]
		public void Avg_Function_Test()
		{
			AvgFunction fn = new AvgFunction();
			double[] values = { 1, 2, 3, 4, 5 };

			ProcessValues(fn, values);
			Assert.AreEqual(3, fn.Result);
			Assert.AreEqual(5, fn.ProcessedItemCount);
		}

		[Test]
		public void Avg_Function_Ignores_Initial_Value()
		{
			AvgFunction fn = new AvgFunction();
			fn.Initialize(22);
			double[] values = { 1, 2, 3, 4, 5 };

			ProcessValues(fn, values);
			Assert.AreEqual(3, fn.Result);
			Assert.AreEqual(5, fn.ProcessedItemCount);
		}

		[Test]
		public void Count_Function_Test()
		{
			CountFunction<int> fn = new CountFunction<int>();
			int[] values = { 10, 20, 30, 40, 50 };

			ProcessValues(fn, values);
			Assert.AreEqual( 5, fn.Result);
			Assert.AreEqual(5, fn.ProcessedItemCount);
		}

		[Test]
		public void Count_Function_Ignores_Initial_Value()
		{
			CountFunction<int> fn = new CountFunction<int>();
			fn.Initialize(222);
			int[] values = { 10, 20, 30, 40, 50 };

			ProcessValues(fn, values);
			Assert.AreEqual(5, fn.Result);
			Assert.AreEqual(5, fn.ProcessedItemCount);
		}   */

		private void ProcessValues<T>(IAggregateFunction<T> fn, ICollection<T> values)
		{
			foreach (T value in values)
			{
				fn.ProcessValue(value);
			}
		}
	}


}

