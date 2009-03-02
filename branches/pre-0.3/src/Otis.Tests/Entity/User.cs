using System;
using System.Collections.Generic;

namespace Otis.Tests.Entity
{
	public class User : EntityBase
	{
		private User _boss;
		private string _userName;
		private string _firstName;
		private string _lastName;
		private IList<Project> _projects = new List<Project>();
		private int _age;
		private DateTime _birthDay;
		private NamedEntity[] _documents;
		private string _userGender;

		public User Boss
		{
			get { return _boss; }
			set { _boss = value; }
		}

		public string UserName
		{
			get { return _userName; }
			set { _userName = value; }
		}

		public string FirstName
		{
			get { return _firstName; }
			set { _firstName = value; }
		}

		public string LastName
		{
			get { return _lastName; }
			set { _lastName = value; }
		}

		public IList<Project> Projects
		{
			get { return _projects; }
			set { _projects = value; }
		}

		public int Age
		{
			get { return _age; }
			set { _age = value; }
		}

		public NamedEntity[] Documents
		{
			get { return _documents; }
			set { _documents = value; }
		}

		public DateTime BirthDay
		{
			get { return _birthDay; }
			set { _birthDay = value; }
		}

		public string UserGender
		{
			get { return _userGender; }
			set { _userGender = value; }
		}
	}
}