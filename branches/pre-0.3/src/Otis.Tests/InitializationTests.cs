using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class InitializationTests
	{
		[Test]
		public void Read_Mapping_From_Assembly()
		{
			IMappingDescriptorProvider provider = ProviderFactory.FromType(typeof(AttributedUserDTO));
			Assert.AreEqual(1, provider.ClassDescriptors.Count);
			ClassMappingDescriptor desc = provider.ClassDescriptors[0];
			CheckDescription<AttributedUserDTO, User>(desc);
		}

		[Test]
		public void Read_Mapping_From_Xml()
		{
			IMappingDescriptorProvider provider = ProviderFactory.FromXmlFile("XmlMappings\\mappings.xml");

			Assert.AreEqual(2, provider.ClassDescriptors.Count);
			ClassMappingDescriptor desc = provider.ClassDescriptors[0];
			CheckDescription<XmlUserDTO, User>(desc);
		}

		private static void CheckDescription<Target, Source>(ClassMappingDescriptor desc) 
		{
			Assert.AreEqual(typeof(Target), desc.TargetType);
			Assert.AreEqual(typeof(Source), desc.SourceType);
			Assert.AreEqual("Otis.Tests.Util.Convert", desc.MappingHelper);

			Assert.AreEqual(8, desc.MemberDescriptors.Count);
			MemberMappingDescriptor member = null;

			member = Helpers.FindMember(desc.MemberDescriptors, "Id");
			Assert.AreEqual("$Id", member.Expression);
			Assert.AreEqual(null, member.NullValue );
			Assert.AreEqual(typeof(Target), member.OwnerType);
			Assert.AreEqual(typeof(int), member.Type);

			member = Helpers.FindMember(desc.MemberDescriptors, "Age");
			Assert.AreEqual("$Age", member.Expression);
			Assert.AreEqual(null, member.NullValue);
			Assert.AreEqual(typeof(Target), member.OwnerType);
			Assert.AreEqual(typeof(int), member.Type);

			member = Helpers.FindMember(desc.MemberDescriptors, "UserName");
			Assert.AreEqual("$UserName.ToUpper()", member.Expression);
			Assert.AreEqual("\"[unknown]\"", member.NullValue);
			Assert.AreEqual(typeof(Target), member.OwnerType);
			Assert.AreEqual(typeof(string), member.Type);

			member = Helpers.FindMember(desc.MemberDescriptors, "FullName");
			Assert.AreEqual("$FirstName + \" \" + $LastName", member.Expression);
			Assert.AreEqual("\"MISSING_NAME_PART\"", member.NullValue);
			Assert.AreEqual(typeof(Target), member.OwnerType);
			Assert.AreEqual(typeof(string), member.Type);

			member = Helpers.FindMember(desc.MemberDescriptors, "Title");
			Assert.AreEqual("\"Mr.\" + $FirstName + \" \" + $LastName", member.Expression);
			Assert.AreEqual(null, member.NullValue);
			Assert.AreEqual(typeof(Target), member.OwnerType);
			Assert.AreEqual(typeof(string), member.Type);

			member = Helpers.FindMember(desc.MemberDescriptors, "ProjectCount");
			Assert.AreEqual("$Projects.Count", member.Expression);
			Assert.AreEqual(null, member.NullValue);
			Assert.AreEqual(typeof(Target), member.OwnerType);
			Assert.AreEqual(typeof(int), member.Type);

			member = Helpers.FindMember(desc.MemberDescriptors, "Gender");
			Assert.AreEqual(2, member.Projections.Count);
			Assert.AreEqual("Otis.Tests.Gender.Male", member.Projections["\"M\""]);
			Assert.AreEqual("Otis.Tests.Gender.Female", member.Projections["\"W\""]);
			Assert.AreEqual(typeof(Target), member.OwnerType);
			Assert.AreEqual(typeof(Otis.Tests.Gender), member.Type);


			member = Helpers.FindMember(desc.MemberDescriptors, "GenderCode");
			Assert.AreEqual(2, member.Projections.Count);
			Assert.AreEqual("\"M\"", member.Projections["\"M\""]);
			Assert.AreEqual("\"W\"", member.Projections["\"W\""]);
		}

		[Test]
		public void Formatting_Is_Detected_In_Assembly_Mapping()
		{
			IMappingDescriptorProvider provider = ProviderFactory.FromType(typeof(UserDTO));
			ClassMappingDescriptor desc = provider.ClassDescriptors[0];
			MemberMappingDescriptor member = Helpers.FindMember(desc.MemberDescriptors, "BirthDay");

			Assert.IsTrue(member.HasFormatting);
			Assert.AreEqual("{0:D}", member.Format);
		}

		[Test]
		public void Formatting_Is_Detected_In_Xml_Mapping()
		{
			IMappingDescriptorProvider provider = ProviderFactory.FromXmlFile("XmlMappings\\mappings.xml");

			Assert.AreEqual(2, provider.ClassDescriptors.Count);
			ClassMappingDescriptor desc = provider.ClassDescriptors[1];
			MemberMappingDescriptor member = Helpers.FindMember(desc.MemberDescriptors, "BirthDay");

			Assert.IsTrue(member.HasFormatting);
			Assert.AreEqual("{0:D}", member.Format);
		}

		[Test]
		[ExpectedException(typeof(OtisException), 
			ExpectedMessage = "Xml configuration error: member 'Missing' does not exist in class 'Otis.Tests.UserDTO'")]
		public void Xml_Config_Fails_For_Non_Existing_Member()
		{
			string xmlCfg = "<otis-mapping xmlns=\"urn:otis-mapping-1.0\" >"
							+ "<class name=\"Otis.Tests.UserDTO, Otis.Tests\" source=\"Otis.Tests.Entity.User, Otis.Tests\" >"
			                + "<member name=\"Id\" />"
							+ "<member name=\"Age\" />"
							+ "<member name=\"Missing\" />"
							+ "</class>"
							+ "</otis-mapping>";
			IMappingDescriptorProvider provider = ProviderFactory.FromXmlString(xmlCfg);
		}

		[Test]
		[ExpectedException(typeof(OtisException),
		   ExpectedMessage = "Xml configuration error: formatting is applied to member 'Otis.Tests.UserDTO.Id' which is not a string but 'System.Int32'")]
		public void Xml_Config_Fails_When_Non_String_Member_Has_Formatting()
		{
			string xmlCfg = "<otis-mapping xmlns=\"urn:otis-mapping-1.0\" >"
							+ "<class name=\"Otis.Tests.UserDTO, Otis.Tests\" source=\"Otis.Tests.Entity.User, Otis.Tests\" >"
							+ "<member name=\"Id\" format=\"{0}\" />"
							+ "</class>"
							+ "</otis-mapping>";
			IMappingDescriptorProvider provider = ProviderFactory.FromXmlString(xmlCfg);
		}

		[Test]
		[ExpectedException(typeof(OtisException),
		  ExpectedMessage = "Xml configuration error: formatting is applied to member 'Otis.Tests.InvalidDTO2.Age' which is not a string but 'System.Int32'")]
		public void Type_Config_Fails_When_Non_String_Member_Has_Formatting()
		{
			IMappingDescriptorProvider provider = ProviderFactory.FromType(typeof(InvalidDTO2));
		}
	}

	[MapClass(typeof(User))]
	class InvalidDTO2
	{
		[Map(Format = "{0}")]
		public int Age;
	}
}
