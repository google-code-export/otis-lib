/*
 * Created by: Zdeslav Vojkovic
 * Created: Monday, November 19, 2007
 */

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Otis.CodeGen;
using Otis.Functions;
using Otis.Tests.Dto;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class AggregateMappingTest
	{
		private IAssembler<UserDTO, User> _assembler;
		private User _user;

		[SetUp]
		public void Setup()
		{
			Configuration cfg;
			cfg = new Configuration();
			cfg.AddType<UserDTO>();
			cfg.AddType<ProjectDTO>(); // todo: should this be automatically detected
			cfg.AddType<DocumentDTO>(); // todo: should this be automatically detected
			cfg.AddType<TaskDTO>(); // todo: should this be automatically detected

			cfg.BuildAssemblers();

			_assembler = cfg.GetAssembler<IAssembler<UserDTO,User>>();
			_user = Helpers.CreateComplexUser();
		}

		[TearDown]
		public void TearDown() {}

		[Test]
		public void Builtin_Cummulative_Functions_Min_Max_Test()
		{
			UserDTO dto = _assembler.AssembleFrom(_user);
			Assert.AreEqual(1, dto.MinTaskDuration);
			Assert.AreEqual(6, dto.MaxTaskDuration);
		}

		[Test]
		public void Builtin_Cummulative_Functions_Count_Sum_Test()
		{
			UserDTO dto = _assembler.AssembleFrom(_user);
			Assert.AreEqual(2, dto.DocumentCount);
			Assert.AreEqual(17, dto.TotalTaskDuration);
			Assert.AreEqual(5, dto.AllTasksCount);

		}

		[Test]
		public void Builtin_Cummulative_Functions_Avg_Test()
		{
			UserDTO dto = _assembler.AssembleFrom(_user);
			Assert.AreEqual(17 / 5.0, dto.AverageTaskDuration, 0.00001);
		}

		[Test]
		public void Builtin_Aggregate_Functions_Collect_Test()
		{
			UserDTO dto = _assembler.AssembleFrom(_user);

			Assert.AreEqual(5, dto.AllTaskDurations.Length);
			Assert.AreEqual(5, dto.AllTaskDurationsList.Count);
			Assert.AreEqual(5, dto.AllTasks.Length);
			Assert.AreEqual(5, dto.AllTasksList.Count);

			int[] taskIDs = { 2, 4, 1, 4, 6 };
			for (int i = 0; i < 5; i++)
			{
				Assert.AreEqual(taskIDs[i], dto.AllTaskDurations[i]);
				Assert.AreEqual(taskIDs[i], dto.AllTaskDurationsList[i]);
				Assert.AreEqual(taskIDs[i] * 60, dto.AllTasks[i].DurationInMinutes);
				Assert.AreEqual(taskIDs[i] * 60, dto.AllTasksList[i].DurationInMinutes);
			};
		}

		[Test]
		public void Aggregate_Functions_Understand_Complex_Path_Items()
		{
			Configuration cfg;
			cfg = new Configuration();
			cfg.AddType<ComplexUserDTO>();
			cfg.BuildAssemblers();

			IAssembler<ComplexUserDTO, User> assembler = cfg.GetAssembler<IAssembler<ComplexUserDTO,User>>();
			ComplexUserDTO dto = assembler.AssembleFrom(_user);
			Assert.AreEqual(4, dto.AvgDocumentNameLength);
		}

		[Test]
		public void User_Defined_Aggregate_Functions()
		{
			Configuration cfg;
			cfg = new Configuration();
			cfg.AddType<SimpleUserDTO>();
			cfg.RegisterFunction("median", typeof(MedianFn));
			cfg.BuildAssemblers();

			IAssembler<SimpleUserDTO, User> assembler = cfg.GetAssembler<IAssembler<SimpleUserDTO,User>>();
			SimpleUserDTO dto = assembler.AssembleFrom(_user);
			Assert.AreEqual(4, dto.MedianTaskDuration);
			Assert.AreEqual(4, dto.MedianDocumentNameLength);
		}

		[Test]
		[ExpectedException(typeof(OtisException))]
		public void Mapping_To_Untyped_Collection_Fails()   // todo: why shouldn't this pass
		{
			Configuration cfg;
			cfg = new Configuration();
			cfg.AddType<TestDTO>();
			cfg.BuildAssemblers();
		}

		[Test]
		public void Collect_Fails_On_Noncollection_Target()
		{
			CollectFunction fn = new CollectFunction();
			MemberMappingDescriptor member = new MemberMappingDescriptor();
			member.OwnerType = typeof(AttributedUserDTO);
			member.Type = typeof(string);
			member.Member = "FullName";
			member.Expression = "sum:$Projects/Tasks";
			ClassMappingDescriptor desc = new ClassMappingDescriptor();
			desc.SourceType = typeof (User);

			AggregateFunctionContext ctxt = new AggregateFunctionContext(member, desc, null, "", null);

			try
			{
				fn.GetInitializationStatements(ctxt);
			}
			catch(OtisException e)
			{
				if (e.Message.Contains("Target member 'FullName' for 'collect' aggregate function must be an array or a collection"))
					return; // success
			}
			Assert.Fail("Tested method didn't throw an exception!");
		}
	}

	public class EntityWithUntypedCollection
	{
		public ArrayList Users;
	}

	[MapClass(typeof(EntityWithUntypedCollection))]
	public class TestDTO
	{
		[Map("count:$Users")]
		public int UserCount;
	}

	[MapClass(typeof(User))]
	public class SimpleUserDTO
	{
		[Map("median:$Projects/Tasks/Duration")] 
		public int MedianTaskDuration;

		[Map("median:$$Documents/Name.Length")] 
		public int MedianDocumentNameLength;
	}

	[MapClass(typeof(User))]
	public class ComplexUserDTO
	{
		[Map("avg:$Documents/Name.Length")]
		public int AvgDocumentNameLength;
	}
				 
	public class MedianFn : IAggregateFunction<int> 
	{
		List<int> _values = new List<int>();

		public void Initialize(int initialValue) {} // ignore the initial value
		public void ProcessValue(int value) { _values.Add(value); }
		public int ProcessedItemCount { get { return _values.Count; } }
		public string ExpressionFormat { get { return null; } }

		public int Result
		{
			get
			{
				_values.Sort();
				return _values[ProcessedItemCount / 2];
			}
		}
	}	 
}

