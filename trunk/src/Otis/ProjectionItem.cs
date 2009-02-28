namespace Otis
{
	public class ProjectionItem
	{
		private string m_from;
		private string m_to;

		public ProjectionItem() {}


		public ProjectionItem(string from, string to)
		{
			m_from = from.Trim();
			m_to = to.Trim();
		}

		public string From
		{
			get { return m_from; }
			set { m_from = value.Trim(); }
		}

		public string To
		{
			get { return m_to; }
			set { m_to = value.Trim(); }
		}
	}
}