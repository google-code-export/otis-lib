using System;
using System.Collections.Generic;
using System.Text;

namespace Otis.Sample.Domain
{
	public class Artist : NamedItem
	{
		private string m_country;
		private IList<Record> m_records = new List<Record>();

		public Artist(int id, string name, string country)
			: base(id, name)
		{
			m_country = country;
		}

		public IList<Record> Records
		{
			get { return m_records; }
			set { m_records = value; }
		}

		public string Country
		{
			get { return m_country; }
			set { m_country = value; }
		}
	}
}
