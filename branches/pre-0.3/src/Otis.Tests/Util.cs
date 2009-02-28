using System;
using System.Collections.Generic;
using System.Text;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	static class Helpers
	{
		public static User CreateComplexUser()
		{
			User ceo = new User();
			ceo.Id = 102;
			ceo.Age = 50;
			ceo.FirstName = "Mega";
			ceo.LastName = "Boss";
			ceo.UserName = "ceo";

			User boss = new User();
			boss.Id = 101;
			boss.Age = 40;
			boss.FirstName = "X";
			boss.LastName = "Y";
			boss.UserName = "xy";
			boss.Boss = ceo;

			User user = new User();
			user.Id = 100;
			user.Age = 33;
			user.FirstName = "Zdeslav";
			user.LastName = "Vojkovic";
			user.UserName = "zdeslavv";
			user.Projects.Add(new Project("proj1"));
			user.Projects.Add(new Project("proj2"));
			user.Projects.Add(new Project("proj3"));

			user.Projects[0].Tasks.Add(new Task(2));
			user.Projects[0].Tasks.Add(new Task(4));
			user.Projects[1].Tasks.Add(new Task(1));
			user.Projects[1].Tasks.Add(new Task(4));
			user.Projects[1].Tasks.Add(new Task(6));

			NamedEntity[] docs = new NamedEntity[2];
			docs[0] = new NamedEntity("doc1");
			docs[1] = new NamedEntity("doc2");
			docs[0].Id = 1000;
			docs[1].Id = 1001;
			user.Documents = docs;

			user.Boss = boss;

			return user;
		}

		public static MemberMappingDescriptor FindMember(IList<MemberMappingDescriptor> descriptors, string name)
		{
			List<MemberMappingDescriptor> copy = new List<MemberMappingDescriptor>(descriptors);
			return copy.Find(delegate(MemberMappingDescriptor d) { return d.Member == name; });
		}
	}
}
