/*
 * Created by: Zdeslav Vojkovic
 * Created: Thursday, February 07, 2008
 */

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class PerformanceTest
	{
		const int ITER_COUNT = 100000;

		//[Test]
		public void ComparePerformance()
		{
			User user = Helpers.CreateComplexUser();

			// add more tasks
			user.Projects[0].Tasks.Add(new Task(5));
			user.Projects[0].Tasks.Add(new Task(4));
			user.Projects[0].Tasks.Add(new Task(2));
			user.Projects[0].Tasks.Add(new Task(6));
			user.Projects[0].Tasks.Add(new Task(4));
			user.Projects[0].Tasks.Add(new Task(2));

			user.Projects[1].Tasks.Add(new Task(3));
			user.Projects[1].Tasks.Add(new Task(4));
			user.Projects[1].Tasks.Add(new Task(5));
			user.Projects[1].Tasks.Add(new Task(1));
			user.Projects[1].Tasks.Add(new Task(4));

			user.Projects[0].Tasks.Add(new Task(5));
			user.Projects[0].Tasks.Add(new Task(4));
			user.Projects[0].Tasks.Add(new Task(2));
			user.Projects[0].Tasks.Add(new Task(6));
			user.Projects[0].Tasks.Add(new Task(4));
			user.Projects[0].Tasks.Add(new Task(2));

			user.Projects[1].Tasks.Add(new Task(3));
			user.Projects[1].Tasks.Add(new Task(4));
			user.Projects[1].Tasks.Add(new Task(5));
			user.Projects[1].Tasks.Add(new Task(1));
			user.Projects[1].Tasks.Add(new Task(4));


			UserDTO dto = new UserDTO();

			Configuration cfg;
			cfg = new Configuration();
			cfg.AddType(typeof(UserDTO));
			cfg.AddType(typeof(ProjectDTO)); // todo: should this be automatically detected
			cfg.AddType(typeof(DocumentDTO)); // todo: should this be automatically detected
			cfg.AddType(typeof(TaskDTO)); // todo: should this be automatically detected

			cfg.BuildAssemblers();
			IAssembler<UserDTO, User> assembler = cfg.GetAssembler<IAssembler<UserDTO,User>>();

			assembler.Assemble(ref dto, ref user); // run it once

			long time = Environment.TickCount;
			for(int i = 0; i < ITER_COUNT; i++)
			{
				assembler.Assemble(ref dto, ref user);
			}
			long time1 = Environment.TickCount - time;
			Console.Out.WriteLine("Generated assembler timing = {0}", time1);

			Assemble(dto, user); // run it once
			time = Environment.TickCount;
			for (int i = 0; i < ITER_COUNT; i++)
			{
				Assemble(dto, user);
			}
			long time2 = Environment.TickCount - time;
			Console.Out.WriteLine("Manual assembler timing = {0}", time2);
			Console.Out.WriteLine("Ratio = {0}", time1/(double)time2);
		}

		private void Assemble(UserDTO dto, User user)
		{
			dto.Id = user.Id;
			dto.Age = user.Age;
			dto.UserName = "\"<NotAvailable>\"";
			dto.FullName = user.FirstName + " " + user.LastName;
			dto.BirthDay = string.Format("{0:D}", user.BirthDay);

			if(user.Boss != null)
			{
				Assemble(dto.Boss, user.Boss);
			}

			dto.Projects = new ProjectDTO[user.Projects.Count];
			dto.ProjectsCopy = new List<ProjectDTO>(user.Projects.Count);
			int i = 0;
			if (user.Projects != null)
				foreach (Project project in user.Projects)
				{
					dto.Projects[i] = AssembleFrom(project);
					dto.ProjectsCopy.Add(AssembleFrom(project));
					i++;
				}  

			int documentCount = user.Documents == null ? 0 : user.Documents.Length;
			dto.DocumentsCopy = new DocumentDTO[documentCount];
			dto.Documents = new List<DocumentDTO>(documentCount);
			StringBuilder sb = new StringBuilder();
						  
			if (user.Documents != null)
			{
				i = 0;
				foreach (NamedEntity doc in user.Documents)
				{
					dto.DocumentsCopy[i] = AssembleFrom(doc);
					dto.Documents.Add(AssembleFrom(doc));
					sb.Append(doc.Name);
					sb.Append(", ");
					i++;
				}
			}

			dto.DocumentInfo = sb.Length > 2 ? sb.ToString(0, sb.Length - 2) : "";
			dto.DocumentCount = documentCount;	  

			int min = int.MaxValue;
			int max = int.MinValue;
			double total = 0;
			int allCount = 0;
			if (user.Projects != null)
				foreach (Project project in user.Projects)
				{
					allCount += project.Tasks.Count;
				}
			
			dto.AllTaskDurations = new long[allCount];
			dto.AllTaskDurationsList = new List<double>(allCount);
			dto.AllTasks = new TaskDTO[allCount];
			dto.AllTasksList = new List<TaskDTO>(allCount);	
			 
			i = 0;
			if (user.Projects != null)
				foreach (Project project in user.Projects)
				{
					allCount += project.Tasks.Count;
					foreach (Task task in project.Tasks)
					{
						total = total + task.Duration;
						if (min > task.Duration)
							min = task.Duration;
						if (max < task.Duration)
							max = task.Duration;

						dto.AllTaskDurations[i] = task.Duration;
						dto.AllTaskDurationsList.Add(task.Duration);
						dto.AllTasks[i] = AssembleFrom(task);
						dto.AllTasksList.Add(AssembleFrom(task)); 
						i++;
					}
				}   
			
			dto.AverageTaskDuration = total / allCount;
			dto.TotalTaskDuration = total;
			dto.MinTaskDuration = min;
			dto.MaxTaskDuration = max;
			dto.AllTasksCount = allCount;	 
		}									 

		private ProjectDTO AssembleFrom(Project project)
		{
			ProjectDTO dto = new ProjectDTO();
			dto.Id = project.Id;
			dto.Name = project.Name;
			dto.TaskCount = project.Tasks.Count;
			return dto;
		}

		private DocumentDTO AssembleFrom(NamedEntity document)
		{
			DocumentDTO dto = new DocumentDTO();
			dto.Description = document.Id + " - " + document.Name;
			return dto;
		}

		private TaskDTO AssembleFrom(Task task)
		{
			TaskDTO dto = new TaskDTO();
			dto.DurationInMinutes = task.Duration * 60;
			return dto;
		}

	}
}

