using System;
using System.Collections.Generic;

namespace Otis.Sample.Domain
{
	public class Song : NamedItem
	{
		private int m_durationInSeconds;
		private string m_content;
		private IList<Comment> m_comments = new List<Comment>();
		private IList<Rating> m_ratings = new List<Rating>();

		public Song(string name, int durationInSeconds)
			: base(0, name)
		{
			m_durationInSeconds = durationInSeconds;
		}

		public int DurationInSeconds
		{
			get { return m_durationInSeconds; }
			set { m_durationInSeconds = value; }
		}

		public string Content
		{
			get { return m_content; }
			set { m_content = value; }
		}

		public IList<Comment> Comments
		{
			get { return m_comments; }
			set { m_comments = value; }
		}

		public IList<Rating> Ratings
		{
			get { return m_ratings; }
			set { m_ratings = value; }
		}
	}
}