using System;
using System.Collections.Generic;
using System.Text;

namespace Otis.Tests
{
	public class XmlUserDTO
	{
		public int Id;
		public int Age;
		public string UserName;
		public string FullName;
		public string Title;
		public int ProjectCount;
		public int AllTasksCount;
		public Gender Gender;
		public string GenderCode;
	}

	public enum Gender
	{
		Male,
		Female
	}
}
