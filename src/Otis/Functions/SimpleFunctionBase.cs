using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using Otis.Generation;
using Otis.Parsing;

namespace Otis.Functions
{
	// todo: clean up, move stuff from Default, remove context from GetFormatForExecutedExpression param list, ... 
	abstract public class SimpleFunctionBase : IAggregateFunctionCodeGenerator 
	{
		private AggregateFunctionContext _context;

		virtual public IEnumerable<CodeStatement> GetInitializationStatements(AggregateFunctionContext context)
		{
			_context = context;

			if(!IsTypeSupportedAsTarget(context.Member.Type))
				throw new OtisException(GetUnsupportedTargetTypeErrorMessage());

			if (!IsTypeSupportedAsSource(context.SourceItemType))
				throw new OtisException(GetUnsupportedSourceTypeErrorMessage());

			CodeStatement st = new CodeVariableDeclarationStatement(GetFunctionObjectType(), 
				context.FunctionObjectName,
				new CodeSnippetExpression(GetFunctionObjectInitialValue()));

			return new CodeStatement[] { st };
		}

		virtual public IEnumerable<string> GetIterationStatements(AggregateFunctionContext context, IList<AggregateExpressionPathItem> pathItems)
		{
			_context = context;
			AggregateExpressionPathItem lastPathItem = pathItems[pathItems.Count - 1];

			string finalTarget = "";
			foreach (AggregateExpressionPathItem pathItem in pathItems)
				if (pathItem.IsCollection)
					finalTarget = pathItem.Object;

			string finalExpression = context.Member.AggregateMappingDescriptor.FinalExpression;

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

		virtual public CodeStatement GetAssignmentStatement(AggregateFunctionContext context)
		{
			_context = context;

			string expression = GetResultExpression();
			if (context.Member.HasFormatting)
				expression = string.Format("string.Format(\"{0}\", {1})", context.Member.Format, expression);

			return new CodeAssignStatement(
					new CodeVariableReferenceExpression("target." + context.Member.Member),
					new CodeArgumentReferenceExpression(expression));
		}

		virtual protected Type GetFunctionObjectType()
		{
			if(!Context.Member.HasFormatting)
				return Context.Member.Type;	

			// if there is formatting, target type is string, but source type
			// may be anything, so we choose source item type as target type
			//return Context.SourceItemType;
			Type type = GetDefaultTargetType(); // give chance to derived class to say
			if (type != null)
				return type;
			else
				return Context.SourceItemType;
		}

		/// <summary>
		/// returns the function object type which will be used if target type can't be deduced.
		/// this usually happens when mapping expression has formatting, so declared target type
		/// is string, but expression is a 'count' function which needs numeric type to be incremented.
		/// however, library has no way to find out this type, because it source type might be anything
		/// (e.g. some entity type), so derived class can override this function to provide suitable
		/// target type.
		/// </summary>
		/// <returns>Target type to be used. If null is returned, mapper will use the type of source item
		/// as a target type</returns>
		virtual protected Type GetDefaultTargetType()
		{
			return null;
		}

		virtual protected string GetFunctionObjectInitialValue()
		{
			return "0";
		}

		protected abstract string GetFormatForExecutedExpression(AggregateFunctionContext context);
		protected virtual string GetResultExpression()
		{
			return Context.FunctionObjectName;
		}

		/// <summary>
		/// Returns whether the 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		protected virtual bool IsTypeSupportedAsTarget(Type type)
		{
			return 
				IsAssignable(type, GetFunctionObjectType())
				|| (type == typeof(string) && Context.Member.HasFormatting);
		}

		/// <summary>
		/// Returns whether the 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		protected virtual bool IsTypeSupportedAsSource(Type type)
		{
			return true;
		}

		/// <summary>
		/// Returns the message which will be displayed when mapping is done to unsupported target type
		/// </summary>
		protected virtual string UnsupportedTargetTypeErrorMessage
		{
			get
			{
				string msg = ErrorBuilder.UnsupportedTargetTypeForAggregateFunction(Context);
				return msg;
			}
		}

		/// <summary>
		/// Returns the message which will be displayed when mapping is done from unsupported source type
		/// </summary>
		protected virtual string UnsupportedSourceTypeErrorMessage
		{
			get
			{
				string msg = ErrorBuilder.UnsupportedSourceTypeForAggregateFunction(Context);
				return msg;
			}
		}

		protected AggregateFunctionContext Context
		{
			get { return _context; }
		}

		private string GetUnsupportedTargetTypeErrorMessage()
		{
			string msg = UnsupportedTargetTypeErrorMessage;
			return ErrorBuilder.CreateStandardErrorMessage(Context.Member, msg);
		}

		private string GetUnsupportedSourceTypeErrorMessage()
		{
			string msg = UnsupportedSourceTypeErrorMessage;
			return ErrorBuilder.CreateStandardErrorMessage(Context.Member, msg);
		}

		protected bool IsAssignable(Type targetType, Type sourceType)
		{
			if (targetType.IsAssignableFrom(sourceType))
				return true;

			if (!targetType.IsPrimitive)
				return false;

			if(targetType == typeof(double))
				return sourceType == typeof(float) || sourceType == typeof(long) || sourceType == typeof(int) || sourceType == typeof(short) || sourceType == typeof(sbyte) || sourceType == typeof(char);

			if (targetType == typeof(float))
				return sourceType == typeof(long) || sourceType == typeof(int) || sourceType == typeof(short) || sourceType == typeof(sbyte) || sourceType == typeof(char);

			if (targetType == typeof(long))
				return sourceType == typeof(int) || sourceType == typeof(short) || sourceType == typeof(sbyte) || sourceType == typeof(char);

			if (targetType == typeof(int))
				return sourceType == typeof(short) || sourceType == typeof(sbyte) || sourceType == typeof(char);

			if (targetType == typeof(short))
				return sourceType == typeof(sbyte) || sourceType == typeof(char);

			if (targetType == typeof(ulong))
				return sourceType == typeof(uint) || sourceType == typeof(ushort) || sourceType == typeof(byte) || sourceType == typeof(char);

			if (targetType == typeof(uint))
				return sourceType == typeof(ushort) || sourceType == typeof(byte) || sourceType == typeof(char);

			if (targetType == typeof(ushort))
				return sourceType == typeof(byte) || sourceType == typeof(char);

			return false;
		}
	}
}
