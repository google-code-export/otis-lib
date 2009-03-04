/*
 * Created by: 
 * Created: Saturday, March 01, 2008
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Otis.Attributes;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class ExpressionFormattingTests
	{
		[Test]
		public void Formatting_Works_For_Aggregate_Expressions()
		{
			Configuration cfg = new Configuration();
			cfg.AddType(typeof(Dummy));
			IAssembler<Dummy, User> asm = cfg.GetAssembler<IAssembler<Dummy, User>>();
			Dummy dummy = asm.AssembleFrom(Helpers.CreateComplexUser());

			Assert.AreEqual("2 documents", dummy.DocumentInfo);
			Assert.AreEqual("Avg document length: 4.00 characters", dummy.AvgDocumentNameLengthInfo);
		}

		[Test]
		public void Formatting_Works_For_User_Aggregates_Not_Derived_From_SimpleFunctionBase()
		{
			Configuration cfg = new Configuration();
			cfg.RegisterFunction<MedianFn>("median");
			cfg.AddType(typeof(Dummy2));
			IAssembler<Dummy2, User> asm = cfg.GetAssembler<IAssembler<Dummy2, User>>();
			Dummy2 dummy = asm.AssembleFrom(Helpers.CreateComplexUser());

			Assert.AreEqual("Median document length: 4.00 characters", dummy.MedianDocumentNameLengthInfo);
		}
	}

	[MapClass(typeof(User))]
	public class Dummy
	{
		[Map("count:$Documents", Format = "{0} documents")]
		public string DocumentInfo;

		[Map("avg:$Documents/Name.Length", Format = "Avg document length: {0:N2} characters")]
		public string AvgDocumentNameLengthInfo;
	}

	[MapClass(typeof(User))]
	public class Dummy2
	{
		[Map("median:$Documents/Name.Length", Format = "Median document length: {0:N2} characters")]
		public string MedianDocumentNameLengthInfo;
	}

}

