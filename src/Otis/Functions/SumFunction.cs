using System;
using Otis.Generation;

namespace Otis.Functions
{
	public class SumFunction : SimpleFunctionBase
	{
		protected override string GetFormatForExecutedExpression(AggregateFunctionContext context)
		{
			return "FN_OBJ = FN_OBJ + CURR_ITEM;";
		}
	}
}
