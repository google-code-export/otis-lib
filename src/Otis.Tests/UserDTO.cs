using System;
using System.Collections.Generic;
using System.Text;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[MapClass(typeof(User))]
	public class UserDTO
	{
		//-----------------------------------------------------------------------------------------
		// simple mappings
		//-----------------------------------------------------------------------------------------

		[Map("$Id")]   
		public int Id;

		[Map]		
		public int Age;

		[Map("\"<NotAvailable>\"")] 
		public string UserName;

		[Map("$FirstName + \" \" + $LastName")] 
		public string FullName;

		/*[Map("$Boss.FirstName + ' ' + $Boss.LastName", NullValue = "")] */ //todo: if removed -> NullReferenceException
		public string BossName;

		//-----------------------------------------------------------------------------------------
		// entity mappings
		//-----------------------------------------------------------------------------------------

		[Map]
		public UserDTO Boss;

		//-----------------------------------------------------------------------------------------
		// string mappings
		//-----------------------------------------------------------------------------------------
		[Map(Format="{0:D}")]
		public string BirthDay;

		//-----------------------------------------------------------------------------------------
		// collection mappings
		//-----------------------------------------------------------------------------------------
		[Map("$Projects")]
		public ProjectDTO[] Projects;  // IList<S> -> T[]

		[Map("$Projects")]
		public IList<ProjectDTO> ProjectsCopy = new List<ProjectDTO>(); // IList<S> -> IList<T>

		[Map("$Documents")]
		public IList<DocumentDTO> Documents = new List<DocumentDTO>(); // S[] -> IList<T>

		[Map("$Documents")]
		public DocumentDTO[] DocumentsCopy; // S[] -> T[]

		//-----------------------------------------------------------------------------------------
		// complex mappings
		//-----------------------------------------------------------------------------------------
		[Map("count:$Documents")]
		public int DocumentCount;

		[Map("avg:$Projects/Tasks/Duration")]
		public double AverageTaskDuration;

		[Map("sum:$Projects/Tasks/Duration")]
		public double TotalTaskDuration;

		[Map("min:$Projects/Tasks/Duration")]
		public int MinTaskDuration;

		[Map("max:$Projects/Tasks/Duration")]
		public double MaxTaskDuration;

		[Map("count:$Projects/Tasks")]
		public int AllTasksCount;

		[Map("get:$Projects/Tasks/Duration")]
		public long[] AllTaskDurations; // int -> long

		[Map("get:$Projects/Tasks/Duration")]
		public IList<double> AllTaskDurationsList;	// int -> double

		[Map("get:$Projects/Tasks")]
		public TaskDTO[] AllTasks;

		[Map("get:$Projects/Tasks")]
		public IList<TaskDTO> AllTasksList;

		[Map("concat:$Documents/Name")]
		public string DocumentInfo;
	}

	[MapClass(typeof(Project))]
	public class ProjectDTO
	{
		[Map]
		public int Id;

		[Map]
		public string Name;

		[Map("$Tasks.Count")]
		public int TaskCount;
	}

	[MapClass(typeof(NamedEntity))]
	public class DocumentDTO
	{
		[Map("$Id.ToString() + \" - \" + $Name")]
		public string Description;
	}

	[MapClass(typeof(Task))]
	public class TaskDTO
	{
		[Map("$Duration * 60")]
		public int DurationInMinutes;
	}
}
