using System;
using System.Collections.Generic;
using Otis.Attributes;

namespace Otis.Descriptors
{
	/// <summary>
	/// Describes the single type transformation. E.g. <c>Entity.User</c>-><c>DTO.User</c>
	/// </summary>
	public class ClassMappingDescriptor
	{
		List<MemberMappingDescriptor> _memberDescriptors = new List<MemberMappingDescriptor>();
		private string _assemblerBaseName;
		private Type _targetType;
		private Type _sourceType;
		private NamedAssembler _assemblerName;
		private string _mappingHelper;
		private bool _isHelperStatic;
		private string _mappingPreparer;
		private bool _isPreparerStatic;

		public ClassMappingDescriptor()
		{
			_isPreparerStatic = true;
			_isHelperStatic = true;
		}

		/// <summary>
		/// Gets/sets the Base Assembler for this Class
		/// </summary>
		public string AssemblerBaseName
		{
			get { return _assemblerBaseName; }
			set { _assemblerBaseName = value; }
		}

		/// <summary>
		/// Returns the list of member transformations. For each transformation from a source field/property
		/// to a target field/property, there is an <c>MemberMappingDescriptor</c> instance in the list.
		/// </summary>
		public IList<MemberMappingDescriptor> MemberDescriptors { get { return _memberDescriptors; } }

		/// <summary>
		/// Gets/sets target type for transformation
		/// </summary>
		public Type TargetType
		{
			get { return _targetType; }
			set { _targetType = value; }
		}

		/// <summary>
		/// Gets/sets source type for transformation
		/// </summary>
		public Type SourceType
		{
			get { return _sourceType; }
			set { _sourceType = value; }
		}

		/// <summary>
		/// Gets/sets the name of the assembler in the Generated Assembly
		/// </summary>
		public NamedAssembler AssemblerName
		{
			get { return _assemblerName; }
			set { _assemblerName = value; }
		}

		public bool HasNamedAssembler
		{
			get { return _assemblerName != null; }
		}

		/// <summary>
		/// Gets/sets name of the mapping helper function. For details, see <see cref="MapClassAttribute"/>.
		/// </summary>
		public string MappingHelper
		{
			get { return _mappingHelper; }
			set { _mappingHelper = value; }
		}

		/// <summary>
		/// returns whether there is a helper function
		/// </summary>
		public bool HasHelper
		{
			get { return !string.IsNullOrEmpty(MappingHelper); }
		}

		/// <summary>
		/// Returns whether the helper function is static
		/// </summary>
		public bool IsHelperStatic
		{
			get { return _isHelperStatic; }
			set { _isHelperStatic = value; }
		}

		/// <summary>
		/// Gets/sets name of the mapping preparer function. For details, see <see cref="MapClassAttribute"/>.
		/// </summary>
		public string MappingPreparer
		{
			get { return _mappingPreparer; }
			set { _mappingPreparer = value; }
		}

		/// <summary>
		/// returns whether there is a preparer function
		/// </summary>
		public bool HasPreparer
		{
			get { return !string.IsNullOrEmpty(MappingPreparer); }
		}

		/// <summary>
		/// Returns whether the preparer function is static
		/// </summary>
		public bool IsPreparerStatic
		{
			get { return _isPreparerStatic; }
			set { _isPreparerStatic = value; }
		}
	}
}