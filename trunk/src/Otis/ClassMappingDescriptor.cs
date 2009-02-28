using System;
using System.Collections.Generic;

namespace Otis
{
	/// <summary>
	/// Describes the single type transformation. E.g. <c>Entity.User</c>-><c>DTO.User</c>
	/// </summary>
	public class ClassMappingDescriptor
	{
		List<MemberMappingDescriptor> m_memberDescriptors = new List<MemberMappingDescriptor>();
		private Type m_targetType;
		private Type m_sourceType;

		private string m_mappingHelper;
		private bool m_isHelperStatic = true;
		private string m_mappingPreparer;
		private bool m_isPreparerStatic = true;
		
		/// <summary>
		/// Returns the list of member transformations. For each transformation from a source field/property
		/// to a target field/property, there is an <c>MemberMappingDescriptor</c> instance in the list.
		/// </summary>
		public IList<MemberMappingDescriptor> MemberDescriptors { get { return m_memberDescriptors; } }

		/// <summary>
		/// Gets/sets target type for transformation
		/// </summary>
		public Type TargetType
		{
			get { return m_targetType; }
			set { m_targetType = value; }
		}

		/// <summary>
		/// Gets/sets source type for transformation
		/// </summary>
		public Type SourceType
		{
			get { return m_sourceType; }
			set { m_sourceType = value; }
		}

		/// <summary>
		/// Gets/sets name of the mapping helper function. For details, see <see cref="MapClassAttribute"/>.
		/// </summary>
		public string MappingHelper
		{
			get { return m_mappingHelper; }
			set { m_mappingHelper = value; }
		}

		/// <summary>
		/// returns whether there is a helper function
		/// </summary>
		public bool HasHelper
		{
			get { return !string.IsNullOrEmpty(m_mappingHelper); }
		}

		/// <summary>
		/// Returns whether the helper function is static
		/// </summary>
		public bool IsHelperStatic
		{
			get { return m_isHelperStatic; }
			set { m_isHelperStatic = value; }
		}
		/// <summary>
		/// Gets/sets name of the mapping preparer function. For details, see <see cref="MapClassAttribute"/>.
		/// </summary>
		public string MappingPreparer
		{
			get { return m_mappingPreparer; }
			set { m_mappingPreparer = value; }
		}

		/// <summary>
		/// returns whether there is a preparer function
		/// </summary>
		public bool HasPreparer
		{
			get { return !string.IsNullOrEmpty(m_mappingPreparer); }
		}

		/// <summary>
		/// Returns whether the preparer function is static
		/// </summary>
		public bool IsPreparerStatic
		{
			get { return m_isPreparerStatic; }
			set { m_isPreparerStatic = value; }
		}

	}
}