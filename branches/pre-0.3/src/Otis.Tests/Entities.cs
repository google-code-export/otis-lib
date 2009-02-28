using System;
using System.Collections.Generic;
using System.Text;

namespace Otis.Tests.Entity
{
	public abstract class Entity
	{
		private int m_id;
			
		public int Id
		{
			get { return m_id; }
			set { m_id = value; }
		}
	}

	public class NamedEntity : Entity
	{
		private string m_name;

		public NamedEntity()
		{
			m_name = "Unknown";
		}

		public NamedEntity(string name)
		{
			m_name = name;
		}

		public string Name
		{
			get { return m_name; }
			set { m_name = value; }
		}
	}

	public class User : Entity
	{
		private User m_boss;
		private string m_userName;
		private string m_firstName;
		private string m_lastName;
		private IList<Project> m_projects = new List<Project>();
		private int m_age;
		private DateTime m_birthDay;
		private NamedEntity[] m_documents;
		private string m_userGender;

		public User Boss
		{
			get { return m_boss; }
			set { m_boss = value; }
		}

		public string UserName
		{
			get { return m_userName; }
			set { m_userName = value; }
		}

		public string FirstName
		{
			get { return m_firstName; }
			set { m_firstName = value; }
		}

		public string LastName
		{
			get { return m_lastName; }
			set { m_lastName = value; }
		}

		public IList<Project> Projects
		{
			get { return m_projects; }
			set { m_projects = value; }
		}

		public int Age
		{
			get { return m_age; }
			set { m_age = value; }
		}

		public NamedEntity[] Documents
		{
			get { return m_documents; }
			set { m_documents = value; }
		}

		public DateTime BirthDay
		{
			get { return m_birthDay; }
			set { m_birthDay = value; }
		}

		public string UserGender
		{
			get { return m_userGender; }
			set { m_userGender = value; }
		}
	}

	public class Project : NamedEntity
	{
		public Project(string name) : base(name)
		{
			
		}
		private IList<Task> m_tasks = new List<Task>();

		public IList<Task> Tasks
		{
			get { return m_tasks; }
			set { m_tasks = value; }
		}
	}

	public class Task : NamedEntity
	{
		private int m_duration;

		public Task(){}

		public Task(int duration) { m_duration = duration; }

		public int Duration
		{
			get { return m_duration; }
			set { m_duration = value; }
		}
	}

	public class Company : NamedEntity
	{
		private IList<User> m_employees;
		private Registry m_registry;

		public IList<User> Employees
		{
			get { return m_employees; }
			set { m_employees = value; }
		}

		public Registry Registry
		{
			get { return m_registry; }
			set { m_registry = value; }
		}
	}

	public class Registry : Entity
	{
		private IList<User> m_employees;

		public IList<User> Employees
		{
			get { return m_employees; }
			set { m_employees = value; }
		}
	}

	public class DerivedUser : User
	{
	   private string m_derivedProperty;

	   public string DerivedProperty
	   {
	      get { return m_derivedProperty; }
	      set { m_derivedProperty = value; }
	   }
	}

	public class GenericEntity<T>
	{
	   private T m_id;
	   private int? m_nullableProperty;

	   public T Id
	   {
	      get { return m_id; }
	      set { m_id = value; }
	   }

	   public int? NullableProperty
	   {
	      get { return m_nullableProperty; }
	      set { m_nullableProperty = value; }
	   }
	}
}
