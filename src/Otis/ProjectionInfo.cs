using System.Collections.Generic;

namespace Otis
{
	public class ProjectionInfo : Dictionary<string, string>
	{
		public IEnumerable<ProjectionItem> Items
		{
			get
			{
				foreach (string key in Keys)
				{
					yield return new ProjectionItem(key, this[key]);
				}
				yield break;
			}
		}
	}
}