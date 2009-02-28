using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Otis.Parsing;

namespace Otis
{
	/// <summary>
	/// Describes a transformation for one field/property within a type transformation
	/// </summary>
	public class MemberMappingDescriptor
	{
		private string m_member;
		private string m_expression;
		private string m_format;
		private string m_nullValue;
		private Type m_type;
		private Type m_ownerType;
		private bool m_isArray;
		private bool m_isList;
		private AggregateMappingDescription m_aggregateDescriptor = null;
		ProjectionInfo m_projections = new ProjectionInfo();
		List<string> m_nullableParts = new List<string>();

		public MemberMappingDescriptor()
		{
		}

		public MemberMappingDescriptor(MemberMappingDescriptor copyFrom)
		{
			m_member = copyFrom.m_member;
			m_expression = copyFrom.m_expression;
			m_format = copyFrom.m_format;
			m_nullValue = copyFrom.m_nullValue;
			m_type = copyFrom.m_type;
			m_ownerType = copyFrom.m_ownerType;
			m_isArray = copyFrom.m_isArray;
			m_isList = copyFrom.m_isList;
			m_projections = copyFrom.m_projections;
			m_nullableParts = copyFrom.m_nullableParts;
		}

		/// <summary>
		/// Gets/sets name of the mapped field or property on the target class of transformation.
		/// </summary>
		public string Member
		{
			get { return m_member; }
			set { m_member = value; }
		}

		/// <summary>
		/// Gets/sets name of the expression to which <see cref="Member"/> is mapped.
		/// </summary>
		public string Expression
		{
			get { return m_expression; }
			set
			{
				m_expression = ExpressionParser.NormalizeExpression(value.Trim());
				UpdateNullableParts(m_expression);
			}
		}

		/// <summary>
		/// Gets/sets the value which will be assigned to target <see cref="Member"/> if the
		/// <see cref="Expression"/> is null
		/// </summary>
		public string NullValue
		{
			get { return m_nullValue; }
			set
			{
				if (value == null)
					m_nullValue = null;
				else
					m_nullValue = ExpressionParser.NormalizeExpression(value.Trim());
			}
		}

		/// <summary>
		/// Gets/sets the formatting string for dtring members
		/// </summary>
		public string Format
		{
			get { return m_format; }
			set { m_format = value; }
		}

		/// <summary>
		/// Gets/sets the type of the target member
		/// </summary>
		public Type Type
		{
			get { return m_type; }
			set { m_type = value; }
		}

		/// <summary>
		/// Gets/sets the type where the memeber is defined
		/// </summary>
		public Type OwnerType
		{
			get { return m_ownerType; }
			set { m_ownerType = value; }
		}

		/// <summary>
		/// Gets/sets whether the target member is an array
		/// </summary>
		public bool IsArray
		{
			get { return m_isArray; }
			set { m_isArray = value; }
		}

		/// <summary>
		/// Gets/sets whether the target member is an implementation of <see cref="ICollection{T}"/>
		/// </summary>
		public bool IsList
		{
			get { return m_isList; }
			set { m_isList = value; }
		}

		/// <summary>
		/// returns whether the mapping has the formatting string attached
		/// </summary>
		public bool HasFormatting
		{
			get { return !string.IsNullOrEmpty(m_format); }
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
				if (m_aggregateDescriptor == null)
					m_aggregateDescriptor = new AggregateMappingDescription(Expression, Type);

				return m_aggregateDescriptor;
			}
		}

		/// <summary>
		/// Returns the projections
		/// </summary>
		public ProjectionInfo Projections
		{
			get { return m_projections; }
			set { m_projections = value; }
		}
																	   
		/// <summary>
		/// Returns the list of nullable parts in the mapped expression
		/// </summary>
		public List<string> NullableParts
		{
			get { return m_nullableParts; }
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
			m_nullableParts.Clear();
			Regex regex = new Regex(@"(\$[A-Za-z]\w*(\(.*?\))?)");
			MatchCollection matches = regex.Matches(expression);
			if (matches.Count == 0)
				return;

			foreach (Match match in matches)
			{
				m_nullableParts.Add(match.Value);
			}

		}
	}
}