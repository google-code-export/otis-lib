namespace Otis.Tests.Entity
{
	public class DerivedUser : User
	{
		private string _derivedProperty;

		public string DerivedProperty
		{
			get { return _derivedProperty; }
			set { _derivedProperty = value; }
		}
	}
}