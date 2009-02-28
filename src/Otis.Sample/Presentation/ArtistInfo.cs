using System;
using System.Collections.Generic;
using System.Text;
using Otis.Sample.Domain;

namespace Otis.Sample.Presentation
{
	public class ArtistInfo
	{
		private int m_id;
		private string m_name;
		private int m_recordCount;
		private string m_description;
		private RecordInfo[] m_records;

		public int Id
		{
			get { return m_id; }
			set { m_id = value; }
		}

		public string Name
		{
			get { return m_name; }
			set { m_name = value; }
		}

		public int RecordCount
		{
			get { return m_recordCount; }
			set { m_recordCount = value; }
		}

		public string Description
		{
			get { return m_description; }
			set { m_description = value; }
		}

		public RecordInfo[] Records
		{
			get { return m_records; }
			set { m_records = value; }
		}
	}
}
