using System;
using System.Collections.Generic;
using System.Text;
using Otis.Tests.Entity;

namespace Otis.Tests
{

	[MapClass(typeof(DerivedUser))]
	public class DerivedUserDTO : UserDTO
	{
      private string m_derivedProperty;

		[Map]
      public string DerivedProperty
      {
         get { return m_derivedProperty; }
         set { m_derivedProperty = value; }
      }
   }
}
