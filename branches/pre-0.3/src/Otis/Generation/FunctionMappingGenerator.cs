using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using Otis.Parsing;

namespace Otis.Generation
{
	class FunctionMappingGenerator
	{
		public static CodeStatement[] CreateMappingStatements(ClassMappingDescriptor descriptor, CodeGeneratorContext context)
		{
			Dictionary<string, List<MemberMappingDescriptor>> aggregateGroups = new Dictionary<string, List<MemberMappingDescriptor>>();
			List<CodeStatement> statements = new List<CodeStatement>(20);
			foreach (MemberMappingDescriptor member in descriptor.MemberDescriptors)
			{
				if (member.IsAggregateExpression)
				{
					// group all agregates by expression to avoid multiple traversals over same path
					//string path = GetPath(member.Expression);
					string path = GetPath(descriptor, member);
					if(!aggregateGroups.ContainsKey(path))	
						aggregateGroups[path] = new List<MemberMappingDescriptor>(1);
					aggregateGroups[path].Add(member);
				}
				else
				{
					CodeStatement[] st = CreateNonAggregateMappingStatements(descriptor, member, context);
					if(member.HasNullValue)
					{
						CodeStatement[] falseStatements = st;
						CodeStatement[] trueStatements = new CodeStatement[1];
						trueStatements[0] = new CodeAssignStatement(
							new CodeVariableReferenceExpression("target." + member.Member),
							new CodeSnippetExpression(member.NullValue.ToString()));

						string checkExpression = GetNullablePartsCheckExpression(member);
						CodeExpression ifExpression = new CodeSnippetExpression(checkExpression);
																  
						st = new CodeStatement[1];
						st[0] = new CodeConditionStatement(ifExpression, trueStatements, falseStatements);    
					}
					statements.AddRange(st);
				}
			}

			foreach (List<MemberMappingDescriptor> group in aggregateGroups.Values)
			{
					CodeStatement[] st = CreateAggregateMappingStatements(descriptor, group, context);
					statements.AddRange(st);
			}

			return statements.ToArray();
		}

		private static string GetPath(ClassMappingDescriptor descriptor, MemberMappingDescriptor member)
		{
			// todo: optimize - don't do this for every member, this is also done in AggregateFunctionContext ctor
			IList<AggregateExpressionPathItem> pathItems = ExpressionParser.BuildAggregatePathItem(descriptor, member);
			bool isLastItemCollection = pathItems[pathItems.Count - 1].IsCollection;
			
			string path = member.Expression;
			int pos = path.IndexOf(':');
			
			if (pos >= 0)
				path = path.Substring(pos + 1);	 

			if (!isLastItemCollection)
			{
				pos = path.LastIndexOf('/');
				if (pos >= 0)
					path = path.Substring(0, pos);
			}

			return path;
		}

		private static CodeStatement[] CreateNonAggregateMappingStatements(ClassMappingDescriptor descriptor, MemberMappingDescriptor member, CodeGeneratorContext context)
		{
			if (member.Projections.Count > 0)
				return CreateProjectionMapping(member);

			if (member.Type == typeof(string) || member.Type.IsPrimitive)
				return CreateSimpleMapping(member);

			if (member.IsArray)
				return CreateArrayMappingStatements(member);

			if (member.IsList)
				return CreateListMappingStatements(member);

			return CreateAssemblerMapping(member);
		}

		private static CodeStatement[] CreateAggregateMappingStatements(ClassMappingDescriptor descriptor, ICollection<MemberMappingDescriptor> members, CodeGeneratorContext context)
		{
			/*if (!member.IsAggregateExpression)										  todo: remove
				throw new OtisException(string.Format("Expression '{0}' is not a aggregate expression", member.Expression)); */

			AggregateExpressionBuilder expBuilder = new AggregateExpressionBuilder(descriptor, members, context.FunctionMap);
			return expBuilder.GetStatements();
		}

