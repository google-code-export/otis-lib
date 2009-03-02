/*
 * Created by: Zdeslav Vojkovic
 * Created: Sunday, March 02, 2008
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Otis.CodeGen;
using Otis.Functions;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class SimpleFunctionBaseTest
	{
		[Test]
		public void Assignable_Check()
		{
			AssertAssignable<object, string>();
			AssertNotAssignable<string, object>();

			AssertAssignable<NamedEntity, Project>();
			AssertNotAssignable<Project, NamedEntity>();
			AssertAssignable<Entity.EntityBase, Project>();
			AssertNotAssignable<Project, Entity.EntityBase>();

			AssertAssignable<double, float>();
			AssertAssignable<double, long>();
			AssertAssignable<double, short>();
			AssertAssignable<double, sbyte>();
			AssertAssignable<double, char>();

			AssertAssignable<float, long>();
			AssertAssignable<float, short>();
			AssertAssignable<float, sbyte>();
			AssertAssignable<float, char>();

			AssertAssignable<int, short>();
			AssertAssignable<int, sbyte>();
			AssertAssignable<int, char>();

			AssertAssignable<short, sbyte>();
			AssertAssignable<short, char>();

			AssertAssignable<uint, ushort>();
			AssertAssignable<uint, byte>();
			AssertAssignable<uint, char>();

			AssertAssignable<ushort, byte>();
			AssertAssignable<ushort, char>();

			AssertNotAssignable<double, ulong>();
			AssertNotAssignable<double, ushort>();
			AssertNotAssignable<double, byte>();

			AssertNotAssignable<float, ulong>();
			AssertNotAssignable<float, ushort>();
			AssertNotAssignable<float, byte>();

			AssertNotAssignable<int, uint>();
			AssertNotAssignable<int, ushort>();
			AssertNotAssignable<int, byte>();

			AssertNotAssignable<short, ushort>();
			AssertNotAssignable<short, byte>();

			AssertNotAssignable<char, byte>();
			AssertNotAssignable<char, sbyte>();
			AssertNotAssignable<byte, char>();
			AssertNotAssignable<sbyte, char>();
		}

		private void AssertAssignable<TARGET, SOURCE>()
		{
			SimpleFunction fn = new SimpleFunction();
			Assert.IsTrue(fn.IsAssignable(typeof(TARGET), typeof(SOURCE)));
		}

		private void AssertNotAssignable<TARGET, SOURCE>()
		{
			SimpleFunction fn = new SimpleFunction();
			Assert.IsFalse(fn.IsAssignable(typeof(TARGET), typeof(SOURCE)));
		}
	}

	class SimpleFunction  : SimpleFunctionBase
	{
		new public bool IsAssignable(Type target, Type source)	// to expose it as public
		{
			return base.IsAssignable(target, source);
		}

		protected override string GetFormatForExecutedExpression(AggregateFunctionContext context)
		{
			throw new NotImplementedException();
		}
	}
}

