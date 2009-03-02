using Otis.Tests.Entity;

namespace Otis.Tests.Dto
{
	[MapClass(typeof(NamedEntity))]
	public class DocumentDTO
	{
		[Map("$Id.ToString() + \" - \" + $Name")]
		public string Description;
	}
}