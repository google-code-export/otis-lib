namespace Otis.Tests.Entity
{
	public class Task : NamedEntity
	{
		private int _duration;

		public Task(){}

		public Task(int duration) { _duration = duration; }

		public int Duration
		{
			get { return _duration; }
			set { _duration = value; }
		}
	}
}