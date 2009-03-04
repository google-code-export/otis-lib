using System;
using Otis.Generation;

namespace Otis.Functions
{
	public class CountFunction : SimpleFunctionBase
	{
		protected override string GetFormatForExecutedExpression(AggregateFunctionContext context)
		{
			return "FN_OBJ = FN_OBJ + 1;";
		}

		protected override Type GetDefaultTargetType()
		{
			return typeof(int);
		}
	}
}
