using System;
using System.Collections.Generic;
using Otis.Parsing;

namespace Otis.Generation
{
	public class AggregateFunctionContext
	{
		private readonly MemberMappingDescriptor _member;
		private readonly ClassMappingDescriptor _descriptor;
		private readonly Type _implementationType;
		private readonly string _functionObjectName;
		private readonly AggregateExpressionPathItem _sourceItem;
		private readonly IAggregateFunctionCodeGenerator _generator;
		private readonly IList<AggregateExpressionPathItem> _pathItems;

		public AggregateFunctionContext(MemberMappingDescriptor member, 
										ClassMappingDescriptor descriptor, 
										Type implementationType, 
										string functionObjectName, 
										IAggregateFunctionCodeGenerator generator)
		{
			_member = member;
			_generator = generator;
			_functionObjectName = functionObjectName;
			_implementationType = implementationType;
			_descriptor = descriptor;

			_pathItems = ExpressionParser.BuildAggregatePathItem(descriptor, member);
			_sourceItem = _pathItems[_pathItems.Count - 1];
		}

		public MemberMappingDescriptor Member
		{
			get { return _member; }
		}

		public ClassMappingDescriptor Descriptor
		{
			get { return _descriptor; }
		}

		public Type SourceItemType
		{
			get { return _sourceItem.Type; }
		}

		public Type ImplementationType
		{
			get { return _implementationType; }
		}

		public string FunctionObjectName
		{
			get { return _functionObjectName; }
		}

		public AggregateExpressionPathItem SourceItem
		{
			get { return _sourceItem; }
		}

		public IAggregateFunctionCodeGenerator Generator
		{
			get { return _generator; }
		}

		public IList<AggregateExpressionPathItem> PathItems
		{
			get { return _pathItems; }
		}
	}
}