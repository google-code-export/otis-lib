using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Otis.Parsing;
using Otis.Utils;

namespace Otis
{
	/// <summary>
	/// Describes a transformation for one field/property within a type transformation
	/// </summary>
	public class MemberMappingDescriptor
	{
		private string _expression;
		private string _nullValue;
		private AggregateMappingDescription _aggregateDescriptor = null;

		private string _member;
		private string _format;
		private Type _type;
		private Type _ownerType;
		private Type _sourceType;
		private Type _sourceOwnerType;
		private List<string> _nullableParts;
		private ProjectionInfo _projections;

		public MemberMappingDescriptor()
		{
			NullableParts = new List<string>();
			Projections = new ProjectionInfo();
		}

		public MemberMappingDescriptor(MemberMappingDescriptor copyFrom)
		{
			Member = copyFrom.Member;
			_expression = copyFrom._expression;
			Format = copyFrom.Format;
			_nullValue = copyFrom._nullValue;
			Type = copyFrom.Type;
			OwnerType = copyFrom.OwnerType;
			Projections = copyFrom.Projections;
			NullableParts = copyFrom.NullableParts;
			SourceType = copyFrom.SourceType;
			SourceOwnerType = copyFrom.SourceOwnerType;
		}

		/// <summary>
		/// Gets/sets name of the mapped field or property on the target class of transformation.
		/// </summary>
		public string Member
		{
			get { return _member; }
			set { _member = value; }
		}

		/// <summary>
		/// Gets/sets name of the expression to which <see cref="Member"/> is mapped.
		/// </summary>
		public string Expression
		{
			get { return _expression; }
			set
			{
				_expression = ExpressionParser.NormalizeExpression(value.Trim());
				UpdateNullableParts(_expression);
			}
		}

		/// <summary>
		/// Gets/sets the value which will be assigned to target <see cref="Member"/> if the
		/// <see cref="Expression"/> is null
		/// </summary>
		public string NullValue
		{
			get { return _nullValue; }
			set
			{
				if (value == null)
					_nullValue = null;
				else
					_nullValue = ExpressionParser.NormalizeExpression(value.Trim());
			}
		}

		/// <summary>
		/// Gets/sets the formatting string for dtring members
		/// </summary>
		public string Format
		{
			get { return _format; }
			set { _format = value; }
		}

		/// <summary>
		/// Gets/sets the type of the target member
		/// </summary>
		public Type Type
		{
			get { return _type; }
			set { _type = value; }
		}

		/// <summary>
		/// Gets/sets the type of the target member's singluar form if it is an Enumerable type
		/// </summary>
		public Type SingularType
		{
			get
			{
				return TypeHelper.GetSingularType(Type);
			}
		}

		/// <summary>
		/// Gets/sets the type where the memeber is defined
		/// </summary>
		public Type OwnerType
		{
			get { return _ownerType; }
			set { _ownerType = value; }
		}

		/// <summary>
		/// Gets/sets the type of the source member, or null if complex expression
		/// </summary>
		public Type SourceType
		{
			get { return _sourceType; }
			set { _sourceType = value; }
		}

		/// <summary>
		/// Gets/sets the type of the source member's singluar form if it is an Enumerable type
		/// </summary>
		public Type SourceSingluarType
		{
			get
			{
				return TypeHelper.GetSingularType(SourceType);
			}
		}


		/// <summary>
		/// Gets/sets the type of the source where source member is defined
		/// </summary>
		public Type SourceOwnerType
		{
			get { return _sourceOwnerType; }
			set { _sourceOwnerType = value; }
		}

		/// <summary>
		/// Gets/sets whether the target member is an array
		/// </summary>
		public bool IsArray
		{
			get { return Type.IsArray; }
		}

		/// <summary>
		/// Gets/sets whether the target member is an implementation of <see cref="ICollection{T}"/>
		/// </summary>
		public bool IsList
		{
			get { return TypeHelper.IsList(Type); }
		}

		/// <summary>
		/// returns whether the mapping has the formatting string attached
		/// </summary>
		public bool HasFormatting
		{
			get { return !string.IsNullOrEmpty(Format); }
		}

		/// <summary>
		/// returns whether the mapped expression is a aggregate expression
		/// </summary>
		public bool IsAggregateExpression
		{
			get { return ExpressionParser.IsAggregateExpression(Expression); }
		}

		/// <summary>
		/// Returns the descriptor for aggregate expression
		/// </summary>
		public AggregateMappingDescription AggregateMappingDescription
		{
			get
			{
				if (_aggregateDescriptor == null)
					_aggregateDescriptor = new AggregateMappingDescription(Expression, Type);

				return _aggregateDescriptor;
			}
		}

		/// <summary>
		/// Returns the projections
		/// </summary>
		public ProjectionInfo Projections
		{
			get { return _projections; }
			set { _projections = value; }
		}

		/// <summary>
		/// Returns the list of nullable parts in the mapped expression
		/// </summary>
		public List<string> NullableParts
		{
			get { return _nullableParts; }
			private set { _nullableParts = value; }
		}

		/// <summary>
		/// returns whether there are parts in mapped expression which can have null value
		/// </summary>
		public bool HasNullableParts
		{
			get { return NullableParts.Count > 0; }
		}

		/// <summary>
		/// returns whether the mapping has a custom value if mapped expression is null
		/// </summary>
		public bool HasNullValue
		{
			get { return NullValue != null; }
		}

		private void UpdateNullableParts(string expression)
		{
			NullableParts.Clear();
			Regex regex = new Regex(@"(\$[A-Za-z]\w*(\(.*?\))?)");
			MatchCollection matches = regex.Matches(expression);
			if (matches.Count == 0)
				return;

			foreach (Match match in matches)
			{
				NullableParts.Add(match.Value);
			}

		}
	}
}