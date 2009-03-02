using System.Collections.Generic;

namespace Otis.Tests.Entity
{
	public class Company : NamedEntity
	{
		private IList<User> _employees;
		private Registry _registry;

		public IList<User> Employees
		{
			get { return _employees; }
			set { _employees = value; }
		}

		public Registry Registry
		{
			get { return _registry; }
			set { _registry = value; }
		}
	}
}