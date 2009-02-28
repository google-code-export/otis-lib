using System.Collections.Generic;

namespace Otis.Sample.Domain
{
	public class Record : NamedItem
	{
		private string m_description;
		private IList<Song> m_songs = new List<Song>();
		private int m_publishedYear;
		private string m_style = "Rock";

		public Record(int id, string name, int publishedYear)
			: base(id, name)
		{
			m_publishedYear = publishedYear;
		}

		public string Description
		{
			get { return m_description; }
			set { m_description = value; }
		}

		public IList<Song> Songs
		{
			get { return m_songs; }
			set { m_songs = value; }
		}

		public string YearPublished
		{
			get { return m_publishedYear.ToString(); }
		}

		public string Style
		{
			get { return m_style; }
			set { m_style = value; }
		}
	}
}