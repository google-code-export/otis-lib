using System;
using System.Collections.Generic;
using System.Text;

namespace Otis.Sample.Domain
{
	public class Comment
	{
		private string m_author;
		private string m_content;

		public Comment(string author, string content)
		{
			m_author = author;
			m_content = content;
		}

		public string Author
		{
			get { return m_author; }
			set { m_author = value; }
		}

		public string Content
		{
			get { return m_content; }
			set { m_content = value; }
		}
	}
}
