﻿namespace Otis.Tests
{
	public class DerivedXmlUserDTO : XmlUserDTO
	{
		private string _derivedProperty;

	   public string DerivedProperty
	   {
			get { return _derivedProperty; }
			set { _derivedProperty = value; }
	   }
	}
}
