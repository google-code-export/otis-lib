namespace Otis.Tests
{
	public class DerivedXmlUserDTO : XmlUserDTO
	{
	   private string m_derivedProperty;

	   public string DerivedProperty
	   {
	      get { return m_derivedProperty; }
	      set { m_derivedProperty = value; }
	   }
	}
}
