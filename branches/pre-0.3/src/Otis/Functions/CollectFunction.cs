using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using Otis.CodeGen;

namespace Otis.Functions
{
	public class CollectFunction : SimpleFunctionBase
	{
		private bool m_isArray;
		private string m_expression;

		public override IEnumerable<CodeStatement> GetInitializationStatements(AggregateFunctionContext context)
		{
			MemberMappingDescriptor member = context.Member;
			Type resultType;
			Type instanceType;
			bool isSimpleMapping = false;
			if (member.IsArray || member.IsList)
			{
				instanceType = member.IsArray ?
					member.AggregateMappingDescription.TargetType.GetElementType() :
					member.AggregateMappingDescription.TargetType.GetGenericArguments()[0];
				resultType = typeof(List<>).MakeGenericType(instanceType);

				if (!instanceType.IsPrimitive && instanceType != typeof(string))
				{
					m_expression = string.Format("{{0}}.Add({0}.AssembleFrom({{1}}));", GetAssemblerName(context));
				}
				else
				{
					m_expression = "{0}.Add({1});";
					isSimpleMapping = true;
				}
			}
			else
			{
				string msg = ErrorBuilder.CantAggregateOverNoncollectionError(member, "collect");
				throw new OtisException(msg); //todo: test
			}

			m_isArray = member.IsArray;
			List<CodeStatement> statements = new List<CodeStatement>();
			
			CodeExpression[] parameters = new CodeExpression[0];
			CodeStatement st = new CodeVariableDeclarationStatement(resultType,
				context.FunctionObjectName,
				new CodeObjectCreateExpression(resultType, parameters));

			statements.Add(st);

			if(!isSimpleMapping)
			{
				st = new CodeVariableDeclarationStatement(
					string.Format("IAssembler<{0}, {1}>", TypeHelper.GetTypeDefinition(instanceType), TypeHelper.GetTypeDefinition(context.SourceItemType)),
					GetAssemblerName(context),
					new CodeSnippetExpression("this"));
				statements.Add(st);
			}

			return statements;
		}

		override public CodeStatement GetAssignmentStatement(AggregateFunctionContext context)
		{
			string resultExpression = m_isArray ? string.Format("{0}.ToArray()", context.FunctionObjectName) : context.FunctionObjectName;

			return new CodeAssignStatement(
					new CodeVariableReferenceExpression("target." + context.Member.Member),
					new CodeArgumentReferenceExpression(resultExpression));
		}

		protected override string GetFormatForExecutedExpression(AggregateFunctionContext context)
		{
			// todo: test
			return m_expression;
		}

		private static string GetAssemblerName(AggregateFunctionContext context)
		{
			return context.FunctionObjectName + "_Asm";
		}

	}
}
