namespace Otis
{
	public class ProjectionItem
	{
		private string _from;
		private string _to;

		public ProjectionItem() {}


		public ProjectionItem(string from, string to)
		{
			_from = from.Trim();
			_to = to.Trim();
		}

		public string From
		{
			get { return _from; }
			set { _from = value.Trim(); }
		}

		public string To
		{
			get { return _to; }
			set { _to = value.Trim(); }
		}
	}
}