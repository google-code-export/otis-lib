using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using Otis.Providers;

namespace Otis.Tests
{
	[TestFixture]
	public class ProjectionParsingTests
	{
		MemberMappingDescriptor m_desc;

		[SetUp]
		public void Setup()
		{		 
			m_desc = new MemberMappingDescriptor();
			m_desc.Member = "Gender";
			m_desc.OwnerType = typeof(XmlUserDTO);
			m_desc.Type = typeof(Gender);
		}
		[Test]
		public void Parse_Valid_Xml_Mapping()
		{
			string xmlCfg = "<otis-mapping>"
							+ "<class name=\"Otis.Tests.XmlUserDTO, Otis.Tests\" source=\"Otis.Tests.Entity.User, Otis.Tests\" >"
							+ "  <member name=\"Gender\" expression=\"$UserGender\">"
							+ "    <map from=\"['M']\" to=\"Male\" />"
							+ "    <map from=\"['W']\" to=\"Female\" />"
							+ "	 </member>"
							+ "  <member name=\"GenderCode\" expression=\"$UserGender\">"
							+ "    <map to=\"['M']\" />"
							+ "    <map to=\"['W']\" />"
							+ "  </member>;"
							+ "</class>"
							+ "</otis-mapping>";
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xmlCfg);
			XmlNodeList members = xmlDoc.SelectNodes("//member");

			XmlNodeList xmlGender = members[0].SelectNodes("map");
			ProjectionInfo projections = XmlMappingDescriptionProvider.BuildProjections(m_desc, xmlGender);
			Assert.AreEqual(2, projections.Count);
			Assert.AreEqual("Otis.Tests.Gender.Male", projections["\"M\""]);
			Assert.AreEqual("Otis.Tests.Gender.Female", projections["\"W\""]);

			m_desc.Type = typeof(string);
			m_desc.OwnerType = typeof (XmlUserDTO);
			m_desc.Member = "GenderCode";

			XmlNodeList xmlGenderCode = members[1].SelectNodes("map");	 
			projections = XmlMappingDescriptionProvider.BuildProjections(m_desc, xmlGenderCode);
			Assert.AreEqual(2, projections.Count);
			Assert.AreEqual("\"M\"", projections["\"M\""]);
			Assert.AreEqual("\"W\"", projections["\"W\""]);
		}

		[Test]
		[ExpectedException(typeof(OtisException))]
		public void Parse_Xml_Mapping_With_Empty_To_Attribute()
		{
			string xmlCfg = "<otis-mapping>"
							+ "<class name=\"Otis.Tests.XmlUserDTO, Otis.Tests\" source=\"Otis.Tests.Entity.User, Otis.Tests\" >"
							+ "  <member name=\"Gender\" expression=\"$UserGender\">"
							+ "    <map from=\"['M']\" to=\"\" />"
							+ "    <map from=\"['W']\" to=\"\" />"
							+ "  </member>;"
							+ "</class>"
							+ "</otis-mapping>";
			BuildXmlProjections(xmlCfg);
		}

		[Test]
		public void Parse_Attributes()
		{
			string basicExpression = "\"M\" => Male; \"W\" => Female";
			ProjectionInfo projections = SingleTypeMappingDescriptorProvider.GetProjections(m_desc, basicExpression);
			Check(projections);

			projections = SingleTypeMappingDescriptorProvider.GetProjections(m_desc, "['M' => Male; 'W' => Female]");
			Check(projections);

			projections = SingleTypeMappingDescriptorProvider.GetProjections(m_desc, basicExpression + ";");
			Check(projections);

			projections = SingleTypeMappingDescriptorProvider.GetProjections(m_desc, basicExpression + "; ");
			Check(projections);
		}

		[Test]
		public void Mapping_Fails_For_Duplicate_Projection()
		{
			MemberMappingDescriptor desc = new MemberMappingDescriptor();
			desc.OwnerType = typeof(AttributedUserDTO);
			desc.Type = typeof(string);
			desc.Member = "FullName";
			desc.IsArray = false;
			desc.IsList = false;
			try
			{
				List<ProjectionItem> items = new List<ProjectionItem>();
				items.Add(new ProjectionItem("source", "target_1"));
				items.Add(new ProjectionItem("source", "target_2"));
				ProjectionBuilder.Build(desc, items);
			}
			catch (OtisException e)
			{
				if (e.Message.Contains("Invalid projection 'source => target_2'. 'source' is already mapped to '\"target_1\"'"))
					return; // success
			}
			Assert.Fail("Tested method didn't throw an exception!");  
		}

		[Test]
		public void Projection_Mapping_Fails_For_Nonexisting_Enum_Value()
		{
			MemberMappingDescriptor desc = new MemberMappingDescriptor();
			desc.OwnerType = typeof(AttributedUserDTO);
			desc.Type = typeof(Gender);
			desc.Member = "Gender";
			desc.IsArray = false;
			desc.IsList = false;
			try
			{
				List<ProjectionItem> items = new List<ProjectionItem>();
				items.Add(new ProjectionItem("X", "Undefined"));
				ProjectionBuilder.Build(desc, items);
			}
			catch (OtisException e)
			{
				if (e.Message.Contains("Invalid projection 'X => Undefined'. Value 'Undefined' is not defined in 'Otis.Tests.Gender' enumeration"))
					return; // success
			}
			Assert.Fail("Tested method didn't throw an exception!");  
		}

		[Test]
		public void Projection_Mapping_Fails_For_Missing_To_Attribute_In_Xml()
		{
			string xmlCfg = "<otis-mapping>"
							+ "<class name=\"Otis.Tests.XmlUserDTO, Otis.Tests\" source=\"Otis.Tests.Entity.User, Otis.Tests\" >"
							+ "  <member name=\"Gender\" expression=\"$UserGender\">"
							+ "    <map from=\"['W']\" to=\"\" />"
							+ "  </member>;"
							+ "</class>"
							+ "</otis-mapping>";

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xmlCfg);
			XmlNodeList nodes = xmlDoc.SelectNodes("//member")[0].SelectNodes("map");

			try
			{
				XmlMappingDescriptionProvider.BuildProjections(null, nodes);
			}
			catch (OtisException e)
			{
				if (e.Message.Contains("Invalid projection. Attribute 'to' in XML projection mapping must not be empty"))
					return; // success
			}
			Assert.Fail("Tested method didn't throw an exception!"); 
		}


		private static void Check(ProjectionInfo projections)
		{
			Assert.AreEqual(2, projections.Count);
			Assert.AreEqual("Otis.Tests.Gender.Male", projections["\"M\""]);
			Assert.AreEqual("Otis.Tests.Gender.Female", projections["\"W\""]);
		}

		private void BuildXmlProjections(string xmlCfg)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xmlCfg);
			XmlNodeList xmlGender = xmlDoc.SelectNodes("//member")[0].SelectNodes("map");
			ProjectionInfo projections = XmlMappingDescriptionProvider.BuildProjections(m_desc, xmlGender);
		}
	}
}

