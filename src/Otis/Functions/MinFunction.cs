using System;
using System.Collections.Generic;
using System.Text;
using Otis.CodeGen;

namespace Otis.Functions
{
	public class MinFunction : MinMaxFunctionBase
	{
		protected override string GetFormatForExecutedExpression(AggregateFunctionContext context)
		{
			// todo: test
			return "if(CURR_ITEM < FN_OBJ) FN_OBJ = CURR_ITEM;";
		}

		protected override string GetFieldNameForInitialValue()
		{
			return "MaxValue";
		}
	}
}
