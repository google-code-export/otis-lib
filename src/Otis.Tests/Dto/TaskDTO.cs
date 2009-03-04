using Otis.Attributes;
using Otis.Tests.Entity;

namespace Otis.Tests.Dto
{
	[MapClass(typeof(Task))]
	public class TaskDTO
	{
		[Map("$Duration * 60")]
		public int DurationInMinutes;
	}
}