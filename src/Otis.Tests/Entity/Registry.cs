using System.Collections.Generic;

namespace Otis.Tests.Entity
{
	public class Registry : EntityBase
	{
		private IList<User> _employees;

		public IList<User> Employees
		{
			get { return _employees; }
			set { _employees = value; }
		}
	}
}