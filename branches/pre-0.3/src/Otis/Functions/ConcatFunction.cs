using System;
using System.Collections.Generic;
using System.Text;
using Otis.CodeGen;

namespace Otis.Functions
{
	public class ConcatFunction : SimpleFunctionBase
	{
		private string _separator = ", ";

		override protected Type GetFunctionObjectType()
		{
			return typeof (StringBuilder);
		}

		protected override string GetFunctionObjectInitialValue()
		{
			return "new System.Text.StringBuilder(40)";
		}

		protected override string GetResultExpression()
		{
			return string.Format("({0}.Length > {1} ? {0}.ToString(0, {0}.Length - {1}) : \"\")", 
				Context.FunctionObjectName, Separator.Length);
		}

		protected override string GetFormatForExecutedExpression(AggregateFunctionContext context)
		{
			return string.Format("FN_OBJ.Append(CURR_ITEM); FN_OBJ.Append(\"{0}\");", Separator);
			// -> "{{ FN_OBJ.Append(CURR_ITEM); FN_OBJ.Append(\"separator\");  }}"
			// -> "{ FN_OBJ.Append(CURR_ITEM); FN_OBJ.Append(\"separator\");  }"
		}

		protected override bool IsTypeSupportedAsTarget(Type type)
		{
			return type == typeof (string);
		}

		protected override string UnsupportedTargetTypeErrorMessage
		{
			get { return "Aggregate function 'concat' only supports type 'System.String' as target type"; }
		}

		public string Separator
		{
			get { return _separator; }
		}
	}
}
