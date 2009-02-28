/*
 * Created by: Zdeslav Vojkovic
 * Created: Saturday, September 29, 2007
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Otis.Tests
{
	[TestFixture]
	public class XmlProviderTest
	{
		private const string errMissingFile = "Configuration file 'XmlMappings\\some_non_existing_file.xml' can't be found";
		
		[Test]
		[ExpectedException(typeof(OtisException), ExpectedMessage = errMissingFile)]
		public void Configuration_Fails_With_Missing_File()
		{
			ProviderFactory.FromXmlFile("XmlMappings\\some_non_existing_file.xml");
		}

		[Test]
		[ExpectedException(typeof(OtisException))]
		public void Configuration_Fails_If_Xml_Is_Not_Compliant_With_Schema()
		{
			ProviderFactory.FromXmlFile("XmlMappings\\mapping_non_compliant.xml");
		}

		[Test]
		[ExpectedException(typeof(OtisException))]
		public void Configuration_Fails_If_Xml_Is_Not_Well_Formed()
		{
			ProviderFactory.FromXmlFile("XmlMappings\\mapping_bad_form.xml");
		}

		[Test]
		public void Helper_Is_Recognized()
		{
			string xmlCfg = "<otis-mapping xmlns=\"urn:otis-mapping-1.0\" >"
							+ "<class name=\"Otis.Tests.UserDTO, Otis.Tests\" source=\"Otis.Tests.Entity.User, Otis.Tests\" helper=\"Otis.Tests.Util.Convert\" >"
							+ "<member name=\"Id\" /></class></otis-mapping>";
			IMappingDescriptorProvider provider = ProviderFactory.FromXmlString(xmlCfg);
			Assert.IsFalse(provider.ClassDescriptors[0].HasPreparer);
			Assert.IsTrue(provider.ClassDescriptors[0].HasHelper);
			Assert.IsTrue(provider.ClassDescriptors[0].IsHelperStatic);
			Assert.AreEqual("Otis.Tests.Util.Convert", provider.ClassDescriptors[0].MappingHelper);
		}

		[Test]
		public void Preparer_Is_Recognized()
		{
			string xmlCfg = "<otis-mapping xmlns=\"urn:otis-mapping-1.0\" >"
							+ "<class name=\"Otis.Tests.UserDTO, Otis.Tests\" source=\"Otis.Tests.Entity.User, Otis.Tests\" preparer=\"Otis.Tests.Util.Convert\" >"
							+ "<member name=\"Id\" /></class></otis-mapping>";
			IMappingDescriptorProvider provider = ProviderFactory.FromXmlString(xmlCfg);
			Assert.IsFalse(provider.ClassDescriptors[0].HasHelper);
			Assert.IsTrue(provider.ClassDescriptors[0].HasPreparer);
			Assert.IsTrue(provider.ClassDescriptors[0].IsPreparerStatic);
			Assert.AreEqual("Otis.Tests.Util.Convert", provider.ClassDescriptors[0].MappingPreparer);
		}
	}
}