		private static CodeStatement[] CreateAssemblerMapping(MemberMappingDescriptor member) 
		{
			string expression = UnwindExpression(member.Expression);

			CodeVariableReferenceExpression targetVariableReference =
				new CodeVariableReferenceExpression("target." + member.Member);

			CodeArgumentReferenceExpression expressionArgumentReference = new CodeArgumentReferenceExpression(expression);

			if(member.Type.IsPrimitive)
			{
				return new CodeStatement[] { new CodeAssignStatement(targetVariableReference, expressionArgumentReference) };
			}

			//else we want to call another assembler
			//TODO: check for named assembler
			string assemblerName = Util.GetAssemblerName(member.Type, member.SourceType);

			CodeMethodInvokeExpression transform =
				new CodeMethodInvokeExpression(
					new CodeObjectCreateExpression(assemblerName),
					"AssembleFrom",
					expressionArgumentReference);

			if(member.HasNullValue)
			{
				CodeStatement nullValueStatement = new CodeConditionStatement(
					new CodeBinaryOperatorExpression(
						expressionArgumentReference,
						CodeBinaryOperatorType.ValueEquality,
						new CodePrimitiveExpression(null)),
					new CodeStatement[]
						{
							new CodeAssignStatement(
								targetVariableReference,
								new CodeSnippetExpression(member.NullValue))
						},
					new CodeStatement[]
						{
							new CodeAssignStatement(
								targetVariableReference,
								transform)
						});

				return new CodeStatement[] { nullValueStatement };
			}

			return new CodeStatement[] { new CodeAssignStatement( targetVariableReference, transform) };
		}

		private static CodeStatement[] CreateSimpleMapping(MemberMappingDescriptor member)
		{
			string expression = UnwindExpression(member.Expression);

			if(member.Type == typeof(string))
			{
				expression = member.HasFormatting 
					? string.Format("string.Format(\"{0}\", {1})", member.Format, expression) 
					: string.Format("({0}).ToString()", expression);				
			}

			CodeSnippetExpression exp = new CodeSnippetExpression(expression);
			CodeStatement st = new CodeAssignStatement(
				new CodeVariableReferenceExpression("target." + member.Member),
				exp);

			return new CodeStatement[] { st };
		}

		private static CodeStatement[] CreateArrayMappingStatements(MemberMappingDescriptor member)
		{
			string expression = member.Expression.Replace("$", "source.");

			//TODO: check for named assembler
			string assemblerName = Util.GetAssemblerName(member.SingularType, member.SourceSingluarType);

			CodeAssignStatement transform =
				new CodeAssignStatement(
					new CodeVariableReferenceExpression("target." + member.Member),
					new CodeMethodInvokeExpression(
						new CodeObjectCreateExpression(assemblerName),
						"ToArray",
						new CodeArgumentReferenceExpression(expression)));

			return new CodeStatement[] { transform };
		}

		private static CodeStatement[] CreateListMappingStatements(MemberMappingDescriptor member)
		{
			string expression = member.Expression.Replace("$", "source.");

			//else we want to call another assembler
			//TODO: check for named assembler
			string assemblerName = Util.GetAssemblerName(member.SingularType, member.SourceSingluarType);

			CodeAssignStatement transform =
				new CodeAssignStatement(
					new CodeVariableReferenceExpression("target." + member.Member),
					new CodeMethodInvokeExpression(
						new CodeObjectCreateExpression(assemblerName),
						"ToList",
						new CodeArgumentReferenceExpression(expression)));

			return new CodeStatement[] { transform };
		}

		private static CodeStatement[] CreateProjectionMapping(MemberMappingDescriptor member)
		{
			
			StringBuilder snippet = new StringBuilder();
			snippet.AppendLine(string.Format("switch({0}) {{", UnwindExpression(member.Expression)));

			foreach (ProjectionItem item in member.Projections.Items)
			{
				string resultExpression;
				if(member.HasFormatting)
					resultExpression = string.Format("string.Format(\"{0}\", {1})", member.Format, item.To);
				else
					resultExpression = item.To;

				snippet.AppendLine(string.Format("	case {0}: target.{1} = {2}; break;", item.From, member.Member, resultExpression));
			}

			snippet.AppendLine("}");

			CodeStatementCollection coll = new CodeStatementCollection();
			coll.Add(new CodeSnippetExpression(snippet.ToString()));
			return new CodeStatement[1] { coll[0] };
		}

		private static string UnwindExpression(string expression)
		{
			return expression.Replace("$", "source.");
		}

		private static string GetNullablePartsCheckExpression(MemberMappingDescriptor member)
		{
			StringBuilder sb = new StringBuilder(20);
			foreach (string nullablePart in member.NullableParts)
			{
				sb.Append(nullablePart.Replace("$", "source."));
				sb.Append(" == null || ");
			}
			return sb.ToString(0, sb.Length - 4);
		}
	}
}