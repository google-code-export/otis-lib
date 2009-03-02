namespace Otis.Tests.Entity
{
	public abstract class EntityBase
	{
		private int _id;
			
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}
	}
}