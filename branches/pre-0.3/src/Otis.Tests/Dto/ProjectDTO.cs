using Otis.Tests.Entity;

namespace Otis.Tests.Dto
{
	[MapClass(typeof(Project))]
	public class ProjectDTO
	{
		[Map]
		public int Id;

		[Map]
		public string Name;

		[Map("$Tasks.Count")]
		public int TaskCount;
	}
}