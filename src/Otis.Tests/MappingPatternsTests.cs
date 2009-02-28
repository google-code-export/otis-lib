/*
 * Created by: Zdeslav Vojkovic
 * Created: Thursday, January 24, 2008
 */

using Otis.Parsing;
using NUnit.Framework;

namespace Otis.Tests
{
	[TestFixture]
	public class MappingPatternsTests
	{

		[Test]
		public void Valid_Aggregate_Expressions()
		{
			Assert.IsTrue(ExpressionParser.IsAggregateExpression("sum:$Project/Tasks"));
			Assert.IsTrue(ExpressionParser.IsAggregateExpression("sum:$Project/Tasks1"));
			Assert.IsTrue(ExpressionParser.IsAggregateExpression("sum:Project"));
			Assert.IsTrue(ExpressionParser.IsAggregateExpression("collect:Project/Tasks/Id"));
			Assert.IsTrue(ExpressionParser.IsAggregateExpression(" sum:$Project/Tasks "));
			Assert.IsTrue(ExpressionParser.IsAggregateExpression(" sum:$User.Project/Tasks1 "));
			Assert.IsTrue(ExpressionParser.IsAggregateExpression("_sum:$Project/Tasks"));
		}

		[Test]
		public void Invalid_Aggregate_Expressions()
		{
			Assert.IsFalse(ExpressionParser.IsAggregateExpression("sum:$Project/Tasks/"));
			Assert.IsFalse(ExpressionParser.IsAggregateExpression("sum Project"));
			Assert.IsFalse(ExpressionParser.IsAggregateExpression(".sum:Project"));
		}

		[Test]
		public void Valid_Literal_Expressions()
		{
			Assert.IsTrue(ExpressionParser.IsLiteralExpression("[whathever]"));
			Assert.IsTrue(ExpressionParser.IsLiteralExpression(" [whathever] "));
			Assert.IsTrue(ExpressionParser.IsLiteralExpression(" [whathever] "));
			Assert.IsTrue(ExpressionParser.IsLiteralExpression("['a' + 'b']"));
			Assert.IsTrue(ExpressionParser.IsLiteralExpression("['a' + \"b\"]"));
		}

		[Test]
		public void Invalid_Literal_Expressions()
		{
			Assert.IsFalse(ExpressionParser.IsLiteralExpression("[whathever"));
			Assert.IsFalse(ExpressionParser.IsLiteralExpression(" whathever] "));
			Assert.IsFalse(ExpressionParser.IsLiteralExpression(""));
		}

		[Test]
		public void Valid_Projection_Expressions()
		{
			Assert.IsTrue(ExpressionParser.IsProjectionExpression("a=>b"));
			Assert.IsTrue(ExpressionParser.IsProjectionExpression(" a => b "));
			Assert.IsTrue(ExpressionParser.IsProjectionExpression("a=>b;c=>d"));
			Assert.IsTrue(ExpressionParser.IsProjectionExpression("ax=>bx;cx => dx"));
			Assert.IsTrue(ExpressionParser.IsProjectionExpression("a=>b; c=>d;"));
			Assert.IsTrue(ExpressionParser.IsProjectionExpression("a=>b; \"c\"=>d;"));
			Assert.IsTrue(ExpressionParser.IsProjectionExpression("a=>b; \"c\"=>d; "));
		}

		[Test]
		public void Invalid_Projection_Expressions()
		{
			Assert.IsFalse(ExpressionParser.IsProjectionExpression("aa=>>ab:ac=>ad"));
			Assert.IsFalse(ExpressionParser.IsProjectionExpression("aa=>ab:ac=>ad"));
			Assert.IsFalse(ExpressionParser.IsProjectionExpression(".aa=>ab:ac=>ad"));
			Assert.IsFalse(ExpressionParser.IsProjectionExpression("a=a=>ab;ac=>ad"));
			Assert.IsFalse(ExpressionParser.IsProjectionExpression("a=a=>ab>ac=>ad"));
			Assert.IsFalse(ExpressionParser.IsProjectionExpression("aa=>ab=>ad"));
			Assert.IsFalse(ExpressionParser.IsProjectionExpression(""));
			Assert.IsFalse(ExpressionParser.IsProjectionExpression(" a = b "));
			Assert.IsFalse(ExpressionParser.IsProjectionExpression(" a = > b "));
		}

		[Test]
		public void Literal_Expression_Normalization()
		{
			Assert.AreEqual("\"a\" + \"b\"", ExpressionParser.NormalizeExpression("['a' + 'b']"));
			Assert.AreEqual("'a' + 'b'", ExpressionParser.NormalizeExpression("'a' + 'b'"));
			Assert.AreEqual("\"a\" + \"b\" + \"c\"", ExpressionParser.NormalizeExpression("['a' + 'b' + \"c\"]"));
		}
	}
}

