using System;
using System.Collections.Generic;
using System.Text;
using Otis.Tests.Entity;

namespace Otis.Tests
{

	[MapClass(typeof(DerivedUser))]
	public class DerivedUserDTO : UserDTO
	{
		private string _derivedProperty;

		[Map]
      public string DerivedProperty
      {
			get { return _derivedProperty; }
			set { _derivedProperty = value; }
      }
   }
}
