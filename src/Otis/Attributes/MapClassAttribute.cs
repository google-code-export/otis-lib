using System;

namespace Otis.Attributes
{
	/// <summary>
	/// Marks a class as a part of some type transformation.
	/// </summary>
	/// <remarks>
	/// If class is not marked with this attribute it will not be analyzed during configuration building.
	/// <example>
	/// An example of mapping using MapAttribute:
	/// <code>
	/// /// Defines the following transformation: AttributedUserDTO -> User
	/// [MapClass(typeof(User), Helper = "Otis.Tests.Util.Convert")]
	/// public class AttributedUserDTO
	/// {
	/// 	[Map("$Id")] 
	///		public int Id; 
	/// 
	/// 	[Map("$FirstName + \" \" + $LastName")] 
	/// 	public string FullName;
	/// }
	/// </code>
	/// </example>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class MapClassAttribute : Attribute
	{
		private string _preparer;
		private Type _sourceType;
		private string _assemblerBaseName;
		private string _assemblerName;
		
		/// <summary>
		/// Instantiates new instance. 
		/// </summary>
		/// <param name="sourceType">Defines the source type for the transformation.</param>
		public MapClassAttribute(Type sourceType)
		{
			SourceType = sourceType;
		}

		private string _helper;

		/// <summary>
		/// Sets the helper function for the mapping.
		/// </summary>
		/// <remarks>This function is called after other mappings are done, to provide support for more
		/// complex mappings which cannot be easily defined in mapping metadata. This can be a static 
		/// function, in which case full name has to be provided, including namespace, or a member 
		/// function of the target class, in which case only the name of the function is needed. 
		/// <para>
		/// Function has to be public, and only one helper is allowed per mapping. the signature of
		/// the function must be <c>void Function(ref Target target, ref Source source)</c>
		/// </para>
		/// <para>
		/// Helper function can also be assigned by marking a member of the target class with
		/// <see cref="MappingHelperAttribute"/>.
		/// </para>
		/// </remarks> 
		public string Helper
		{
			get { return _helper; }
			set { _helper = value; }
		}

		/// <summary>
		/// Sets the preparer function for the mapping.
		/// </summary>
		/// <remarks>This function is called before any other mappings are done, to enable custom
		/// preparation code to be run. E.g. you can check if some property of target has to be 
		/// initialized in some special way to allow assembler to do its work. This can be a static 
		/// function, in which case full name has to be provided, including namespace, or a member 
		/// function of the target class, in which case only the name of the function is needed. 
		/// <para>
		/// Function has to be public, and only one preparer is allowed per mapping. the signature of
		/// the function must be <c>void Function(ref Target target, ref Source source)</c>
		/// </para>
		/// <para>
		/// Preparer function can also be assigned by marking a member of the target class with
		/// <see cref="MappingPreparerAttribute"/>.
		/// </para>
		/// </remarks> 
		public string Preparer
		{
			get { return _preparer; }
			set { _preparer = value; }
		}

		public Type SourceType
		{
			get { return _sourceType; }
			private set { _sourceType = value; }
		}

		/// <summary>
		/// Gets/sets the AssemblerBaseName of this Assembler, and will use the associated AssemblerBaseType for generation
		/// </summary>
		public string AssemblerBaseName
		{
			get { return _assemblerBaseName; }
			set { _assemblerBaseName = value;}
		}

		/// <summary>
		/// Gets/sets the AssemblerName for this Assembler, must be valid Class Name when specifying
		/// </summary>
		public string AssemblerName
		{
			get { return _assemblerName; }
			set { _assemblerName = value; }
		}
	}
}