using System;
using System.Collections.Generic;
using System.Text;

namespace Otis.Tests.Entity
{
	public abstract class Entity
	{
		private int _id;
			
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}
	}

	public class NamedEntity : Entity
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

	public class User : Entity
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

	public class Registry : Entity
	{
		private IList<User> _employees;

		public IList<User> Employees
		{
			get { return _employees; }
			set { _employees = value; }
		}
	}

	public class DerivedUser : User
	{
	   private string _derivedProperty;

	   public string DerivedProperty
	   {
	      get { return _derivedProperty; }
	      set { _derivedProperty = value; }
	   }
	}

	public class GenericEntity<T>
	{
	   private T _id;
	   private int? _nullableProperty;

	   public T Id
	   {
	      get { return _id; }
	      set { _id = value; }
	   }

	   public int? NullableProperty
	   {
	      get { return _nullableProperty; }
	      set { _nullableProperty = value; }
	   }
	}
}
