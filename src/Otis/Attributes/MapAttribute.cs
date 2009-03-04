using System;

namespace Otis.Attributes
{
	/// <summary>
	/// Marks a field or property of a class as being mapped from another type.
	/// </summary>
	/// <remarks>
	/// This attribute is used to provide mapping metadata on target types in type transformation.
	/// The expression set in attribute is applied on source object and the resulting value is assigned
	/// to the mapped field/property of the target object.
	/// <example>
	/// An example of mapping using MapAttribute:
	/// <code>
	/// [MapClass(typeof(User), Helper = "Otis.Tests.Util.Convert")]
	/// public class AttributedUserDTO
	/// {
	/// 	private string m_fullName;
	/// 
	/// 	[Map("$Id")]        // simple mapping
	/// 	public int Id;
	/// 
	/// 	[Map]				// if source member name is omitted it is assumed to be equal to target member name
	/// 	public int Age;
	/// 
	/// 	[Map("$UserName.ToUpper()", NullValue = "[unknown]")]	 // null value can be set to anything
	/// 	public string UserName;
	/// 
	/// 	[Map("$FirstName + \" \" + $LastName")] // 
	/// 	public string FullName
	/// 	{
	/// 		get { return m_fullName; }
	/// 		set { m_fullName = value; }
	/// 	}
	/// 
	/// 	[Map("$Projects.Count")]
	/// 	public int ProjectCount;
	/// }
	/// </code>
	/// </example>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class MapAttribute : Attribute
	{
		private string _expression;
		private object _nullValue;
		private string _format;
		private string _projection;

		/// <summary>
		/// Instantiates new MapAttribute. Target member will be assigned the value of the source object member with 
		/// same name.
		/// <example>
		/// <code>
		/// [MapClass(typeof(User), Helper = "Otis.Tests.Util.Convert")]
		/// public class AttributedUserDTO
		/// {
		/// 	[Map] // expression is ommited, 'Id' property of the source object will be used
		/// 	public int Id;
		/// }
		/// </code>
		/// </example>
		/// </summary>
		public MapAttribute() { }
		
		/// <summary>
		/// Instantiates new MapAttribute
		/// </summary>
		/// <param name="expression">Expression to be applied on source object and assigned to the target member</param>
		public MapAttribute(string expression)
		{
			_expression = expression;
		}

		/// <summary>
		/// Gets/sets mapping expression
		/// </summary>
		public string Expression
		{
			get { return _expression; }
		}

		/// <summary>
		/// If mapping expression returns null, target member will be assigned the value specified by this property.
		/// Default is null.
		/// </summary>
		public object NullValue
		{
			get { return _nullValue; }
			set { _nullValue = value; }
		}

		/// <summary>
		/// If target member is a string, this property can be used to modify the result of conversion
		/// </summary>
		/// <remarks>
		/// This property can only be applied to members which are strings. 
		/// The content of this property is format string as used in <c>string.Format</c> method,
		/// where the source expression is used as first parameter:
		/// <code>
		/// class UserDTO
		/// {
		///		[Map("$BirthDate", Format="{0:D}")
		///		public string Birthday;
		/// }
		/// 
		/// void Test()
		/// {
		///		User u = ... // get user from somewher
		///		u.BirthDate	= new DateTime(1973, 10, 22);
		///		UserDTO dto = m_assembler.AssembleFrom(u);
		///		Assert.AreEqual("Monday, October 22, 1973", dto.Birthday);
		/// } 
		/// </code>
		/// </remarks>
		/// <exception cref="OtisException">
		/// Thrown if mapped member is not of string type
		/// </exception>
		public string Format
		{
			get { return _format; }
			set { _format = value; }
		}

		/// <summary>
		/// Defines projection mapping. Projection mapping maps set of values to
		/// another set of values
		/// </summary>
		/// <remarks>
		/// Projection mapping is used to map a set of to a different set of values,
		/// e.g. from one enumeration to another, or from integer code to textual description.
		/// Projection string contains semicolon delimited list of projections in form: 
		/// SOURCE => TARGET. Whitespace is ignored except inside the target value. E.g.
		/// expression  " 100 => Unknown user " maps value 100 to string "Unknown user" if target
		/// member is a string.
		/// <code>
		/// class UserDTO
		/// {
		///		[Map("$GenderCode", Projection=" W => Female; M => Male ")
		///		public Gender Gender;
		/// 
		///		[Map("$SecurityLevel", Projection=" 1 => Restricted; 2 => Normal; 3 => High ")
		///		public string Security;
		/// }
		/// 
		/// void Test()
		/// {
		///		User u = ... // get user from somewhere
		///		u.GenderCode = "W";
		/// 	u.SecurityLevel = 2;
		///		UserDTO dto = _assembler.AssembleFrom(u);
		///		Assert.AreEqual(Gender.Female, dto.Gender);
		/// 	Assert.AreEqual("Normal", dto.Security);
		/// } 
		/// </code>
		/// </remarks>
		/// <exception cref="OtisException">
		/// Thrown if source side of projection has multiple mappings. E.g:
		/// [Map("$SecurityLevel", Projection=" 1 => Restricted; 1 => Normal")
		/// </exception>
		public string Projection
		{
			get { return _projection; }
			set { _projection = value; }
		}

		internal bool HasProjection
		{
			get { return !string.IsNullOrEmpty(Projection); }
		}
	}
}