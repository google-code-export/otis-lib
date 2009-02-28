/*
 * Created by: Zdeslav Vojkovic
 * Created: Monday, April 14, 2008
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Otis.Tests.Entity;

namespace Otis.Tests
{
	[TestFixture]
	public class ComponentMappingTests
	{
		string xmlCfg = "<otis-mapping xmlns='urn:otis-mapping-1.0' >"
						+ "    <class name='Otis.Tests.OrderDTO, Otis.Tests' source='Otis.Tests.Order, Otis.Tests' >"
						+ "        <member name='Id' />"
						+ "        <member name='Customer' expression='$Customer.FirstName + &quot; &quot; + $Customer.LastName' />"
						+ "        <component name='PaymentAddress' >"
						+ "            <member name='Street' expression='$PaymentStreet' />"
						+ "            <component name='City' >"
						+ "                <member name='City' expression='$PaymentCity' />"
						+ "                <member name='ZipCode' expression='$PaymentZip' />"
						+ "            </component>"
						+ "        </component>"
						+ "        <component name='ShippingAddress' >"
						+ "            <member name='Street' expression='$ShippingStreet' />"
						+ "            <component name='City' >"
						+ "                <member name='City' expression='$ShippingCity' />"
						+ "                <member name='ZipCode' expression='$ShippingZip' />"
						+ "            </component>"
						+ "        </component>"
						+ "    </class>"
						+ "</otis-mapping>";

		[SetUp]
		public void SetUp() {}

		[TearDown]
		public void TearDown() {}

		[Test]
		public void Component_Is_Recognized_By_XmlProvider()
		{
			IMappingDescriptorProvider provider = ProviderFactory.FromXmlString(xmlCfg);
			Assert.AreEqual(1, provider.ClassDescriptors.Count);
			CheckDescriptor(provider.ClassDescriptors[0]);
		}

		[Test]
		public void Component_Is_Recognized_By_TypeProvider()
		{
			IMappingDescriptorProvider provider = ProviderFactory.FromType(typeof(OrderDTO));
			Assert.AreEqual(1, provider.ClassDescriptors.Count);
			CheckDescriptor(provider.ClassDescriptors[0]);
		}

		[Test]
		public void XmlMapping()
		{
			Configuration cfg = new Configuration();
			cfg.AddXmlString(xmlCfg);

			IAssembler<OrderDTO, Order> asm = cfg.GetAssembler<OrderDTO, Order>();
			Order user = GetDefaultOrder();
		/*	UserWithComponentsDTO dto = asm.AssembleFrom(user);
			Assert.That("X Y", Is.EqualTo(dto.FullName));
			Assert.That(1, Is.EqualTo(dto.Id));
			Assert.That("road to nowhere 1", Is.EqualTo(dto.Address.Street));
			Assert.That("Whatevertown", Is.EqualTo(dto.Address.City.CityName));
			Assert.That("12345", Is.EqualTo(dto.Address.City.ZipCode));	*/
		}

		private Order GetDefaultOrder()
		{
			Order order = new Order();

			order.Id = 1;
			order.Customer.FirstName = "X";
			order.Customer.LastName = "Y";
			order.PaymentStreet = "road to nowhere 1";
			order.PaymentCity = "Whatevertown";
			order.PaymentZip = "12345";
			order.ShippingStreet = "road to nowhere 1";
			order.ShippingCity = "Whatevertown";
			order.ShippingZip = "12345";
			return order;
		}

		private void CheckDescriptor(ClassMappingDescriptor descriptor)
		{
			//throw new Exception("!");
		}

	}

	[MapClass(typeof(Order))]
	public class OrderDTO
	{
		public int Id;
		public string Customer;

		//[Nested("Payment")]
		public Address PaymentAddress = new Address();

		//[Nested("Shipping")]
		public Address ShippingAddress = new Address();
	}

	[MapClass(typeof(Address))]
	public class Address
	{
		//[Map("$PaymentStreet", NestedContext = "Payment")] // [MapNested("$PaymentStreet")] or [MapNested(OrderDTO.PaymentAddress)]
		//[Map("$ShippingStreet", Context = "Shipping")]
		[Map("...")]
		public string Street;
		public CityInfo City = new CityInfo();
	}

	public class CityInfo
	{
		public string CityName;
		public string ZipCode;
	}

	public class Order
	{
		public int Id;
		public User Customer = new User();
		public string PaymentStreet;
		public string PaymentCity;
		public string PaymentZip;
		public string ShippingStreet;
		public string ShippingCity;
		public string ShippingZip;
	}
}

