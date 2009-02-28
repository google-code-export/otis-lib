using System;
using System.Collections.Generic;
using System.Text;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	public class XmlGenericEntityDTO
	{
      private int m_id;
      private int? m_nullableProperty;

      public int Id
      {
         get { return m_id; }
         set { m_id = value; }
      }

      public int? NullableProperty
      {
         get { return m_nullableProperty; }
         set { m_nullableProperty = value; }
      }
   }

	[MapClass(typeof(GenericEntity<int>))]
	public class AttributedGenericEntityDTO
	{
      private int m_id;
      private int? m_nullableProperty;

      [Map]
      public int Id
      {
         get { return m_id; }
         set { m_id = value; }
      }

		[Map]
      public int? NullableProperty
      {
         get { return m_nullableProperty; }
         set { m_nullableProperty = value; }
      }
	}
}
