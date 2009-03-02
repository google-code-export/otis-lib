﻿using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Otis.Tests.Dto;
using Otis.Tests.Entity;
using System.Reflection;

namespace Otis.Tests
{
	[TestFixture]
	public class GenericsMappingTests
	{
		private GenericEntity<int> _source;

		[SetUp]
		public void TestFixtureSetup()
		{
			_source = new GenericEntity<int>();
			_source.Id = 1;
			_source.NullableProperty = 2;
		}

		public IAssembler<XmlGenericEntityDTO, GenericEntity<int>> CreateXmlAssembler()
		{
			Configuration cfg = new Configuration();
			cfg.AddAssemblyReference(Assembly.GetAssembly(typeof(GenericsMappingTests)));
			cfg.AddAssemblyResources(Assembly.GetAssembly(typeof(XmlGenericEntityDTO)), "otis.xml");
			return cfg.GetAssembler<IAssembler<XmlGenericEntityDTO, GenericEntity<int>>>();
		}

		public IAssembler<AttributedGenericEntityDTO, GenericEntity<int>> CreateAttributedAssembler()
		{
			Configuration cfg = new Configuration();
			cfg.AddType(typeof(AttributedGenericEntityDTO));
			// the following 2 lines are there to init Nullable transformation
			cfg.AddAssemblyReference(Assembly.GetAssembly(typeof(GenericsMappingTests)));
			cfg.AddAssemblyResources(Assembly.GetAssembly(typeof(XmlGenericEntityDTO)), "otis.xml");
			
			return cfg.GetAssembler<IAssembler<AttributedGenericEntityDTO, GenericEntity<int>>>();
		}

		[Test]
		public void TestAttributedGenerics()
		{
			IAssembler<AttributedGenericEntityDTO, GenericEntity<int>> asm = CreateAttributedAssembler();

			AttributedGenericEntityDTO dto = asm.AssembleFrom(_source);
			Assert.That(dto.Id, Is.EqualTo(1));
			Assert.That(dto.NullableProperty, Is.EqualTo(2));

			// test nullables
			_source.NullableProperty = null;
			dto = asm.AssembleFrom(_source);
			Assert.That(dto.NullableProperty, Is.Null);
		}

		[Test]
		public void TestXmlGenerics()
		{
			IAssembler<XmlGenericEntityDTO, GenericEntity<int>> asm = CreateXmlAssembler();

			XmlGenericEntityDTO xmlDto = asm.AssembleFrom(_source);
			Assert.That(xmlDto.Id,Is.EqualTo(1));
			Assert.That(xmlDto.NullableProperty, Is.EqualTo(2));

			// test nullables
			_source.NullableProperty = null;
			xmlDto = asm.AssembleFrom(_source);
			Assert.That(xmlDto.NullableProperty, Is.Null);
		}

		public static void ConvertByRef<T>(ref T target, ref T source)
		{
			target = source;
		}
	}
}
