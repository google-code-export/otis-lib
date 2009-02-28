/*
 * Created by: Zdeslav Vojkovic
 * Created: Sunday, March 02, 2008
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class ExternalAssemblyTest
	{

		[Test]
		public void Recognize_External_Assemblies_For_Base_Class()
		{
			Configuration cfg = new Configuration();
			//cfg.AddAssemblyReference("nunit.core.dll");	not needed, but can be done
			//cfg.AddAssemblyReference("nunit.framework.dll");

			cfg.AddType<ExternalDependency>();
			cfg.BuildAssemblers();
		}

		[Test]
		public void Recognize_External_Assemblies_For_Implemented_Interfaces()
		{
			Configuration cfg = new Configuration();
			cfg.AddType<ExternalDependency2>();
			cfg.BuildAssemblers();
		}

		[Test]
		public void Assembly_Can_Be_Added_Explicitly_Via_Path()
		{
			MyConfiguration cfg = new MyConfiguration();
			Assert.IsFalse(cfg.ContainsAssembly("nunit.framework"));
			cfg.AddAssemblyReference("nunit.framework.dll");
			Assert.IsTrue(cfg.ContainsAssembly("nunit.framework.dll"));
		}

		[Test]
		public void Assembly_Can_Be_Added_Explicitly_Via_Assembly_Name()
		{
			MyConfiguration cfg = new MyConfiguration();
			Assert.IsFalse(cfg.ContainsAssembly("nunit.framework"));

			AssemblyName assemblyName =
		 new AssemblyName(typeof(TestAttribute).Assembly.FullName);

			cfg.AddAssemblyReference(assemblyName);
			Assert.IsTrue(cfg.ContainsAssembly("nunit.framework"));
		}

		[Test]
		public void Assembly_Can_Be_Added_Explicitly_Via_Assembly()
		{
			MyConfiguration cfg = new MyConfiguration();
			Assert.IsFalse(cfg.ContainsAssembly("nunit.framework"));

			cfg.AddAssemblyReference(typeof(TestAttribute).Assembly);
			Assert.IsTrue(cfg.ContainsAssembly("nunit.framework"));
		}


	}

	[MapClass(typeof(User))]
	public class ExternalDependency : AssertionHelper
	{
		[Map("count:$Documents", Format = "{0} documents")]
		public string DocumentInfo;
	}

	[MapClass(typeof(User))]
	public class ExternalDependency2 : IExpectException
	{
		[Map("count:$Documents", Format = "{0} documents")]
		public string DocumentInfo;

		public void HandleException(Exception ex)
		{
			throw new NotImplementedException();
		}
	}

	class MyConfiguration : Configuration
	{
		public bool ContainsAssembly(string name)
		{
			foreach (string referencedAssembly in ReferencedAssemblies)
			{
				if (referencedAssembly.Contains(name))
					return true;
			}

			return false;
		}
	}
}

