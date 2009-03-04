using System;
using System.CodeDom;
using System.Collections.Generic;
using Otis.Descriptors;
using Otis.Parsing;
using Otis.Utils;

namespace Otis.Generation
{
	class AggregateExpressionBuilder
	{
		private List<CodeStatement> _statements = null;
		private List<AggregateFunctionContext> _contexts = null;

		public AggregateExpressionBuilder(ClassMappingDescriptor descriptor, ICollection<MemberMappingDescriptor> members, FunctionMap functionMap)
		{
			_contexts = new List<AggregateFunctionContext>(members.Count);

			foreach (MemberMappingDescriptor member in members)
			{
				_contexts.Add(CreateMemberContext(descriptor, member, functionMap));
			}
		}

		public CodeStatement[] GetStatements()
		{
			if (_statements == null)
				Generate();
			
			return _statements.ToArray();
		}

		private void Generate()
		{
			_statements = new List<CodeStatement>(50);
			if(_contexts.Count < 1)
				return;

			foreach (AggregateFunctionContext context in _contexts)
			{
				_statements.AddRange(context.Generator.GetInitializationStatements(context));		
			}

			_statements.AddRange(GetPathTraversalStatements());

			foreach (AggregateFunctionContext context in _contexts)
			{
				_statements.Add(context.Generator.GetAssignmentStatement(context));
			}
		}

		private IEnumerable<CodeStatement> GetPathTraversalStatements()
		{
			string exp = "";
			IList<AggregateExpressionPathItem> pathItems
				= ExpressionParser.BuildAggregatePathItem(_contexts[0].Descriptor, _contexts[0].Member);

			foreach (AggregateExpressionPathItem pathItem in pathItems)
			{
				if (pathItem.IsCollection)
				{
					exp += string.Format("foreach({0} {1} in {2}.{3})",
						TypeHelper.GetTypeDefinition(pathItem.Type),
						pathItem.Object,
						pathItem.Target,
						pathItem.Expression);
				}
			}

			exp += Environment.NewLine + "\t\t\t\t{" + Environment.NewLine;
			foreach (AggregateFunctionContext context in _contexts)
			{
				IEnumerable<string> itemExpressions = context.Generator.GetIterationStatements(context, context.PathItems);
				foreach (string itemExpression in itemExpressions)
				{
					exp += "\t\t\t\t\t";
					exp += itemExpression;
					if(!exp.EndsWith(";"))
						exp += ";";
					exp += Environment.NewLine; // todo: smarter way
				}
			}
			exp += "\t\t\t\t}";

			CodeConditionStatement ifStatement = new CodeConditionStatement(
				new CodeSnippetExpression(pathItems[0].Target + "." + pathItems[0].Expression + " != null"),
				new CodeStatement[] { new CodeExpressionStatement(new CodeSnippetExpression(exp)) },
				new CodeStatement[0]);

			
			CodeStatementCollection statements = new CodeStatementCollection();
			statements.Add(ifStatement);

			CodeStatement[] arr = new CodeStatement[statements.Count];
			statements.CopyTo(arr, 0);
			return arr;		
		}

		private Type GetSourceItemType(IList<AggregateExpressionPathItem> items, MemberMappingDescriptor member)
		{
			if (items == null || items.Count < 1)
			{
				string msg = ErrorBuilder.InvalidAggregatePathError(member);
				throw new OtisException(msg);
			}

			return items[items.Count - 1].Type;
		}

		private IAggregateFunctionCodeGenerator GetGeneratorImpl(Type implementationType, MemberMappingDescriptor member)
		{
			Type[] interfaces = implementationType.GetInterfaces();
			foreach (Type itf in interfaces) // first check if it has its own implementation of IAggregateFunctionCodeGenerator
			{
				if (itf == typeof (IAggregateFunctionCodeGenerator))
					return Activator.CreateInstance(implementationType, true) as IAggregateFunctionCodeGenerator;
			}

			foreach (Type itf in interfaces) // now check if it only implement IAggregateFunction
			{
				if (itf.IsGenericType && itf.GetGenericTypeDefinition() == typeof(IAggregateFunction<>))
					return new DefaultAggregateFunctionCodeGenerator();
			}

			string msg = ErrorBuilder.InvalidAggregatePathError(member);
			throw new OtisException(msg); // test
		}

		private AggregateFunctionContext CreateMemberContext(ClassMappingDescriptor descriptor, MemberMappingDescriptor member, FunctionMap functionMap)
		{
			Type implementationType = functionMap.GetTypeForFunction(member.AggregateMappingDescriptor.FunctionName);

			string functionObjectName = string.Format("_{0}_to_{1}_Fn_",
				member.AggregateMappingDescriptor.FunctionObject,
				member.Member);

			if (implementationType.IsGenericType)
			{
				if (member.IsArray || member.IsList)
				{
					Type instanceType = member.IsArray ?
						member.AggregateMappingDescriptor.TargetType.GetElementType() :
						member.AggregateMappingDescriptor.TargetType.GetGenericArguments()[0];
					implementationType = implementationType.MakeGenericType(instanceType);
				}
				else
				{
					implementationType = implementationType.MakeGenericType(member.AggregateMappingDescriptor.TargetType);
				}
			}

			IAggregateFunctionCodeGenerator generator = GetGeneratorImpl(implementationType, member);

			return new AggregateFunctionContext(member,
								descriptor,
								implementationType,
								functionObjectName,
								generator);
		}

	}
}
