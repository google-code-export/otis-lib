namespace Otis.Tests.Entity
{
	public class NamedEntity : EntityBase
	{
		private string _name;

		public NamedEntity()
		{
			_name = "Unknown";
		}

		public NamedEntity(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	}
}