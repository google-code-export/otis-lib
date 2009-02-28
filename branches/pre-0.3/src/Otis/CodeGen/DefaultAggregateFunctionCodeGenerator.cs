using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using Otis.Parsing;

namespace Otis.CodeGen
{
	public class DefaultAggregateFunctionCodeGenerator : IAggregateFunctionCodeGenerator 
	{
		virtual public IEnumerable<CodeStatement> GetInitializationStatements(AggregateFunctionContext context)
		{
			CodeExpression[] parameters = new CodeExpression[0];
			CodeStatement st = new CodeVariableDeclarationStatement(context.ImplementationType, context.FunctionObjectName,
							new CodeObjectCreateExpression(context.ImplementationType, parameters));

			return new CodeStatement[] { st };
		}

		virtual public IEnumerable<string> GetIterationStatements(AggregateFunctionContext context, IList<AggregateExpressionPathItem> pathItems)
		{
			AggregateExpressionPathItem lastPathItem = pathItems[pathItems.Count - 1];
			string finalTarget = "";
			foreach (AggregateExpressionPathItem pathItem in pathItems)
				if (pathItem.IsCollection)
					finalTarget = pathItem.Object;

			string finalExpression = context.Member.AggregateMappingDescription.FinalExpression;

			string processedExpression = "";
			if ((string.IsNullOrEmpty(finalExpression) || lastPathItem.IsCollection))
				processedExpression = finalTarget;
			else
				processedExpression = string.Format("{0}.{1}", finalTarget, finalExpression);

			string fullFormat = GetFormatForExecutedExpression(context);
			fullFormat = fullFormat.Replace("FN_OBJ", "{0}");
			fullFormat = fullFormat.Replace("CURR_ITEM", "{1}");

			return new string[] { string.Format(fullFormat, context.FunctionObjectName, processedExpression) };
		}

		protected virtual string GetFormatForExecutedExpression(AggregateFunctionContext context) 
		{
			string processedExpressionFormat = GetProcessedExpressionFormat(context);
			return "{0}.ProcessValue(" + processedExpressionFormat + ")";
		}

		virtual public CodeStatement GetAssignmentStatement(AggregateFunctionContext context)
		{
			string resultExpression = context.FunctionObjectName + ".Result";
			if (context.Member.HasFormatting)
				resultExpression = string.Format("string.Format(\"{0}\", {1}.Result)", context.Member.Format, context.FunctionObjectName);

			return new CodeAssignStatement(
					new CodeVariableReferenceExpression("target." + context.Member.Member),
					new CodeArgumentReferenceExpression(resultExpression));	
		}


		private string GetProcessedExpressionFormat(AggregateFunctionContext context)
		{
			IExpressionFormatProvider fmtProvider = context.Generator as IExpressionFormatProvider;

			if (fmtProvider == null)
			{
				Type realImplementationType = context.ImplementationType.IsGenericTypeDefinition ?
				                              context.ImplementationType.MakeGenericType(typeof (int)) :
				                              context.ImplementationType;

				fmtProvider = (IExpressionFormatProvider) Activator.CreateInstance(realImplementationType, true);
			}

			if (string.IsNullOrEmpty(fmtProvider.ExpressionFormat))
				return "{1}";
			else
				return fmtProvider.ExpressionFormat;
		}

	}
}
