using System;
using System.Collections.Generic;
using Otis.Parsing;

namespace Otis.CodeGen
{
	public class AggregateFunctionContext
	{
		private readonly MemberMappingDescriptor m_member;
		private readonly ClassMappingDescriptor m_descriptor;
		private readonly Type m_implementationType;
		private readonly string m_functionObjectName;
		private readonly AggregateExpressionPathItem m_sourceItem;
		private readonly IAggregateFunctionCodeGenerator m_generator;
		private readonly IList<AggregateExpressionPathItem> m_pathItems;

		public AggregateFunctionContext(MemberMappingDescriptor member, 
										ClassMappingDescriptor descriptor, 
										Type implementationType, 
										string functionObjectName, 
										IAggregateFunctionCodeGenerator generator)
		{
			m_member = member;
			m_generator = generator;
			m_functionObjectName = functionObjectName;
			m_implementationType = implementationType;
			m_descriptor = descriptor;

			m_pathItems = ExpressionParser.BuildAggregatePathItem(descriptor, member);
			m_sourceItem = m_pathItems[m_pathItems.Count - 1];
		}

		public MemberMappingDescriptor Member
		{
			get { return m_member; }
		}

		public ClassMappingDescriptor Descriptor
		{
			get { return m_descriptor; }
		}

		public Type SourceItemType
		{
			get { return m_sourceItem.Type; }
		}

		public Type ImplementationType
		{
			get { return m_implementationType; }
		}

		public string FunctionObjectName
		{
			get { return m_functionObjectName; }
		}

		public AggregateExpressionPathItem SourceItem
		{
			get { return m_sourceItem; }
		}

		public IAggregateFunctionCodeGenerator Generator
		{
			get { return m_generator; }
		}

		public IList<AggregateExpressionPathItem> PathItems
		{
			get { return m_pathItems; }
		}
	}
}