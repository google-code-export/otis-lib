using System;
using Otis;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[MapClass(typeof(User), Helper = "Otis.Tests.Util.Convert")]
	public class AttributedUserDTO
	{
		private string m_fullName;

		[Map("$Id")]         // simple mapping
		public int Id;

		[Map]				// if source member name is omitted it is assumed to be equal to target member name
		public int Age;

		[Map("$UserName.ToUpper()", NullValue = "\"[unknown]\"")]	 // null value can be set to anything
		public string UserName;

		[Map("$FirstName + \" \" + $LastName", NullValue = "MISSING_NAME_PART")] 
		public string FullName
		{
			get { return m_fullName; }
			set { m_fullName = value; }
		}

		[Map("['Mr.' + $FirstName + ' ' + $LastName]")]
		public string Title;

		[Map("$Projects.Count")]
		public int ProjectCount;

		[Map("$UserGender", Projection = "[ 'M' => Male; 'W' => Female ]")]
		public Gender Gender;

		[Map("$UserGender", Projection = "[ 'M' => 'M'; 'W' => 'W' ]")]
		public string GenderCode;
	}

	public class Util
	{
		public static void Convert(ref AttributedUserDTO dto, ref User user)
		{}
		public static void Convert(ref XmlUserDTO dto, ref User user)
		{ }
	}

}