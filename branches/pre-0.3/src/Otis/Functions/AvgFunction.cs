using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using Otis.Generation;

namespace Otis.Functions
{
	public class AvgFunction : SimpleFunctionBase
	{
		public override IEnumerable<CodeStatement> GetInitializationStatements(AggregateFunctionContext context)
		{
			List<CodeStatement> statements = new List<CodeStatement>(base.GetInitializationStatements(context));

			CodeStatement st = new CodeVariableDeclarationStatement(typeof(int),
				GetCounterObjectName(context),
				new CodeSnippetExpression("0"));

			statements.Add(st);

			return statements;
		}

		protected override string GetResultExpression()
		{
			return string.Format("{0}/{1}", Context.FunctionObjectName, GetCounterObjectName(Context));
		}

		private static string GetCounterObjectName(AggregateFunctionContext context)
		{
			return context.FunctionObjectName + "_Cnt";
		}

		protected override string GetFormatForExecutedExpression(AggregateFunctionContext context)
		{
			// todo: test
			return string.Format("FN_OBJ = FN_OBJ + CURR_ITEM; {0}++;", GetCounterObjectName(context));
		}

		protected override Type GetDefaultTargetType()
		{
			return typeof(double);
		}
	}
}
