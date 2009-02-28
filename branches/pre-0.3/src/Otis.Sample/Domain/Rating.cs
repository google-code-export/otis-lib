using System;
using System.Collections.Generic;
using System.Text;

namespace Otis.Sample.Domain
{
	public class Rating
	{
		private int m_value;
		private string m_ratedBy;

		public Rating(int value, string ratedBy)
		{
			m_value = value;
			m_ratedBy = ratedBy;
		}

		public Rating(int value)
		{
			m_value = value;
			m_ratedBy = "anonymous";
		}

		public int Value
		{
			get { return m_value; }
			set { m_value = value; }
		}

		public string RatedBy
		{
			get { return m_ratedBy; }
			set { m_ratedBy = value; }
		}
	}
}
