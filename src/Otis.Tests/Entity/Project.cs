using System.Collections.Generic;

namespace Otis.Tests.Entity
{
	public class Project : NamedEntity
	{
		public Project(string name) : base(name)
		{
			
		}
		private IList<Task> _tasks = new List<Task>();

		public IList<Task> Tasks
		{
			get { return _tasks; }
			set { _tasks = value; }
		}
	}
}