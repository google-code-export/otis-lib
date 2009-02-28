using System;
using System.Collections.Generic;
using System.Text;

namespace Otis.Sample.Domain
{
	public class NamedItem
	{
		private int m_id;
		private string m_name;

		public NamedItem(int id, string name)
		{
			m_id = id;
			m_name = name;
		}

		public int Id
		{
			get { return m_id; }
		}

		public string Name
		{
			get { return m_name; }
		}
	}
}
