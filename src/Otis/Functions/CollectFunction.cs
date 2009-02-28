using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using Otis.CodeGen;

namespace Otis.Functions
{
	public class CollectFunction : SimpleFunctionBase
	{
		private bool _isArray;
		private string _expression;

		public override IEnumerable<CodeStatement> GetInitializationStatements(AggregateFunctionContext context)
		{
			MemberMappingDescriptor member = context.Member;
			bool isSimpleMapping = false;

			if (!member.IsArray && !member.IsList)
			{
				string msg = ErrorBuilder.CantAggregateOverNoncollectionError(member, "collect");
				throw new OtisException(msg); //todo: test
			}

			Type instanceType = member.SingularType;
			Type resultType = typeof(List<>).MakeGenericType(instanceType);

			if (!instanceType.IsPrimitive && instanceType != typeof(string))
			{
				_expression = string.Format("{{0}}.Add({0}.AssembleFrom({{1}}));", GetAssemblerName(context));
			}
			else
			{
				_expression = "{0}.Add({1});";
				isSimpleMapping = true;
			}
		
			_isArray = member.IsArray;
			List<CodeStatement> statements = new List<CodeStatement>();
			
			CodeExpression[] parameters = new CodeExpression[0];
			CodeStatement st = new CodeVariableDeclarationStatement(resultType,
				context.FunctionObjectName,
				new CodeObjectCreateExpression(resultType, parameters));

			statements.Add(st);

			string assemblerName = Util.GetAssemblerName(instanceType, context.SourceItemType);

			if(!isSimpleMapping)
			{
				st = new CodeVariableDeclarationStatement(
					string.Format("IAssembler<{0}, {1}>", TypeHelper.GetTypeDefinition(instanceType), TypeHelper.GetTypeDefinition(context.SourceItemType)),
					GetAssemblerName(context),
					new CodeObjectCreateExpression(assemblerName));
				statements.Add(st);
			}

			return statements;
		}

		override public CodeStatement GetAssignmentStatement(AggregateFunctionContext context)
		{
			string resultExpression = _isArray ? string.Format("{0}.ToArray()", context.FunctionObjectName) : context.FunctionObjectName;

			return new CodeAssignStatement(
					new CodeVariableReferenceExpression("target." + context.Member.Member),
					new CodeArgumentReferenceExpression(resultExpression));
		}

		protected override string GetFormatForExecutedExpression(AggregateFunctionContext context)
		{
			// todo: test
			return _expression;
		}

		private static string GetAssemblerName(AggregateFunctionContext context)
		{
			return context.FunctionObjectName + "_Asm";
		}

	}
}
